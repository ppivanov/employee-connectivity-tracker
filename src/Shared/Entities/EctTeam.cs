using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EctBlazorApp.Shared.Entities
{
    public class EctTeam
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int LeaderId { get; set; }
        public EctUser Leader { get; set; }
        public ICollection<EctUser> Members { get; set; }
    }
}
