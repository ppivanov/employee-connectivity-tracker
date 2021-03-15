using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EctBlazorApp.Shared
{
    public class NotificationOptionsResponse
    {
        const int MAX_MARGIN = 100;
        const int MAX_POINTS = 200;
        const int MIN_MARGIN = 0;
        const int MIN_POINTS = 0;
        const string MARGIN_ERROR = "The margin for notifications must be between 0% and 100%";
        const string POINTS_ERROR = "The points threshold must be between 0 and 200";                   // cannot have a constant interpolated string

        public static int MaxMargin => MAX_MARGIN;
        public static int MaxPoints => MAX_POINTS;
        public static int MinMargin => MIN_MARGIN;
        public static int MinPoints => MIN_POINTS;

        public static string MarginErrorMessage => MARGIN_ERROR;
        public static string PointsErrorMessage => POINTS_ERROR;

        [Required]
        [Range(MIN_POINTS, MAX_POINTS, ErrorMessage = POINTS_ERROR)]
        public int PointsThreshold { get; set; }
        [Required]
        [Range(MIN_MARGIN, MAX_MARGIN, ErrorMessage = MARGIN_ERROR)]
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
