namespace SaveForPerksAPI.Models
{
    public class BusinessWithAdminUserResponseDto
    {
        public BusinessDto Business { get; set; } = null!;
        public BusinessUserDto BusinessUser { get; set; } = null!;
    }
}
