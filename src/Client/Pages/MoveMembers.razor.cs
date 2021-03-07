using EctBlazorApp.Client.Graph;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Pages
{
    public class MoveMembersClass : ComponentBase
    {
        [Inject]
        protected IControllerConnection ApiConn { get; set; }
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        protected List<EctTeam> InAppTeams { get; set; }

        protected string LeftTeamSelection { get; set; }
        protected string RightTeamSelection { get; set; }

        protected EctTeam LeftTeam
        {
            get
            {
                if (InAppTeams == null || string.IsNullOrWhiteSpace(LeftTeamSelection)) return null;
                return InAppTeams.FirstOrDefault(t => t.Name.ToLower().Equals(LeftTeamSelection.ToLower()));
            }
        }
        protected EctTeam RightTeam
        {
            get
            {
                if (InAppTeams == null || string.IsNullOrWhiteSpace(RightTeamSelection)) return null;
                return InAppTeams.FirstOrDefault(t => t.Name.ToLower().Equals(RightTeamSelection.ToLower()));
            }
        }

        protected void RemoveMember(EctTeam team, string emailToRemove)
        {
            team.Members = team.Members.Where(m => m.Email.Equals(emailToRemove) == false).ToList();
        }

        protected void MoveMember(EctTeam fromTeam, EctTeam toTeam, string emailToRemove)
        {
            var userToMove = fromTeam.Members.FirstOrDefault(m => m.Email.Equals(emailToRemove));
            fromTeam.Members = fromTeam.Members.Where(m => m != userToMove).ToList();
            toTeam.Members.Add(userToMove);
        }

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Move Members");
            InAppTeams = (await ApiConn.FetchAllTeams()).ToList();
        }
    }
}
