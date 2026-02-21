using AutoMapper;
using SaveForPerksAPI.Common;
using SaveForPerksAPI.Entities;
using SaveForPerksAPI.Models;
using SaveForPerksAPI.Repositories;

namespace SaveForPerksAPI.Services;

public class CustomerService : ICustomerService
{
    private readonly ISaveForPerksRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerService> _logger;
    private readonly IQrCodeService _qrCodeService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IAuth0ManagementService _auth0ManagementService;

    public CustomerService(
        ISaveForPerksRepository repository,
        IMapper mapper,
        ILogger<CustomerService> logger,
        IQrCodeService qrCodeService,
        IAuthorizationService authorizationService,
        IAuth0ManagementService auth0ManagementService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _qrCodeService = qrCodeService ?? throw new ArgumentNullException(nameof(qrCodeService));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        _auth0ManagementService = auth0ManagementService ?? throw new ArgumentNullException(nameof(auth0ManagementService));
    }

    public async Task<Result<CustomerDto>> GetCustomerByAuthProviderIdAsync(string authProviderId)
    {
        // 1. Validate input
        if (string.IsNullOrWhiteSpace(authProviderId))
        {
            _logger.LogWarning("GetCustomerByAuthProviderId called with empty authProviderId");
            return Result<CustomerDto>.Failure("Auth provider ID is required");
        }

        // 2. Get Customer by authProviderId
        var user = await _repository.GetCustomerByAuthProviderIdAsync(authProviderId);
        
        if (user == null)
        {
            _logger.LogInformation(
                "Customer not found for authProviderId: {AuthProviderId}", 
                authProviderId);
            return Result<CustomerDto>.Failure("Customer not found");
        }

        // 3. Map and return
        var userDto = _mapper.Map<CustomerDto>(user);
        
        _logger.LogInformation(
            "Customer found for authProviderId: {AuthProviderId}, CustomerId: {CustomerId}, Email: {Email}",
            authProviderId, user.Id, user.Email);

        return Result<CustomerDto>.Success(userDto);
    }

    public async Task<Result<CustomerDto>> CreateCustomerAsync(CustomerForCreationDto request)
    {
        // 1. Validate request
        var validationResult = ValidateCreateUserRequest(request);
        if (validationResult.IsFailure)
            return Result<CustomerDto>.Failure(validationResult.Error!);

        // 2. Validate JWT token matches auth provider ID
        var authCheck = _authorizationService.ValidateAuthProviderIdMatch(request.AuthProviderId);
        if (authCheck.IsFailure)
            return Result<CustomerDto>.Failure(authCheck.Error!);

        // 3. Check for duplicate email
        var emailCheck = await CheckDuplicateEmailAsync(request.Email);
        if (emailCheck.IsFailure)
            return Result<CustomerDto>.Failure(emailCheck.Error!);

        // 4. Check for duplicate authProviderId
        var authProviderCheck = await CheckDuplicateAuthProviderIdAsync(request.AuthProviderId);
        if (authProviderCheck.IsFailure)
            return Result<CustomerDto>.Failure(authProviderCheck.Error!);

        // 5. Generate unique QR code
        var qrCodeResult = await GenerateUniqueQrCodeAsync();
        if (qrCodeResult.IsFailure)
            return Result<CustomerDto>.Failure(qrCodeResult.Error!);

        // 6. Create the user
        var createResult = await CreateCustomerEntityAsync(request, qrCodeResult.Value);
        if (createResult.IsFailure)
            return Result<CustomerDto>.Failure(createResult.Error!);

        var user = createResult.Value;

        // 7. Map and return
        var userDto = _mapper.Map<CustomerDto>(user);

        _logger.LogInformation(
            "Customer created successfully. CustomerId: {CustomerId}, Email: {Email}, AuthProviderId: {AuthProviderId}, QrCodeValue: {QrCodeValue}",
            user.Id, user.Email, user.AuthProviderId, user.QrCodeValue);

        return Result<CustomerDto>.Success(userDto);
    }

    public async Task<Result<CustomerDto>> UpdateCustomerAsync(Guid customerId, CustomerForUpdateDto request)
    {
        // 1. Validate JWT token matches customer's auth provider ID
        var authCheck = await _authorizationService.ValidateCustomerAuthorizationAsync(customerId);
        if (authCheck.IsFailure)
            return Result<CustomerDto>.Failure(authCheck.Error!);

        // 2. Validate request
        var validationResult = ValidateUpdateCustomerRequest(request);
        if (validationResult.IsFailure)
            return Result<CustomerDto>.Failure(validationResult.Error!);

        // 3. Get customer
        var customer = await _repository.GetCustomerByIdAsync(customerId);
        if (customer == null)
        {
            _logger.LogWarning("Customer not found for update. CustomerId: {CustomerId}", customerId);
            return Result<CustomerDto>.Failure("Customer not found");
        }

        // 4. Update customer
        var updateResult = await UpdateCustomerEntityAsync(customer, request);
        if (updateResult.IsFailure)
            return Result<CustomerDto>.Failure(updateResult.Error!);

        var updatedCustomer = updateResult.Value;

        // 5. Map and return
        var customerDto = _mapper.Map<CustomerDto>(updatedCustomer);

        _logger.LogInformation(
            "Customer updated successfully. CustomerId: {CustomerId}, NewName: {Name}",
            customerId, request.Name);

        return Result<CustomerDto>.Success(customerDto);
    }

    private Result<bool> ValidateUpdateCustomerRequest(CustomerForUpdateDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            _logger.LogWarning("Validation failed: Name is required");
            return Result<bool>.Failure("Name is required");
        }

        if (request.Name.Length > 100)
        {
            _logger.LogWarning("Validation failed: Name too long. Length: {Length}", request.Name.Length);
            return Result<bool>.Failure("Name must not exceed 100 characters");
        }

        return Result<bool>.Success(true);
    }

    private async Task<Result<Customer>> UpdateCustomerEntityAsync(Customer customer, CustomerForUpdateDto request)
    {
        try
        {
            var oldName = customer.Name;

            // Update the customer name
            customer.Name = request.Name;

            _logger.LogInformation(
                "Customer entity updated. CustomerId: {CustomerId}, OldName: {OldName}, NewName: {NewName}",
                customer.Id, oldName, customer.Name);

            // Save changes (EF Core tracks changes automatically)
            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Transaction committed successfully. CustomerId: {CustomerId}",
                customer.Id);

            return Result<Customer>.Success(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to update customer. CustomerId: {CustomerId}, Name: {Name}, Error: {Error}",
                customer.Id, request.Name, ex.Message);
            return Result<Customer>.Failure(
                "An error occurred while updating the customer");
        }
    }

    private Result<bool> ValidateCreateUserRequest(CustomerForCreationDto request)
    {
        if (string.IsNullOrWhiteSpace(request.AuthProviderId))
        {
            _logger.LogWarning("Validation failed: AuthProviderId is required");
            return Result<bool>.Failure("Auth provider ID is required");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            _logger.LogWarning("Validation failed: Email is required");
            return Result<bool>.Failure("Email is required");
        }

        // Basic email validation
        if (!request.Email.Contains('@') || !request.Email.Contains('.'))
        {
            _logger.LogWarning("Validation failed: Invalid email format. Email: {Email}", request.Email);
            return Result<bool>.Failure("Invalid email format");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            _logger.LogWarning("Validation failed: Name is required");
            return Result<bool>.Failure("Name is required");
        }

        return Result<bool>.Success(true);
    }

    private async Task<Result<bool>> CheckDuplicateEmailAsync(string email)
    {
        var existingUser = await _repository.GetCustomerByEmailAsync(email);
        if (existingUser != null)
        {
            _logger.LogWarning(
                "Duplicate email detected during user creation. Email: {Email}",
                email);
            return Result<bool>.Failure("A user with this email already exists");
        }

        return Result<bool>.Success(true);
    }

    private async Task<Result<bool>> CheckDuplicateAuthProviderIdAsync(string authProviderId)
    {
        var existingUser = await _repository.GetCustomerByAuthProviderIdAsync(authProviderId);
        if (existingUser != null)
        {
            _logger.LogWarning(
                "Duplicate auth provider ID detected during user creation. AuthProviderId: {AuthProviderId}",
                authProviderId);
            return Result<bool>.Failure("A user with this auth provider ID already exists");
        }

        return Result<bool>.Success(true);
    }

    private async Task<Result<string>> GenerateUniqueQrCodeAsync()
    {
        const int maxAttempts = 10;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var qrCodeValue = _qrCodeService.GenerateQrCodeValue();
            
            if (await _qrCodeService.IsQrCodeUniqueAsync(qrCodeValue))
            {
                _logger.LogInformation(
                    "Unique QR code generated: {QrCodeValue} (attempt {Attempt})",
                    qrCodeValue, attempt + 1);
                return Result<string>.Success(qrCodeValue);
            }

            _logger.LogWarning(
                "QR code collision detected: {QrCodeValue} (attempt {Attempt})",
                qrCodeValue, attempt + 1);
        }

        _logger.LogError("Failed to generate unique QR code after {MaxAttempts} attempts", maxAttempts);
        return Result<string>.Failure("Unable to generate unique QR code. Please try again");
    }

    private async Task<Result<Customer>> CreateCustomerEntityAsync(CustomerForCreationDto request, string qrCodeValue)
    {
        try
        {
            var customerId = Guid.NewGuid();
            var user = new Customer
            {
                Id = customerId,
                AuthProviderId = request.AuthProviderId,
                Email = request.Email,
                Name = request.Name,
                QrCodeValue = qrCodeValue,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateCustomerAsync(user);

            _logger.LogInformation(
                "Customer entity created. CustomerId: {CustomerId}, Email: {Email}, QrCodeValue: {QrCodeValue}",
                customerId, user.Email, user.QrCodeValue);

            // Save changes
            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Transaction committed successfully. CustomerId: {CustomerId}",
                customerId);

            return Result<Customer>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to create user. Email: {Email}, AuthProviderId: {AuthProviderId}, Error: {Error}",
                request.Email, request.AuthProviderId, ex.Message);
            return Result<Customer>.Failure(
                "An error occurred while creating the user");
        }
    }

    public async Task<Result<CustomerDashboardDto>> GetDashboardAsync(Guid customerId)
    {
        try
        {
            // 1. Validate JWT token matches customer's auth provider ID
            var authCheck = await _authorizationService.ValidateCustomerAuthorizationAsync(customerId);
            if (authCheck.IsFailure)
                return Result<CustomerDashboardDto>.Failure(authCheck.Error!);

            _logger.LogInformation("Building dashboard for CustomerId: {CustomerId}", customerId);

            // 2. Get all balances with business and reward details (only where balance > 0)
            var balances = await _repository.GetCustomerBalancesWithDetailsAsync(customerId);
            var balancesList = balances.ToList();

            // 3. Apply expiration logic to all balances and get valid balances
            var validBalances = new List<(CustomerBalance balance, int validBalance)>();
            foreach (var balance in balancesList)
            {
                var validBalance = await ApplyExpirationLogicAsync(customerId, balance.Reward, balance);
                validBalances.Add((balance, validBalance));
            }

            // Filter out expired balances (where validBalance = 0)
            var activeBalances = validBalances.Where(vb => vb.validBalance > 0).ToList();

            // 4. Build Progress (using valid balances only)
            var currentTotalPoints = activeBalances.Sum(vb => vb.validBalance);
            var rewardsAvailable = activeBalances.Count(vb => vb.validBalance >= vb.balance.Reward.CostPoints);

            var progress = new CustomerProgressDto
            {
                CurrentTotalPoints = currentTotalPoints,
                RewardsAvailable = rewardsAvailable
            };

            // 5. Build Achievements
            var lifetimeRewardsClaimed = await _repository.GetLifetimeRewardsClaimedCountAsync(customerId);
            var totalPointsEarned = await _repository.GetLifetimePointsEarnedAsync(customerId);

            var achievements = new CustomerAchievementsDto
            {
                LifetimeRewardsClaimed = lifetimeRewardsClaimed,
                TotalPointsEarned = totalPointsEarned
            };

            // 6. Build Active Businesses (Top 3 by most recent scan, using valid balances)
            var businessesWithScanDates = new List<(CustomerActiveBusinessDto business, DateTime? lastScan)>();

            foreach (var (balance, validBalance) in activeBalances)
            {
                var lastScanDate = await _repository.GetMostRecentScanDateForBusinessAsync(
                    customerId, 
                    balance.Reward.BusinessId);

                var activeBusinessDto = new CustomerActiveBusinessDto
                {
                    Business = _mapper.Map<BusinessDto>(balance.Reward.Business),
                    Balance = validBalance,  // Use valid (non-expired) balance
                    CostPoints = balance.Reward.CostPoints,
                    RewardsAvailable = validBalance / balance.Reward.CostPoints
                };

                businessesWithScanDates.Add((activeBusinessDto, lastScanDate));
            }

            // Sort by most recent scan and take top 3
            var top3Businesses = businessesWithScanDates
                .OrderByDescending(x => x.lastScan ?? DateTime.MinValue)
                .Take(3)
                .Select(x => x.business)
                .ToList();

            // 7. Build Last 30 Days Stats
            var pointsEarned = await _repository.GetLast30DaysPointsEarnedAsync(customerId);
            var scansCompleted = await _repository.GetLast30DaysScansCountAsync(customerId);
            var rewardsClaimed = await _repository.GetLast30DaysRewardsClaimedCountAsync(customerId);

            var last30Days = new CustomerLast30DaysDto
            {
                PointsEarned = pointsEarned,
                ScansCompleted = scansCompleted,
                RewardsClaimed = rewardsClaimed
            };

            // 8. Build Points Expiring in Next 30 Days
            var expiringPoints = new List<CustomerExpiringPointsDto>();

            foreach (var (balance, validBalance) in activeBalances)
            {
                var daysUntilExpiration = await GetDaysUntilExpirationAsync(customerId, balance.Reward, balance);

                // Only include if expiring within 30 days
                if (daysUntilExpiration.HasValue && daysUntilExpiration.Value <= 30)
                {
                    var expiringDto = new CustomerExpiringPointsDto
                    {
                        Business = _mapper.Map<BusinessDto>(balance.Reward.Business),
                        Reward = _mapper.Map<RewardDto>(balance.Reward),
                        Points = validBalance,
                        Rewards = validBalance / balance.Reward.CostPoints,
                        DaysUntilExpiration = daysUntilExpiration.Value
                    };

                    expiringPoints.Add(expiringDto);
                }
            }

            // Sort by days until expiration (most urgent first)
            var sortedExpiringPoints = expiringPoints
                .OrderBy(ep => ep.DaysUntilExpiration)
                .ToList();

            // 9. Build complete dashboard
            var dashboard = new CustomerDashboardDto
            {
                Progress = progress,
                Achievements = achievements,
                Top3Businesses = top3Businesses,
                Last30Days = last30Days,
                PointsExpiringInNext30Days = sortedExpiringPoints
            };

            _logger.LogInformation(
                "Dashboard built successfully for CustomerId: {CustomerId}. TotalPoints: {TotalPoints}, RewardsAvailable: {RewardsAvailable}, Top3Businesses: {Top3Count}",
                customerId, currentTotalPoints, rewardsAvailable, top3Businesses.Count);

            return Result<CustomerDashboardDto>.Success(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to build dashboard for CustomerId: {CustomerId}, Error: {Error}",
                customerId, ex.Message);
            return Result<CustomerDashboardDto>.Failure(
                "An error occurred while loading your dashboard");
        }
    }

    public async Task<Result<bool>> DeleteCustomerAsync(Guid customerId)
    {
        try
        {
            _logger.LogInformation("Starting deletion process for CustomerId: {CustomerId}", customerId);

            // 1. Validate JWT token matches customer's auth provider ID
            var authCheck = await _authorizationService.ValidateCustomerAuthorizationAsync(customerId);
            if (authCheck.IsFailure)
                return Result<bool>.Failure(authCheck.Error!);

            // 2. Get customer (need auth provider ID for Auth0 deletion)
            var customer = await _repository.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer not found for deletion. CustomerId: {CustomerId}", customerId);
                return Result<bool>.Failure("Customer not found");
            }

            // 3. Delete related records in order (most dependent first)
            // Delete customer_balance records
            await _repository.DeleteCustomerBalancesAsync(customerId);
            _logger.LogInformation("Deleted customer balances for CustomerId: {CustomerId}", customerId);

            // Delete reward_redemption records
            await _repository.DeleteRewardRedemptionsAsync(customerId);
            _logger.LogInformation("Deleted reward redemptions for CustomerId: {CustomerId}", customerId);

            // Delete scan_event records
            await _repository.DeleteScanEventsAsync(customerId);
            _logger.LogInformation("Deleted scan events for CustomerId: {CustomerId}", customerId);

            // 4. Delete the customer from database
            await _repository.DeleteCustomerAsync(customer);
            _logger.LogInformation("Deleted customer record for CustomerId: {CustomerId}", customerId);

            // 5. Save all database changes in one transaction
            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Customer deleted from database successfully. CustomerId: {CustomerId}, Email: {Email}, Name: {Name}",
                customerId, customer.Email, customer.Name);

            // 6. Delete from Auth0 (non-blocking - log errors but don't fail the request)
            var auth0Deleted = await _auth0ManagementService.DeleteUserAsync(customer.AuthProviderId);
            if (!auth0Deleted)
            {
                _logger.LogWarning(
                    "Customer deleted from database but failed to delete from Auth0. CustomerId: {CustomerId}, AuthProviderId: {AuthProviderId}. Manual cleanup may be required.",
                    customerId, customer.AuthProviderId);
                // Don't fail the request - database deletion succeeded
            }
            else
            {
                _logger.LogInformation(
                    "Customer deleted from both database and Auth0. CustomerId: {CustomerId}, AuthProviderId: {AuthProviderId}",
                    customerId, customer.AuthProviderId);
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to delete customer. CustomerId: {CustomerId}, Error: {Error}",
                customerId, ex.Message);
            return Result<bool>.Failure(
                "An error occurred while deleting the customer");
        }
    }

    /// <summary>
    /// Checks if customer's balance has expired based on reward's expire_days setting.
    /// If expired, updates balance to 0 in the database.
    /// Returns the current valid balance (0 if expired, actual balance if not).
    /// </summary>
    private async Task<int> ApplyExpirationLogicAsync(Guid customerId, Reward reward, CustomerBalance balance)
    {
        // If no expire_days set, balance never expires
        if (!reward.ExpireDays.HasValue || reward.ExpireDays.Value <= 0)
        {
            return balance.Balance;
        }

        // If balance is already 0, nothing to expire
        if (balance.Balance == 0)
        {
            return 0;
        }

        // Get last activity date (most recent of scan or redemption, date only)
        var lastScanDate = await _repository.GetLastScanDateForCustomerRewardAsync(customerId, reward.Id);
        var lastRedemptionDate = await _repository.GetLastRedemptionDateForCustomerRewardAsync(customerId, reward.Id);

        DateTime? lastActivityDate = null;
        if (lastScanDate.HasValue && lastRedemptionDate.HasValue)
        {
            lastActivityDate = lastScanDate.Value > lastRedemptionDate.Value ? lastScanDate.Value : lastRedemptionDate.Value;
        }
        else
        {
            lastActivityDate = lastScanDate ?? lastRedemptionDate;
        }

        // If no activity at all, balance is considered expired
        if (!lastActivityDate.HasValue)
        {
            _logger.LogInformation(
                "No activity found for customer {CustomerId} on reward {RewardId}. Setting balance to 0.",
                customerId, reward.Id);

            balance.Balance = 0;
            balance.LastUpdated = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return 0;
        }

        // Compare dates only (not time)
        var lastActivityDateOnly = lastActivityDate.Value.Date;
        var todayDateOnly = DateTime.UtcNow.Date;
        var daysSinceActivity = (todayDateOnly - lastActivityDateOnly).Days;

        // If activity is older than expire_days, balance is expired
        if (daysSinceActivity > reward.ExpireDays.Value)
        {
            _logger.LogInformation(
                "Balance expired for customer {CustomerId} on reward {RewardId} ({RewardName}). " +
                "Last activity: {LastActivityDate}, Days since: {DaysSince}, Expire days: {ExpireDays}. Setting balance to 0.",
                customerId, reward.Id, reward.Name, 
                lastActivityDateOnly, daysSinceActivity, reward.ExpireDays.Value);

            balance.Balance = 0;
            balance.LastUpdated = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return 0;
        }

        // Balance is still valid
        _logger.LogDebug(
            "Balance still valid for customer {CustomerId} on reward {RewardId}. " +
            "Days since activity: {DaysSince}, Expire days: {ExpireDays}",
            customerId, reward.Id, daysSinceActivity, reward.ExpireDays.Value);

        return balance.Balance;
    }

    /// <summary>
    /// Calculates days until balance expires. Returns null if no expiration set or already expired.
    /// Returns positive number if expiring in the future.
    /// </summary>
    private async Task<int?> GetDaysUntilExpirationAsync(Guid customerId, Reward reward, CustomerBalance balance)
    {
        // If no expire_days set, balance never expires
        if (!reward.ExpireDays.HasValue || reward.ExpireDays.Value <= 0)
        {
            return null;
        }

        // If balance is 0, nothing to expire
        if (balance.Balance == 0)
        {
            return null;
        }

        // Get last activity date
        var lastScanDate = await _repository.GetLastScanDateForCustomerRewardAsync(customerId, reward.Id);
        var lastRedemptionDate = await _repository.GetLastRedemptionDateForCustomerRewardAsync(customerId, reward.Id);

        DateTime? lastActivityDate = null;
        if (lastScanDate.HasValue && lastRedemptionDate.HasValue)
        {
            lastActivityDate = lastScanDate.Value > lastRedemptionDate.Value ? lastScanDate.Value : lastRedemptionDate.Value;
        }
        else
        {
            lastActivityDate = lastScanDate ?? lastRedemptionDate;
        }

        // If no activity, already expired
        if (!lastActivityDate.HasValue)
        {
            return null;
        }

        // Calculate days until expiration
        var lastActivityDateOnly = lastActivityDate.Value.Date;
        var todayDateOnly = DateTime.UtcNow.Date;
        var daysSinceActivity = (todayDateOnly - lastActivityDateOnly).Days;
        var daysUntilExpiration = reward.ExpireDays.Value - daysSinceActivity;

        // If already expired, return null
        if (daysUntilExpiration <= 0)
        {
            return null;
        }

        return daysUntilExpiration;
    }
}
