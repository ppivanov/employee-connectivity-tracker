using System.ComponentModel.DataAnnotations;

namespace EctBlazorApp.Shared.Entities
{
    public class EctAdmin
    {
        public int Id { get; set; }
        [Required]
        public EctUser User { get; set; }
    }
}
