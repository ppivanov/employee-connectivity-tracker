using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EctBlazorApp.Shared.Entities
{
    public class CommunicationPoint
    {
        public int Id { get; set; }

        [Required]
        public string Medium { get; set; }

        [Required]
        public int Points { get; set; }

        public CommunicationPoint()
        {
            Medium = string.Empty;
            Points = 0;
        }

        public static CommunicationPoint GetCommunicationPointForMedium(IEnumerable<CommunicationPoint> communicationPoints, string medium)
        {
            try
            {
                CommunicationPoint queryResult = communicationPoints.FirstOrDefault(cp => cp.Medium.ToLower().Contains(medium));
                return queryResult;
            }
            catch (Exception)
            {
                return new CommunicationPoint();
            }
        }
    }
}
