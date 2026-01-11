using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TapForPerksAPI.Models;
using TapForPerksAPI.Repositories;

namespace TapForPerksAPI.Controllers.LoyaltyOwner
{
    [ApiController]
    [Route("api/lo/scans")]
    public class LoyaltyOwnerScanController : ControllerBase
    {
        private readonly ITapForPerksRepository tapForPerksRepository;
        private readonly IMapper mapper;

    
        public LoyaltyOwnerScanController(ITapForPerksRepository tapForPerksRepository, IMapper mapper)
        {
            this.tapForPerksRepository = tapForPerksRepository ?? throw new ArgumentNullException(nameof(tapForPerksRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{loyaltyProgrammeId}/events/{scanEventId}", Name = "GetScanEventForLoyaltyProgramme")]
        public async Task<ActionResult<ScanEventDto>> GetScanEventForLoyaltyProgramme(Guid loyaltyProgrammeId, Guid scanEventId)
        {
            var scanEventEntity = await tapForPerksRepository.GetScanEventAsync(loyaltyProgrammeId, scanEventId);
            if (scanEventEntity == null)
            {
                return NotFound();
            }
            var scanEventToReturn = mapper.Map<ScanEventDto>(scanEventEntity);
            return Ok(scanEventToReturn);
        }

        [HttpPost]
        public async Task<ActionResult<ScanEventDto>> CreateScanEventForLoyaltyProgramme(ScanEventForCreationDto scanEventForCreationDto)
        {
            var scanEventEntity = mapper.Map<Entities.ScanEvent>(scanEventForCreationDto);

            // Lookup user_id for this qrcode_value
            var userEntity = await tapForPerksRepository.GetUserByQrCodeValueAsync(scanEventEntity.QrCodeValue);
            if (userEntity == null)
            {
                return NotFound("User not found");
            }

            scanEventEntity.UserId = userEntity.Id;

            //

            await tapForPerksRepository.AddScanEvent(scanEventEntity);
            await tapForPerksRepository.SaveChangesAsync();

            var scanEventToReturn = mapper.Map<ScanEventDto>(scanEventEntity);
            return CreatedAtRoute("GetScanEventForLoyaltyProgramme",
                new { loyaltyProgrammeId = scanEventToReturn.LoyaltyProgrammeId, scanEventId = scanEventToReturn.Id },
                scanEventToReturn);
        }

        [HttpGet("History")]
        public async Task<ActionResult<IEnumerable<LoyaltyOwnerDto>>> GetScansHistoryForLoyaltyProgramme()
        {
            /*
            var loyaltyOwners = await tapForPerksRepository.GetLoyaltyOwnersAsync();
            var results = mapper.Map<IEnumerable<LoyaltyOwnerDto>>(loyaltyOwners);
            */
            return Ok(true);
        }
    }
}
