using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        public int PointsThreshold { get; set; }
        public double MarginForNotification { get; set; }

        public ICollection<EctUser> Members { get; set; }

        public List<string> AdditionalUsersToNotify { get; set; }
        public string AdditionalUsersToNotifyAsAtring
        {
            get
            {
                if (AdditionalUsersToNotify.Count < 1)
                    return "";

                return string.Join(" | ", AdditionalUsersToNotify);
            }
            set
            {
                if (value.Length < 1)
                    AdditionalUsersToNotify = new List<string>();
                else
                    AdditionalUsersToNotify = value.Split(" | ").ToList();
            }
        }
    }
}
