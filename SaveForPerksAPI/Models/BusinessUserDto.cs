namespace SaveForPerksAPI.Models
{
    public class BusinessUserDto
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string AuthProviderId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
