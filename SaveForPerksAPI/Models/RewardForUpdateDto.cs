using System.ComponentModel.DataAnnotations;

namespace SaveForPerksAPI.Models
{
    public class RewardForUpdateDto
    {
        [Required(ErrorMessage = "Name value is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "RewardType value is required")]
        public string RewardType { get; set; } = string.Empty;

        [Required(ErrorMessage = "CostPoints value is required")]
        public int? CostPoints { get; set; }

        public bool? IsActive { get; set; }
    }
}
