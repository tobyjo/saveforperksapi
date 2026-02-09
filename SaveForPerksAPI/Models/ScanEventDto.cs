namespace SaveForPerksAPI.Models
{
    public class ScanEventDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid RewardId { get; set; }
        public Guid? BusinessUserId { get; set; }

        public string QrCodeValue { get; set; }
        public int PointsChange { get; set; }
        public DateTime ScannedAt { get; set; }
    }
}
