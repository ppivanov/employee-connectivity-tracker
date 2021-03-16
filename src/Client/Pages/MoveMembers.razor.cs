using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Shared;
using EctBlazorApp.Shared;
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
        protected CustomAuthState AuthState { get; set; }
        [Inject]
        protected IControllerConnection ApiConn { get; set; }
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        private bool serverMessageIsError = false;
        private List<string> InitialLeftTeamRoster;
        private List<string> InitialRightTeamRoster;

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

        protected IEnumerable<EctTeamRequestDetails> InAppTeams { get; set; }

        protected EctTeamRequestDetails LeftTeam { get; set; }
        protected string LeftTeamSelection { get; set; }

        protected bool MemberHasBeenMoved { get; set; } = false;

        protected EctTeamRequestDetails RightTeam { get; set; }
        protected string RightTeamSelection { get; set; }

        protected string ServerMessage { get; set; }

        protected string ServerMessageInlineStyle => serverMessageIsError ? "color:red;" : "color:green";

        protected IEnumerable<EctTeamRequestDetails> SelectableTeamsLeft
        {
            get
            {
                if (InAppTeams == null)
                    return new List<EctTeamRequestDetails>();
                return InAppTeams.Where(t => t != RightTeam);
            }
        }

        protected IEnumerable<EctTeamRequestDetails> SelectableTeamsRight
        {
            get
            {
                if (InAppTeams == null)
                    return new List<EctTeamRequestDetails>();
                return InAppTeams.Where(t => t != LeftTeam);
            }
        }

        protected void MoveMember(EctTeamRequestDetails fromTeam, EctTeamRequestDetails toTeam, string emailToRemove)
        {
            if (toTeam == null)
            {
                serverMessageIsError = true;
                ServerMessage = "Select the other team first.";
                return;
            }
            MemberHasBeenMoved = true;
            var userToMove = fromTeam.MemberNamesAndEmails.FirstOrDefault(m => m.Equals(emailToRemove));
            fromTeam.MemberNamesAndEmails = fromTeam.MemberNamesAndEmails.Where(m => m.Equals(userToMove) == false).ToList();
            toTeam.MemberNamesAndEmails.Add(userToMove);
        }

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Move Members");
            await CustomAuthState.GetUserPermissions(AuthState, ApiConn);
            HasAccess = AuthState.IsAdmin;
            if (HasAccess)
                InAppTeams = await ApiConn.FetchAllTeams();
        }

        protected void RemoveMember(EctTeamRequestDetails team, string emailToRemove)
        {
            MemberHasBeenMoved = true;
            team.MemberNamesAndEmails = team.MemberNamesAndEmails.Where(m => m.Equals(emailToRemove) == false).ToList();
        }

        protected void ResetTeams()
        {
            if(LeftTeam !=null)
                LeftTeam.MemberNamesAndEmails = InitialLeftTeamRoster.ToList();                                                          // Copying the list as the MoveMember method will modify the 'snapshot' of the original roster state

            if (RightTeam != null)
                RightTeam.MemberNamesAndEmails = InitialRightTeamRoster.ToList();

            MemberHasBeenMoved = false;
            ResetServerMessage();
        }

        protected async Task SubmitChanges()
        {
            if(IsMemberMoved() == false || AnyMembersInTeams() == false)
                return;

            var teams = new List<EctTeamRequestDetails> { LeftTeam, RightTeam };
            var response = await ApiConn.SubmitMoveMemberTeams(teams);
            serverMessageIsError = response.Item1 == false;
            ServerMessage = response.Item2;
            if(serverMessageIsError == false)
            {
                InitialLeftTeamRoster = LeftTeam.MemberNamesAndEmails.ToList();
                InitialRightTeamRoster = RightTeam.MemberNamesAndEmails.ToList();
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
                InitialLeftTeamRoster = LeftTeam.MemberNamesAndEmails.ToList();
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
                InitialRightTeamRoster = RightTeam.MemberNamesAndEmails.ToList();
        }


        private void ResetServerMessage()
        {
            serverMessageIsError = false;
            ServerMessage = string.Empty;
        }
        private bool IsMemberMoved()
        {
            if (LeftTeam == null || RightTeam == null || MemberHasBeenMoved == false)
            {
                serverMessageIsError = true;
                ServerMessage = "No members have been moved.";
                return false;
            }

            return true;
        }
        private bool AnyMembersInTeams()
        {
            if(LeftTeam.MemberNamesAndEmails.Count < 1 || RightTeam.MemberNamesAndEmails.Count < 1)
            {
                serverMessageIsError = true;
                ServerMessage = "Teams must have at least one member.";
                return false;
            }

            return true;
        }
    }
}
