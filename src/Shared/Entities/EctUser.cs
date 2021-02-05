using EctBlazorApp.Shared.GraphModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EctBlazorApp.Shared.Entities
{
    public class EctUser
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public DateTime LastSignIn { get; set; }

        public int? MemberOfId { get; set; }
        public EctTeam MemberOf { get; set; }

        public ICollection<CalendarEvent> CalendarEvents { get; set; }
        public ICollection<EctTeam> LeaderOf { get; set; }
        public ICollection<ReceivedMail> ReceivedEmails { get; set; }
        public ICollection<SentMail> SentEmails { get; set; }

        public EctUser() { }

        public EctUser(GraphUserResponse graphUser)
        {
            Email = graphUser.UserPrincipalName;
            FullName = graphUser.DisplayName;
            LastSignIn = new DateTime(2020, 10, 1); // 1st Oct 2020
        }

        public EctUser(EctUser userToCopy)
        {
            Id = userToCopy.Id;
            Email = userToCopy.Email;
            FullName = userToCopy.FullName;
            LastSignIn = userToCopy.LastSignIn;
            MemberOfId = userToCopy.MemberOfId;
        }
    }
}
