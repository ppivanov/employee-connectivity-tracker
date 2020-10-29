using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace EctWebApp.Models
{
    public class EctUser
    {
        private string _userEmail;
        private string _userFullName;

        public DateTime LastSignOn { get; set; }
        public List<EctTeam> LeaderOfTeams { get; set; }
        public EctTeam MemberOfTeam { get; set; }
        public string UserEmail
        {
            get
            {
                return _userEmail;
            }
            private set
            {
                bool isValidEmailAddress = false;
                try
                {
                    MailAddress emailAddress = new MailAddress(value);
                    isValidEmailAddress = emailAddress.Address == value;
                }
                catch (FormatException)
                {
                    throw new FormatException("Invalid email format.");
                }
                _userEmail = isValidEmailAddress ? value : UserFullName;
            }
        }
        public string UserFullName
        {
            get 
            { 
                return _userFullName; 
            }
            private set 
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("A user cannot exist without specifying a full name.");
                _userFullName = value; }
        }

        public EctUser(string userEmail, string userFullName, DateTime lastSignOn, List<EctTeam> leaderOfTeams, EctTeam memberOfTeam)
        {
            UserEmail = userEmail;
            UserFullName = userFullName;
            LastSignOn = lastSignOn;
            LeaderOfTeams = leaderOfTeams;
            MemberOfTeam = memberOfTeam;
        }
    }
}
