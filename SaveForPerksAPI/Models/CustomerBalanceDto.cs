namespace SaveForPerksAPI.Models
{
    public class CustomerBalanceDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid RewardId { get; set; }
        public int Balance { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
