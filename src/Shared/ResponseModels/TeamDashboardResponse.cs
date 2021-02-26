using EctBlazorApp.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EctBlazorApp.Shared
{
    public class TeamDashboardResponse
    {
        public string TeamName { get; set; }

        public List<EctUser> TeamMembers { get; set; }
    }
}
