using EctBlazorApp.Client.Graph;
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

        protected IEnumerable<EctTeam> InAppTeams { get; set; }

        protected EctTeam LeftTeam { get; set; }
        protected string LeftTeamSelection { get; set; }

        protected bool MemberHasBeenMoved { get; set; } = false;

        protected EctTeam RightTeam { get; set; }
        protected string RightTeamSelection { get; set; }

        protected string ServerMessage { get; set; }

        protected string ServerMessageStyle => serverMessageIsError ? "color:red;" : "color:green";

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
            MemberHasBeenMoved = true;
            var userToMove = fromTeam.Members.FirstOrDefault(m => m.Email.Equals(emailToRemove));
            fromTeam.Members = fromTeam.Members.Where(m => m.Email.Equals(userToMove.Email) == false).ToList();
            toTeam.Members.Add(userToMove);
        }

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Move Members");
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

        protected void SubmitChanges()
        {
            if(MemberHasBeenMoved == false)
            {
                serverMessageIsError = true;
                ServerMessage = "No members have been moved.";
                return;
            }
            // Send put request to API
        }

        protected void UpdateLeftTeamSelection()
        {
            if (InAppTeams == null                                                                                      // if there are no in-app teams, no text input, or the team has already been selected, return null
                || string.IsNullOrWhiteSpace(LeftTeamSelection) 
                || SameTeamSelection)
                return;

            LeftTeam = InAppTeams.FirstOrDefault(t => t.Name.ToLower().Equals(LeftTeamSelection.ToLower()));
            InitialLeftTeamRoster = LeftTeam.Members.ToList();
        }

        protected void UpdateRightTeamSelection()
        {
            if (InAppTeams == null 
                || string.IsNullOrWhiteSpace(RightTeamSelection)
                || SameTeamSelection)
                return;

            RightTeam = InAppTeams.FirstOrDefault(t => t.Name.ToLower().Equals(RightTeamSelection.ToLower()));
            InitialRightTeamRoster = RightTeam.Members.ToList();
        }
    }
}
