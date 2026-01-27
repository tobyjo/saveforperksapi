using System.ComponentModel.DataAnnotations;

namespace SaveForPerksAPI.Models
{
    public class RewardOwnerWithAdminUserForCreationDto
    {
        [Required(ErrorMessage = "RewardOwnerName value is required")]
        public string RewardOwnerName { get; set; } = null!;


        public string RewardOwnerDescription { get; set; } = null!;

        [Required(ErrorMessage = "RewardOwnerUserAuthProviderId value is required")]
        public string RewardOwnerUserAuthProviderId { get; set; } = null!;


        [Required(ErrorMessage = "RewardOwnerUserEmail value is required")]
        public string RewardOwnerUserEmail { get; set; } = null!;

        public string RewardOwnerUserName { get; set; } = null!;

    }
}
