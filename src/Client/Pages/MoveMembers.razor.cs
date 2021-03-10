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

        private bool SameTeamSelection
        {
            get
            {
                if (LeftSelectionIsNull || RightSelectionIsNull) return false;
                return LeftTeamSelection.ToLower().Equals(RightTeamSelection.ToLower());
            }
        }
        private bool LeftSelectionIsNull => string.IsNullOrWhiteSpace(LeftTeamSelection);
        private bool RightSelectionIsNull => string.IsNullOrWhiteSpace(RightTeamSelection);

        protected bool MemberHasBeenMoved { get; set; } = false;

        protected IEnumerable<EctTeam> InAppTeams { get; set; }

        protected string LeftTeamSelection { get; set; }
        protected string RightTeamSelection { get; set; }

        protected IEnumerable<EctTeam> SelectableTeamsLeft
        {
            get
            {
                if (InAppTeams == null)
                    return new List<EctTeam>();
                return InAppTeams.Where(t => t != RightTeam);
            }
        }

        protected IEnumerable<EctTeam> SelectableTeamsRight
        {
            get
            {
                if (InAppTeams == null)
                    return new List<EctTeam>();
                return InAppTeams.Where(t => t != LeftTeam);
            }
        }

        protected EctTeam LeftTeam
        {
            get
            {
                if (InAppTeams == null || string.IsNullOrWhiteSpace(LeftTeamSelection) || SameTeamSelection)                                                                                   // if there are no in-app teams, no text input, or the team has already been selected, return null
                    return null;
                return InAppTeams.FirstOrDefault(t => t.Name.ToLower().Equals(LeftTeamSelection.ToLower()));
            }
        }
        protected EctTeam RightTeam
        {
            get
            {
                if (InAppTeams == null || string.IsNullOrWhiteSpace(RightTeamSelection)
                    || SameTeamSelection)
                    return null;
                return InAppTeams.FirstOrDefault(t => t.Name.ToLower().Equals(RightTeamSelection.ToLower()));
            }
        }

        protected void RemoveMember(EctTeam team, string emailToRemove)
        {
            team.Members = team.Members.Where(m => m.Email.Equals(emailToRemove) == false).ToList();
        }

        protected void MoveMember(EctTeam fromTeam, EctTeam toTeam, string emailToRemove)
        {
            MemberHasBeenMoved = true;
            var userToMove = fromTeam.Members.FirstOrDefault(m => m.Email.Equals(emailToRemove));
            fromTeam.Members = fromTeam.Members.Where(m => m != userToMove).ToList();
            toTeam.Members.Add(userToMove);
        }

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Move Members");
            InAppTeams = await ApiConn.FetchAllTeams();
        }
    }
}
