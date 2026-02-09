using System.ComponentModel.DataAnnotations;

namespace SaveForPerksAPI.Models
{
    public class BusinessWithAdminUserForCreationDto
    {
        [Required(ErrorMessage = "BusinessName value is required")]
        public string BusinessName { get; set; } = null!;


        public string BusinessDescription { get; set; } = null!;

        [Required(ErrorMessage = "BusinessCategoryId value is required")]
        public Guid BusinessCategoryId { get; set; }

        [Required(ErrorMessage = "BusinessUserAuthProviderId value is required")]
        public string BusinessUserAuthProviderId { get; set; } = null!;

        [Required(ErrorMessage = "BusinessUserEmail value is required")]
        public string BusinessUserEmail { get; set; } = null!;

        public string BusinessUserName { get; set; } = null!;

    }
}
