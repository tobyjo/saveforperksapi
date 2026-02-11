namespace SaveForPerksAPI.Models
{
    public class BusinessUserProfileResponseDto
    {
        public bool BusinessProfileExists { get; set; }
        public BusinessDto Business { get; set; } = null!;
        public BusinessUserDto BusinessUser { get; set; } = null!;
        public IEnumerable<RewardDto> Rewards { get; set; } = new List<RewardDto>();
    }
}
