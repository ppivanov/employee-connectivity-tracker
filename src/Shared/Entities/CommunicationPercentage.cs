using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EctBlazorApp.Shared.Entities
{
    public class CommunicationPercentage
    {
        public int Id { get; set; }

        [Required]
        public string Medium { get; set; }

        [Required]
        public int Weight { get; set; }

        public static CommunicationPercentage GetCommunicationPercentageForMedium(IEnumerable<CommunicationPercentage> communicationPercentages, string medium)
        {
            try
            {
                CommunicationPercentage queryResult = communicationPercentages.First(cp => cp.Medium.ToLower().Contains(medium));
                return queryResult;
            }
            catch (Exception)
            {
                return new CommunicationPercentage
                {
                    Medium = "null",
                    Weight = 0
                };
            }
        }
    }
}
