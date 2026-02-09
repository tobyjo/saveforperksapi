namespace SaveForPerksAPI.Models
{
    public class RewardRedemptionDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid RewardId { get; set; }
        public Guid? BusinessUserId { get; set; }
        public DateTime RedeemedAt { get; set; }
    }
}
