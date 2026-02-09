using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaveForPerksAPI.Models;
using SaveForPerksAPI.Repositories;

namespace SaveForPerksAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin")]
    public class AdminBusinessController : ControllerBase
    {
        private readonly ISaveForPerksRepository tapForPerksRepository;
        private readonly IMapper mapper;

        public AdminBusinessController(ISaveForPerksRepository tapForPerksRepository, IMapper mapper)
        {
            this.tapForPerksRepository = tapForPerksRepository ?? throw new ArgumentNullException(nameof(tapForPerksRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("businesses")]
        public async Task<ActionResult<IEnumerable<BusinessDto>>> GetBusinesses()
        {
            var businesses = await tapForPerksRepository.GetBusinessesAsync();
            var results = mapper.Map<IEnumerable<BusinessDto>>(businesses);
   
            return Ok(results);
        }

    }
}
