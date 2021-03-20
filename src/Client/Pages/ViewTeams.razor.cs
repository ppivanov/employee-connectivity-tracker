using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Shared;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using static EctBlazorApp.Shared.SharedMethods;

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

        protected string FilterTeamsInput { get; set; } = string.Empty;

        protected bool Initialized { get; set; } = false;

        protected EctTeamRequestDetails SelectedTeam { get; set; }

        protected string ServerMessage { get; set; } = string.Empty;
        protected bool ServerMessageIsError { get; set; } = false;


        protected async Task DeleteSelectedTeam()
        {
            string hasedTeamId = ComputeSha256Hash(SelectedTeam.TeamId);
            bool success = await ApiConn.DeleteTeam(hasedTeamId);
            if (success) 
            {
                ServerMessage = $"Successfully deleted {SelectedTeam.Name}";
                ServerMessageIsError = false;
                Teams = Teams.Where(t => t.TeamId != SelectedTeam.TeamId).ToList();
                SelectedTeam = null;
            }
            else
            {
                ServerMessage = $"Failed to delete team. Please, try again later.";
                ServerMessageIsError = true;
            }
        }

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

        protected void UpdateFilterTeamsInput(ChangeEventArgs args)
        {
            FilterTeamsInput = args.Value.ToString();
        }
    }
}