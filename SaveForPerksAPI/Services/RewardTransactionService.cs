using AutoMapper;
using Microsoft.Extensions.Logging;
using SaveForPerksAPI.Common;
using SaveForPerksAPI.Entities;
using SaveForPerksAPI.Models;
using SaveForPerksAPI.Repositories;

namespace SaveForPerksAPI.Services;

public class RewardTransactionService : IRewardTransactionService
{
    private readonly ISaveForPerksRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<RewardTransactionService> _logger;

    public RewardTransactionService(
        ISaveForPerksRepository repository, 
        IMapper mapper,
        ILogger<RewardTransactionService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<BusinessWithAdminUserResponseDto>> CreateBusinessAsync(
        BusinessWithAdminUserForCreationDto request)
    {
        // 1. Validate request
        var validationResult = ValidateCreateBusinessRequest(request);
        if (validationResult.IsFailure)
            return Result<BusinessWithAdminUserResponseDto>.Failure(validationResult.Error!);

        // 2. Check for duplicate email
        var duplicateCheckResult = await CheckDuplicateEmailAsync(request.BusinessUserEmail);
        if (duplicateCheckResult.IsFailure)
            return Result<BusinessWithAdminUserResponseDto>.Failure(duplicateCheckResult.Error!);

        // 3. Create entities in transaction
        var createResult = await CreateBusinessAndUserAsync(request);
        if (createResult.IsFailure)
            return Result<BusinessWithAdminUserResponseDto>.Failure(createResult.Error!);

        var (rewardOwner, rewardOwnerUser) = createResult.Value;

        // 4. Build response
        var response = BuildCreateRewardOwnerResponse(rewardOwner, rewardOwnerUser);

        _logger.LogInformation(
            "Reward owner and admin customer created successfully. RewardOwnerId: {RewardOwnerId}, BusinessUserId: {CustomerId}, Email: {Email}",
            rewardOwner.Id, rewardOwnerUser.Id, rewardOwnerUser.Email);

        return Result<BusinessWithAdminUserResponseDto>.Success(response);
    }

    private Result<bool> ValidateCreateBusinessRequest(BusinessWithAdminUserForCreationDto request)
    {
        if (string.IsNullOrWhiteSpace(request.BusinessName))
        {
            _logger.LogWarning("Validation failed: BusinessName is required");
            return Result<bool>.Failure("Reward owner name is required");
        }

        if (string.IsNullOrWhiteSpace(request.BusinessUserAuthProviderId))
        {
            _logger.LogWarning("Validation failed: Auth provider ID is required");
            return Result<bool>.Failure("Authentication provider ID is required");
        }

        if (string.IsNullOrWhiteSpace(request.BusinessUserEmail))
        {
            _logger.LogWarning("Validation failed: Email is required");
            return Result<bool>.Failure("Email is required");
        }

        // Basic email validation
        if (!request.BusinessUserEmail.Contains('@') || !request.BusinessUserEmail.Contains('.'))
        {
            _logger.LogWarning("Validation failed: Invalid email format. Email: {Email}", request.BusinessUserEmail);
            return Result<bool>.Failure("Invalid email format");
        }

        if (string.IsNullOrWhiteSpace(request.BusinessUserName))
        {
            _logger.LogWarning("Validation failed: Customer name is required");
            return Result<bool>.Failure("Customer name is required");
        }

        return Result<bool>.Success(true);
    }

    private async Task<Result<bool>> CheckDuplicateEmailAsync(string email)
    {
        var existingUser = await _repository.GetBusinessUserByEmailAsync(email);
        if (existingUser != null)
        {
            _logger.LogWarning(
                "Duplicate email detected during reward owner creation. Email: {Email}", 
                email);
            return Result<bool>.Failure("A customer with this email already exists");
        }

        return Result<bool>.Success(true);
    }

    private async Task<Result<(Business rewardOwner, BusinessUser rewardOwnerUser)>> 
        CreateBusinessAndUserAsync(BusinessWithAdminUserForCreationDto request)
    {
        try
        {
            // Create Business
            var businessId = Guid.NewGuid();
            var business = new Business
            {
                Id = businessId,
                Name = request.BusinessName,
                Description = request.BusinessDescription,
                Address = null, // Can be added later if needed
                CreatedAt = DateTime.UtcNow
            };
            await _repository.CreateBusinessAsync(business);

            _logger.LogInformation(
                "Business entity created. BusinessId: {BusinessId}, Name: {Name}",
                businessId, business.Name);

            // Create BusinessUser (admin)
            var businessUserId = Guid.NewGuid();
            var businessUser = new BusinessUser
            {
                Id = businessUserId,
                BusinessId = businessId,
                AuthProviderId = request.BusinessUserAuthProviderId,
                Email = request.BusinessUserEmail,
                Name = request.BusinessUserName,
                IsAdmin = true, // Always admin for this creation flow
                CreatedAt = DateTime.UtcNow
            };
            await _repository.CreateBusinessUserAsync(businessUser);

            _logger.LogInformation(
                "BusinessUser entity created. BusinessUserId: {BusinessUserId}, Email: {Email}, IsAdmin: true, BusinessId: {BusinessId}",
                businessUserId, businessUser.Email, businessId);

            // Save changes (atomic transaction)
            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Transaction committed successfully. BusinessId: {BusinessId}, BusinessUserId: {BusinessUserId}",
                businessId, businessUserId);

            return Result<(Business, BusinessUser)>.Success((business, businessUser));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to create business and admin customer. BusinessName: {Name}, Email: {Email}, Error: {Error}",
                request.BusinessName, request.BusinessUserEmail, ex.Message);
            return Result<(Business, BusinessUser)>.Failure(
                "An error occurred while creating the business account");
        }
    }

    private BusinessWithAdminUserResponseDto BuildCreateRewardOwnerResponse(
        Business business,
        BusinessUser businessUser)
    {
        return new BusinessWithAdminUserResponseDto
        {
            Business = _mapper.Map<BusinessDto>(business),
            BusinessUser = _mapper.Map<BusinessUserDto>(businessUser)
        };
    }

    public async Task<Result<ScanEventResponseDto>> ProcessScanAndRewardsAsync(
        ScanEventForCreationDto request)
    {
        // 1. Validate request
        var validationResult = await ValidateRequestAsync(request);
        if (validationResult.IsFailure)
            return Result<ScanEventResponseDto>.Failure(validationResult.Error!);

        var (user, reward, existingBalance) = validationResult.Value;

        // 2. Process transaction
        var processResult = await ProcessTransactionAsync(user, reward, existingBalance, request);
        if (processResult.IsFailure)
            return Result<ScanEventResponseDto>.Failure(processResult.Error!);

        var (updatedBalance, scanEvent, redemptionIds) = processResult.Value;

        // 3. Build response
        var response = BuildResponse(user, reward, updatedBalance, scanEvent, request.NumRewardsToClaim, redemptionIds);

        return Result<ScanEventResponseDto>.Success(response);
    }

    private async Task<Result<(Customer customer, Reward reward, CustomerBalance? balance)>> 
        ValidateRequestAsync(ScanEventForCreationDto request)
    {
        // Validate customer exists
        var customer = await _repository.GetCustomerByQrCodeValueAsync(request.QrCodeValue);
        if (customer == null)
        {
            _logger.LogWarning(
                "Customer not found. QrCode: {QrCode}, RewardId: {RewardId}", 
                request.QrCodeValue, request.RewardId);
            return Result<(Customer, Reward, CustomerBalance?)>.Failure(
                "Invalid QR code or reward");  // Generic - don't reveal which one failed
        }

        // Validate reward exists
        var reward = await _repository.GetRewardAsync(request.RewardId);
        if (reward == null)
        {
            _logger.LogWarning(
                "Reward not found. RewardId: {RewardId}, CustomerId: {CustomerId}, QrCode: {QrCode}", 
                request.RewardId, customer.Id, request.QrCodeValue);
            return Result<(Customer, Reward, CustomerBalance?)>.Failure(
                "Invalid QR code or reward");  // Generic - don't reveal which one failed
        }

        // Get existing balance
        var balance = await _repository.GetCustomerBalanceForRewardAsync(customer.Id, reward.Id);

        // Validate reward claiming
        if (request.NumRewardsToClaim > 0)
        {
            if (request.NumRewardsToClaim < 0 || request.NumRewardsToClaim > 100)
            {
                _logger.LogWarning(
                    "Invalid claim count. Customer: {CustomerId} ({CustomerName}), Requested: {Count}", 
                    customer.Id, customer.Name, request.NumRewardsToClaim);
                return Result<(Customer, Reward, CustomerBalance?)>.Failure(
                    "Number of rewards to claim must be between 0 and 100");  // OK - validation error
            }

            if (balance == null)
            {
                _logger.LogInformation(
                    "No balance for claim attempt. CustomerId: {CustomerId} ({CustomerName}), RewardId: {RewardId} ({RewardName})", 
                    customer.Id, customer.Name, reward.Id, reward.Name);
                return Result<(Customer, Reward, CustomerBalance?)>.Failure(
                    "You don't have any points for this reward yet");  // Customer-friendly, not revealing IDs
            }

            var currentBalance = balance.Balance;
            var requiredPoints = reward.CostPoints * request.NumRewardsToClaim;
            
            if (currentBalance < requiredPoints)
            {
                _logger.LogInformation(
                    "Insufficient points. Customer: {CustomerId} ({CustomerName}), Required: {Required}, Available: {Available}, Reward: {RewardName}", 
                    customer.Id, customer.Name, requiredPoints, currentBalance, reward.Name);
                return Result<(Customer, Reward, CustomerBalance?)>.Failure(
                    $"Insufficient points. Required: {requiredPoints}, Available: {currentBalance}");  // OK - customer's own data
            }
        }

        return Result<(Customer, Reward, CustomerBalance?)>.Success((customer, reward, balance));
    }

    private async Task<Result<(CustomerBalance balance, ScanEvent scanEvent, List<Guid>? redemptionIds)>> 
        ProcessTransactionAsync(
            Customer customer, 
            Reward reward, 
            CustomerBalance? existingBalance,
            ScanEventForCreationDto request)
    {
        try
        {
            // 1. Add points to balance
            var balance = await AddPointsAsync(customer, reward, existingBalance, request.PointsChange);

            // 2. Claim rewards if requested (deduct points and create redemptions)
            List<Guid>? redemptionIds = null;
            if (request.NumRewardsToClaim > 0)
            {
                redemptionIds = await ClaimRewardsAsync(customer, reward, balance, request.NumRewardsToClaim);
                _logger.LogInformation(
                    "Rewards claimed. Customer: {CustomerId} ({CustomerName}), Reward: {RewardName}, Count: {Count}, PointsDeducted: {Points}", 
                    customer.Id, customer.Name, reward.Name, request.NumRewardsToClaim, reward.CostPoints * request.NumRewardsToClaim);
            }

            // 3. Create scan event
            var scanEvent = await CreateScanEventAsync(customer, reward, request);
            _logger.LogInformation(
                "Scan event created. ScanEventId: {ScanEventId}, Customer: {CustomerId}, Reward: {RewardId}, Points: +{Points}", 
                scanEvent.Id, customer.Id, reward.Id, request.PointsChange);

            // 4. Save all changes
            await _repository.SaveChangesAsync();

            return Result<(CustomerBalance, ScanEvent, List<Guid>?)>.Success((balance, scanEvent, redemptionIds));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Transaction failed. Customer: {CustomerId}, Reward: {RewardId}, Error: {Error}", 
                customer.Id, reward.Id, ex.Message);
            return Result<(CustomerBalance, ScanEvent, List<Guid>?)>.Failure(
                "An error occurred while processing your request");  // Generic for customer
        }
    }

    private async Task<CustomerBalance> AddPointsAsync(
        Customer customer, 
        Reward reward, 
        CustomerBalance? existing, 
        int pointsToAdd)
    {
        if (existing == null)
        {
            var newBalance = new CustomerBalance
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                RewardId = reward.Id,
                Balance = pointsToAdd,
                LastUpdated = DateTime.UtcNow
            };
            await _repository.CreateUserBalance(newBalance);
            return newBalance;
        }

        existing.Balance += pointsToAdd;
        existing.LastUpdated = DateTime.UtcNow;
        return existing;
    }

    private async Task<List<Guid>> ClaimRewardsAsync(Customer customer, Reward reward, CustomerBalance balance, int count)
    {
        var totalCost = reward.CostPoints * count;
        balance.Balance -= totalCost;
        balance.LastUpdated = DateTime.UtcNow;

        var redemptionIds = new List<Guid>();

        for (int i = 0; i < count; i++)
        {
            var redemptionId = Guid.NewGuid();
            var redemption = new RewardRedemption
            {
                Id = redemptionId,
                CustomerId = customer.Id,
                RewardId = reward.Id,
                BusinessUserId = null, // Can be set if you track who processed the redemption
                RedeemedAt = DateTime.UtcNow
            };
            await _repository.CreateRewardRedemption(redemption);
            redemptionIds.Add(redemptionId);
        }

        return redemptionIds;
    }

    private async Task<ScanEvent> CreateScanEventAsync(Customer customer, Reward reward, ScanEventForCreationDto request)
    {
        var scanEvent = new ScanEvent
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            RewardId = reward.Id,
            QrCodeValue = request.QrCodeValue,
            PointsChange = request.PointsChange,
            BusinessUserId = request.BusinessUserId,
            ScannedAt = DateTime.UtcNow
        };
        await _repository.CreateScanEvent(scanEvent);
        return scanEvent;
    }

    private ScanEventResponseDto BuildResponse(
        Customer customer, 
        Reward reward, 
        CustomerBalance balance,
        ScanEvent scanEvent,
        int numRewardsClaimed,
        List<Guid>? redemptionIds)
    {
        var response = new ScanEventResponseDto
        {
            ScanEvent = _mapper.Map<ScanEventDto>(scanEvent),
            CustomerName = customer.Name,
            CurrentBalance = balance.Balance,
            RewardAvailable = false,
            AvailableReward = null,
            NumRewardsAvailable = 0,
            ClaimedRewards = null
        };

        // Add claimed rewards info if any were claimed
        if (numRewardsClaimed > 0 && redemptionIds != null)
        {
            response.ClaimedRewards = new ClaimedRewardsDto
            {
                NumberClaimed = numRewardsClaimed,
                RewardName = reward.Name,
                TotalPointsDeducted = reward.CostPoints * numRewardsClaimed,
                RedemptionIds = redemptionIds
            };
        }

        // Check if rewards are still available after this transaction
        if (reward.RewardType == RewardType.IncrementalPoints && 
            reward.CostPoints > 0 && 
            balance.Balance >= reward.CostPoints)
        {
            response.RewardAvailable = true;
            response.NumRewardsAvailable = balance.Balance / reward.CostPoints;
            response.AvailableReward = new AvailableRewardDto
            {
                RewardId = reward.Id,
                RewardName = reward.Name,
                RewardType = "incremental_points",
                RequiredPoints = reward.CostPoints
            };
        }

        return response;
    }

    public async Task<Result<CustomerBalanceAndInfoResponseDto>> GetCustomerBalanceForRewardAsync(
        Guid rewardId, 
        string qrCodeValue)
    {
        // 1. Validate customer and reward exist
        var validationResult = await ValidateUserAndRewardAsync(rewardId, qrCodeValue);
        if (validationResult.IsFailure)
            return Result<CustomerBalanceAndInfoResponseDto>.Failure(validationResult.Error!);

        var (customer, reward) = validationResult.Value;

        // 2. Get customer balance
        var balance = await _repository.GetCustomerBalanceForRewardAsync(customer.Id, rewardId);

        // 3. Build response
        var response = BuildUserBalanceResponse(customer, reward, balance, qrCodeValue);

        return Result<CustomerBalanceAndInfoResponseDto>.Success(response);
    }

    private async Task<Result<(Customer user, Reward reward)>> ValidateUserAndRewardAsync(
        Guid rewardId, 
        string qrCodeValue)
    {
        var user = await _repository.GetCustomerByQrCodeValueAsync(qrCodeValue);
        if (user == null)
        {
            _logger.LogWarning(
                "Customer balance check: Customer not found. QrCode: {QrCode}, RewardId: {RewardId}", 
                qrCodeValue, rewardId);
            return Result<(Customer, Reward)>.Failure(
                "Invalid QR code or reward");  // Generic
        }

        var reward = await _repository.GetRewardAsync(rewardId);
        if (reward == null)
        {
            _logger.LogWarning(
                "Customer balance check: Reward not found. RewardId: {RewardId}, CustomerId: {CustomerId}, QrCode: {QrCode}", 
                rewardId, user.Id, qrCodeValue);
            return Result<(Customer, Reward)>.Failure(
                "Invalid QR code or reward");  // Generic
        }

        return Result<(Customer, Reward)>.Success((user, reward));
    }

    private CustomerBalanceAndInfoResponseDto BuildUserBalanceResponse(
        Customer customer, 
        Reward reward, 
        CustomerBalance? balance,
        string qrCodeValue)
    {
        var response = new CustomerBalanceAndInfoResponseDto
        {
            QrCodeValue = qrCodeValue,
            CustomerName = customer.Name,
            CurrentBalance = balance?.Balance ?? 0,
            AvailableReward = null,
            NumRewardsAvailable = 0
        };

        // If no balance exists, return empty response
        if (balance == null)
            return response;

        // Check if reward is available based on reward type
        if (reward.RewardType == RewardType.IncrementalPoints && 
            reward.CostPoints > 0 && 
            balance.Balance >= reward.CostPoints)
        {
            response.NumRewardsAvailable = balance.Balance / reward.CostPoints;
            response.AvailableReward = new AvailableRewardDto
            {
                RewardId = reward.Id,
                RewardName = reward.Name,
                RewardType = "incremental_points",
                RequiredPoints = reward.CostPoints
            };
        }

        return response;
    }

    public async Task<Result<ScanEventDto>> GetScanEventForRewardAsync(
        Guid rewardId, 
        Guid scanEventId)
    {
        var scanEvent = await _repository.GetScanEventAsync(rewardId, scanEventId);
        
        if (scanEvent == null)
        {
            _logger.LogWarning(
                "Scan event not found. ScanEventId: {ScanEventId}, RewardId: {RewardId}", 
                scanEventId, rewardId);
            return Result<ScanEventDto>.Failure(
                "Scan event not found");  // Generic - don't reveal IDs
        }

        var scanEventDto = _mapper.Map<ScanEventDto>(scanEvent);
        return Result<ScanEventDto>.Success(scanEventDto);
    }
}


