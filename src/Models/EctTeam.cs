using System;
using System.Collections.Generic;

namespace EctWebApp.Models
{
    public class EctTeam
    {
        private string _name;
        private EctUser _leader; 
        public string Name
        {
            get 
            { 
                return _name; 
            }
            private set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Team name cannot be empty.");
                _name = value;
            }
        }
        public EctUser Leader 
        {
            get 
            {
                return _leader;
            } 
            set 
            {
                if(value == null || string.IsNullOrEmpty(value.UserFullName))
                    throw new ArgumentException("A team cannot exist without a team leader.");
                _leader = value;
            } 
        }
        public List<EctUser> Members { get; set; }

        public EctTeam(string name, List<EctUser> members, EctUser leader)
        {
            Name = name;
            Leader = leader;
            Members = members;
        }
    }
}
