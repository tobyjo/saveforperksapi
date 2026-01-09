using Microsoft.EntityFrameworkCore;
using TapForPerksAPI.DbContexts;
using TapForPerksAPI.Entities;


namespace TapForPerksAPI.Repositories
{
    public class TapForPerksRepository : ITapForPerksRepository
    {

        private readonly TapForPerksContext _context;
        public TapForPerksRepository(TapForPerksContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<LoyaltyOwner>> GetLoyaltyOwnersAsync()
        {
            return await _context.LoyaltyOwners.OrderBy(lo => lo.Name).ToListAsync();
        }
  
   

    }
}
