using EctBlazorApp.Shared.ValidationAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Shared
{
    public class GraphUserRequestDetails
    {
        public string GraphToken { get; set; }
    }

    public class EctTeamRequestDetails
    {
        [Required (ErrorMessage = "Please, enter a team name.")]
        [StringLength(50, ErrorMessage = "Team name is too long. Please, use less than 50 characters.")]
        public string Name { get; set; }
        [Required (ErrorMessage = "Please, select a team leader.")]
        [MemberFormat (ErrorMessage = "Incorrect format for team lead.")]
        public string LeaderNameAndEmail { get; set; }
        [CollectionNotEmpty(ErrorMessage = "You must select at least one member.")]
        public List<string> MemberNamesAndEmails { get; set; }

        public IEnumerable<string> MemberEmails 
        {
            get => MemberNamesAndEmails.Select(m => GetEmailFromFormattedString(m));
        }

        public bool AreDetailsValid() 
        {
            if (string.IsNullOrEmpty(Name) || Name.Length > 50)
                return false;

            if (IsStringInMemberFormat(LeaderNameAndEmail) == false)
                return false;

            string leaderEmail = GetEmailFromFormattedString(LeaderNameAndEmail);
            if (leaderEmail.IsValidEmail() == false)
                return false;

            if (MemberNamesAndEmails == null || MemberNamesAndEmails.Count < 1)
                return false;

            foreach (var member in MemberNamesAndEmails)
            {
                if (IsStringInMemberFormat(member) == false) return false;
                string memberEmail = GetEmailFromFormattedString(member);
                if (memberEmail.IsValidEmail() == false)
                    return false;
            }
            return true;
        }
    }
}
