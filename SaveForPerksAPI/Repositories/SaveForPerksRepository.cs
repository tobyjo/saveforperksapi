using Microsoft.EntityFrameworkCore;
using SaveForPerksAPI.DbContexts;
using SaveForPerksAPI.Entities;


namespace SaveForPerksAPI.Repositories
{
    public class SaveForPerksRepository : ISaveForPerksRepository
    {

        private readonly SaveForPerksContext _context;
        public SaveForPerksRepository(SaveForPerksContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }


        public async Task<IEnumerable<Business>> GetBusinessesAsync()
        {
            return await _context.Businesses.OrderBy(lo => lo.Name).ToListAsync();
        }
  

        public async Task<ScanEvent?> GetScanEventAsync(Guid rewardId, Guid scanEventId)
        {
            return await _context.ScanEvents
                .Where(se => se.RewardId == rewardId && se.Id == scanEventId)
                .FirstOrDefaultAsync();
        }

        public async Task CreateScanEvent(ScanEvent scanEvent)
        {
            if (scanEvent == null)
            {
                throw new ArgumentNullException(nameof(scanEvent));
            }
            await _context.ScanEvents.AddAsync(scanEvent);
        }


        public async Task<Customer?> GetCustomerByQrCodeValueAsync(string qrCodeValue)
            {
            return await _context.Customer
                .Where(u => u.QrCodeValue == qrCodeValue)
                .FirstOrDefaultAsync();
        }

        public async Task<Customer?> GetCustomerByAuthProviderIdAsync(string authProviderId)
        {
            return await _context.Customer
                .Where(u => u.AuthProviderId == authProviderId)
                .FirstOrDefaultAsync();
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customer
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task CreateCustomerAsync(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            await _context.Customer.AddAsync(customer);
        }

        public async Task<Reward?> GetRewardAsync(Guid rewardId)
        {
            return await _context.Rewards
                .Where(lp => lp.Id == rewardId)
                .FirstOrDefaultAsync();
        } 

        public async Task<CustomerBalance?> GetCustomerBalanceForRewardAsync(Guid customerId, Guid rewardId)
        {
            return await _context.CustomerBalances
                .Where(ub => ub.CustomerId == customerId && ub.RewardId == rewardId)
                .FirstOrDefaultAsync();
        }

        public async Task CreateUserBalance(CustomerBalance customerBalance)
        {
            if (customerBalance == null)
            {
                throw new ArgumentNullException(nameof(customerBalance));
            }
            await _context.CustomerBalances.AddAsync(customerBalance);
        }

        public async Task<RewardRedemption?> GetRewardRedemptionAsync(Guid rewardId, Guid rewardRedemptionId)
        {
            return await _context.RewardRedemptions
                .Where(rr => rr.RewardId == rewardId && rr.Id == rewardRedemptionId)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateCustomerBalance(CustomerBalance customerBalance)
        {
            // No implementation needed for EF Core as it tracks changes automatically
            await Task.CompletedTask;
        }

        public async Task CreateRewardRedemption(RewardRedemption rewardRedemption)
        {
            if (rewardRedemption == null)
            {
                throw new ArgumentNullException(nameof(rewardRedemption));
            }
            await _context.RewardRedemptions.AddAsync(rewardRedemption);
        }

        public async Task<BusinessUser?> GetBusinessUserByEmailAsync(string email)
        {
            return await _context.BusinessUsers
                .Where(rou => rou.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task CreateBusinessAsync(Business business)
        {
            if (business == null)
            {
                throw new ArgumentNullException(nameof(business));
            }
            await _context.Businesses.AddAsync(business);
        }

        public async Task CreateBusinessUserAsync(BusinessUser businessUser)
        {
            if (businessUser == null)
            {
                throw new ArgumentNullException(nameof(businessUser));
            }
            await _context.BusinessUsers.AddAsync(businessUser);
        }

        public async Task<BusinessUser?> GetBusinessUserByAuthProviderIdAsync(string authProviderId)
        {
            return await _context.BusinessUsers
                .Where(rou => rou.AuthProviderId == authProviderId)
                .FirstOrDefaultAsync();
        }

        public async Task<BusinessUser?> GetBusinessUserByIdAsync(Guid businessUserId)
        {
            return await _context.BusinessUsers
                .Where(rou => rou.Id == businessUserId)
                .FirstOrDefaultAsync();
        }

        public async Task<Business?> GetBusinessByIdAsync(Guid businessId)
        {
            return await _context.Businesses
                .Where(ro => ro.Id == businessId)
                .FirstOrDefaultAsync();
        }

        public async Task<Reward?> GetRewardByBusinessIdAsync(Guid businessId)
        {
            return await _context.Rewards
                .Where(r => r.BusinessId == businessId)
                .FirstOrDefaultAsync();
        }

        public async Task CreateRewardAsync(Reward reward)
        {
            if (reward == null)
            {
                throw new ArgumentNullException(nameof(reward));
            }
            await _context.Rewards.AddAsync(reward);
        }

        public async Task<IEnumerable<BusinessCategory>> GetAllBusinessCategoriesAsync()
        {
            return await _context.BusinessCategories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
