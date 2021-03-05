using EctBlazorApp.Shared.Entities;
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

        public string TeamId { get; set; }
        public string LeaderEmail { get => GetEmailFromFormattedString(LeaderNameAndEmail); }

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

        public EctTeamRequestDetails() {  }

        public EctTeamRequestDetails(EctTeam ectTeam)
        {
            Name = ectTeam.Name;
            LeaderNameAndEmail = FormatFullNameAndEmail(ectTeam.Leader.FullName, ectTeam.Leader.Email);
            MemberNamesAndEmails = ectTeam.Members.Where(m => m.Email.Equals(ectTeam.Leader.Email) == false)
                .Select(m => FormatFullNameAndEmail(m.FullName, m.Email)).ToList();
        }
    }
}
