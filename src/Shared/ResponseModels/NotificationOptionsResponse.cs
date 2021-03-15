using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EctBlazorApp.Shared
{
    public class NotificationOptionsResponse
    {
        const int minPoints = 0;
        const int maxPoints = 200;
        const int minMargin = 0;
        const int maxMargin = 100;

        [Required]
        [Range(minPoints, maxPoints, ErrorMessage = "The points threshold must be between {minPoints} and {maxPoints}")]
        public int PointsThreshold { get; set; }
        [Required]
        [Range(minMargin, maxMargin, ErrorMessage = "The margin for notifications must be between {minMargin}% and {maxMargin}%")]
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
