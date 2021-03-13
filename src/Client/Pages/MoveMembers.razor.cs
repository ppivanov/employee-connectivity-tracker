using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Shared;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Pages
{
    public class MoveMembersClass : ComponentBase
    {
        [Inject]
        protected CustomAuthState AuthState { get; set; }
        [Inject]
        protected IControllerConnection ApiConn { get; set; }
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        private bool serverMessageIsError = false;
        private List<EctUser> InitialLeftTeamRoster;
        private List<EctUser> InitialRightTeamRoster;

        private bool LeftSelectionIsNull => string.IsNullOrWhiteSpace(LeftTeamSelection);
        private bool RightSelectionIsNull => string.IsNullOrWhiteSpace(RightTeamSelection);
        private bool SameTeamSelection
        {
            get
            {
                if (LeftSelectionIsNull || RightSelectionIsNull) return false;
                return LeftTeamSelection.ToLower().Equals(RightTeamSelection.ToLower());
            }
        }

        protected bool HasAccess { get; set; } = false;

        protected IEnumerable<EctTeam> InAppTeams { get; set; }

        protected EctTeam LeftTeam { get; set; }
        protected string LeftTeamSelection { get; set; }

        protected bool MemberHasBeenMoved { get; set; } = false;

        protected EctTeam RightTeam { get; set; }
        protected string RightTeamSelection { get; set; }

        protected string ServerMessage { get; set; }

        protected string ServerMessageInlineStyle => serverMessageIsError ? "color:red;" : "color:green";

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

        protected void MoveMember(EctTeam fromTeam, EctTeam toTeam, string emailToRemove)
        {
            if (toTeam == null)
            {
                serverMessageIsError = true;
                ServerMessage = "Select the other team first.";
                return;
            }
            MemberHasBeenMoved = true;
            var userToMove = fromTeam.Members.FirstOrDefault(m => m.Email.Equals(emailToRemove));
            fromTeam.Members = fromTeam.Members.Where(m => m.Email.Equals(userToMove.Email) == false).ToList();
            toTeam.Members.Add(userToMove);
        }

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Move Members");
            await CustomAuthState.GetUserPermissions(AuthState, ApiConn);
            HasAccess = AuthState.IsAdmin;
            if (HasAccess)
                InAppTeams = await ApiConn.FetchAllTeams();
        }

        protected void RemoveMember(EctTeam team, string emailToRemove)
        {
            team.Members = team.Members.Where(m => m.Email.Equals(emailToRemove) == false).ToList();
        }

        protected void ResetTeams()
        {
            LeftTeam.Members = InitialLeftTeamRoster.ToList();                                                          // Copying the list as the MoveMember method will modify the 'snapshot' of the original roster state
            RightTeam.Members = InitialRightTeamRoster.ToList();
            MemberHasBeenMoved = false;
        }

        protected async Task SubmitChanges()
        {
            if(MemberHasBeenMoved == false)
            {
                serverMessageIsError = true;
                ServerMessage = "No members have been moved.";
                return;
            }
            var teams = new List<EctTeam>{ LeftTeam, RightTeam };
            var response = await ApiConn.SubmitMoveMemberTeams(teams);
            serverMessageIsError = response.Item1 == false;
            ServerMessage = response.Item2;
            if(serverMessageIsError == false)
            {
                InitialLeftTeamRoster = LeftTeam.Members.ToList();
                InitialRightTeamRoster = RightTeam.Members.ToList();
            }
        }

        protected void UpdateLeftTeamSelection()
        {
            ResetServerMessage();
            if (InAppTeams == null                                                                                      // if there are no in-app teams, no text input, or the team has already been selected, return null
                || string.IsNullOrWhiteSpace(LeftTeamSelection) 
                || SameTeamSelection)
                return;

            LeftTeam = InAppTeams.FirstOrDefault(t => t.Name.ToLower().Equals(LeftTeamSelection.ToLower()));
            if(LeftTeam != null)
                InitialLeftTeamRoster = LeftTeam.Members.ToList();
        }

        protected void UpdateRightTeamSelection()
        {
            ResetServerMessage();
            if (InAppTeams == null 
                || string.IsNullOrWhiteSpace(RightTeamSelection)
                || SameTeamSelection)
                return;

            RightTeam = InAppTeams.FirstOrDefault(t => t.Name.ToLower().Equals(RightTeamSelection.ToLower()));
            if(RightTeam != null)
                InitialRightTeamRoster = RightTeam.Members.ToList();
        }


        private void ResetServerMessage()
        {
            serverMessageIsError = false;
            ServerMessage = string.Empty;
        }
    }
}
