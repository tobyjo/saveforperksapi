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

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }


        public async Task<IEnumerable<LoyaltyOwner>> GetLoyaltyOwnersAsync()
        {
            return await _context.LoyaltyOwners.OrderBy(lo => lo.Name).ToListAsync();
        }
  

        public async Task<ScanEvent?> GetScanEventAsync(Guid loyaltyProgrammeId, Guid scanEventId)
        {
            return await _context.ScanEvents
                .Where(se => se.LoyaltyProgrammeId == loyaltyProgrammeId && se.Id == scanEventId)
                .FirstOrDefaultAsync();
        }

        public async Task AddScanEvent(ScanEvent scanEvent)
        {
            if (scanEvent == null)
            {
                throw new ArgumentNullException(nameof(scanEvent));
            }
            await _context.ScanEvents.AddAsync(scanEvent);
        }


        public async Task<User?> GetUserByQrCodeValueAsync(string qrCodeValue)
            {
            return await _context.Users
                .Where(u => u.QrCodeValue == qrCodeValue)
                .FirstOrDefaultAsync();
        }


    }
}
