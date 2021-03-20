using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Shared;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Components;

namespace EctBlazorApp.Client.Pages
{
    public class ViewTeamsClass : ComponentBase
    {
        [Inject]
        protected CustomAuthState AuthState { get; set; }
        [Inject]
        protected IControllerConnection ApiConn { get; set; }
        [Inject]
        protected DashboardState DashboardState { get; set; }

        protected List<EctTeamRequestDetails> Teams { get; set; } = new List<EctTeamRequestDetails>();

        protected bool Initialized { get; set; } = false;
        protected EctTeamRequestDetails SelectedTeam { get; set; }

        protected void ExpandTeam(EctTeamRequestDetails team)
        {
            if(SelectedTeam == team)
                SelectedTeam = null;
            else 
                SelectedTeam = team;
        }

        protected override async Task OnInitializedAsync()
        {  
            Initialized = false;
            DashboardState.SetIsDrillDown(false);
            await CustomAuthState.GetUserPermissions(AuthState, ApiConn);
            Teams = (await ApiConn.FetchAllTeams()).ToList();
            Initialized = true;
        }
    }
}