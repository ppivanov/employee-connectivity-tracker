using System.Collections.Generic;

namespace EctBlazorApp.Shared
{
    public class EctTeam
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LeaderId { get; set; }
        public EctUser Leader { get; set; }
        public ICollection<EctUser> Members { get; set; }
    }
}
