using System.ComponentModel.DataAnnotations;

namespace EctBlazorApp.Shared.Entities
{
    public class CommunicationPercentage
    {
        public int Id { get; set; }

        [Required]
        public string Medium { get; set; }

        [Required]
        public string Unit { get; set; }

        [Required]
        public int Weight { get; set; }
    }
}
