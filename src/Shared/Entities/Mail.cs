using System.ComponentModel.DataAnnotations;

namespace EctBlazorApp.Shared.Entities
{
    public abstract class Mail
    {
        public int Id { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public int EctUserId { get; set; }
    }
}
