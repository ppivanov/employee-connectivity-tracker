using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EctBlazorApp.Shared
{
    public class NotificationOptionsResponse
    {
        [Required]
        [Range(0, 200, ErrorMessage = "The points threshold must be between {0} and {1}")]
        public int PointsThreshold { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "The margin for notifications must be between {0}% and {1}%")]
        public double MarginForNotification { get; set; }
        public List<string> UsersToNotify { get; set; }

        public NotificationOptionsResponse() {  }

        public NotificationOptionsResponse(NotificationOptionsResponse optionsToCopy) 
        {
            PointsThreshold = optionsToCopy.PointsThreshold;
            MarginForNotification = optionsToCopy.MarginForNotification;
            UsersToNotify = optionsToCopy.UsersToNotify.ToList();                                           // Make a copy of the users
        }

    }
}
