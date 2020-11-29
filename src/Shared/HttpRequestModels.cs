using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EctBlazorApp.Shared
{
    public class GraphUserRequestDetails
    {
        public string GraphToken { get; set; }
    }

    public class EctTeamRequestDetails
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public string LeaderEmail { get; set; }
        public List<string> MemberEmails { get; set; }

        public bool AreDetailsValid() 
        {
            if (string.IsNullOrEmpty(Name) || Name.Length > 50)
                return false;

            if (!LeaderEmail.IsValidEmail())
                return false;

            foreach (var memberEmail in MemberEmails)
            {
                if (!memberEmail.IsValidEmail())
                    return false;
            }
            return true;
        }
    }
}
