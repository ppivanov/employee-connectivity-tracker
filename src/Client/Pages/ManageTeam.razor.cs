using EctBlazorApp.Client.Graph;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Client.Pages
{
    public class ManageTeamClass : ComponentBase
    {
        [Parameter]
        public string HashedTeamId { get; set; }
        [Inject]
        protected IControllerConnection ApiConn { get; set; }
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        private bool serverMessageIsError = false;
        private HashSet<string> AllAvailableLeaders { get; set; }
        private HashSet<string> MembersFromApi { get; set; }

        protected string CurrentMemberSelection { get; set; } = string.Empty;
        protected bool HasTeamId { get => string.IsNullOrEmpty(HashedTeamId) == false; }
        protected string LeaderInputStyle { get => leaderInputError ? "border: 1px solid red" : string.Empty; }
        protected string MemberInputStyle { get => memberInputError ? "border: 1px solid red" : string.Empty; }
        protected string ServerMessage { get; set; } = string.Empty;
        protected string ServerMessageInlineStyle
        {
            get
            {
                var style = new StringBuilder("text-align: center;");
                var textColor = serverMessageIsError ? "color: red;" : "color: green;";
                style.Append(textColor);

                return style.ToString();
            }
        }
        protected bool TeamNameDisabled { get => string.IsNullOrEmpty(HashedTeamId) == false; }
        protected HashSet<string> AvailableLeaders
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CurrentMemberSelection)
                    || IsStringInMemberFormat(CurrentMemberSelection) == false)
                    return AllAvailableLeaders;

                string selectedEmail = GetEmailFromFormattedString(CurrentMemberSelection);
                var selectableLeaders = AllAvailableLeaders.Where(a =>
                    a.Contains(selectedEmail) == false).ToHashSet();                                    // Copy the rest of the available leaders excluding the current member selection
                return selectableLeaders;
            }
        }
        protected HashSet<string> AvailableMembers
        {
            get
            {
                var selectableMembers = AllAvailableLeaders.ToHashSet();                                   // Copy the available set of members
                selectableMembers.Remove(teamDetails.LeaderNameAndEmail);                                      // Remove the selected leader
                return selectableMembers;
            }
        }

        protected bool hasAccess = false;
        protected bool isSubmitting = false;
        protected bool initialized = false;
        protected bool leaderInputError = false;
        protected bool memberInputError = false;
        protected EctTeamRequestDetails teamDetails;

        public virtual async Task JsInterop(string function, string parameter = "")
        {
            await JsRuntime.InvokeVoidAsync(function, parameter);
        }               // Used to mock JavaScript function calls

        public void SetLeaderNameEmail(ChangeEventArgs args)
        {
            ResetErrorMessage();
            teamDetails.LeaderNameAndEmail = args.Value.ToString();
        }

        public void SetMemberNameEmail(ChangeEventArgs args)
        {
            ResetErrorMessage();
            CurrentMemberSelection = args.Value.ToString();
        }

        protected async Task AddSelectedMember()
        {
            if (IsCurrentMemberSelectionEmpty()
                || IsCurrentMemberSelectionBadlyFormatted(out string selectedEmail)
                || IsCurrentMemberSelectionAlreadyLeader(selectedEmail)
                || IsCurrentMemberSelectionAlreadySelected(selectedEmail)
                || IsCurrentMemberSelectionIneligible(selectedEmail))
                return;

            teamDetails.MemberNamesAndEmails.Add(CurrentMemberSelection);
            AllAvailableLeaders = AllAvailableLeaders.Where(a => a.Contains(selectedEmail) == false).ToHashSet();
            CurrentMemberSelection = string.Empty;

            await InvokeAsync(StateHasChanged);
            await JsInterop("resetCreateTeamMember");
        }

        protected override async Task OnInitializedAsync()
        {
            await Initialize();
        }

        protected override async void OnParametersSet()
        {
            await Initialize();
        }

        private async Task Initialize()
        {
            initialized = false;
            ResetErrorMessage();
            await ResetInputFields();
            if (HasTeamId)
                await InitializeManageTeam();
            else
                await InitializeCreateTeam();

            await InvokeAsync(StateHasChanged);
        }

        protected async Task GetEligibleUsers()
        {
            var response = await ApiConn.GetUsersEligibleForMembers();
            MembersFromApi = response.ToHashSet();
            AllAvailableLeaders = MembersFromApi.ToHashSet();                                       // Copy the set not the reference
        }

        protected async Task RemoveFromSelected(string member)
        {
            teamDetails.MemberNamesAndEmails.Remove(member);
            AvailableLeaders.Add(member);
            await InvokeAsync(StateHasChanged);
        }

        protected async Task SendTeamData()
        {
            if (isSubmitting
                || AreTeamDetailsValid() == false
                || string.IsNullOrWhiteSpace(teamDetails.LeaderNameAndEmail)
                || IsLeaderAlreadySelectedAsMember()
                || IsCurrentLeaderSelectionBadlyFormatted(out string leaderEmail)
                || IsCurrentLeaderSelectionIneligible(leaderEmail)
                )
                return;

            isSubmitting = true;
            teamDetails.TeamId = HasTeamId ? HashedTeamId : string.Empty;
            bool isNewTeam = HasTeamId == false;

            var response = await ApiConn.SubmitTeamData(isNewTeam, teamDetails);
            serverMessageIsError = response.Item1 == false;                                     // is StatusCode of response successful?
            ServerMessage = response.Item2;
            leaderInputError = false;
            memberInputError = false;

            if (isNewTeam && serverMessageIsError == false)
            {
                AllAvailableLeaders.Remove(teamDetails.LeaderNameAndEmail);
                await ResetInputFields();
            }
            isSubmitting = false;
        }

        private bool IsLeaderAlreadySelectedAsMember()
        {
            if (teamDetails.MemberEmails.Contains(teamDetails.LeaderEmail) == false)
                return false;

            serverMessageIsError = true;
            leaderInputError = true;
            ServerMessage = "Team lead is already in member list.";
            isSubmitting = false;
            return true;
        }

        private bool AreTeamDetailsValid()
        {
            if (teamDetails.AreDetailsValid())
                return true;

            serverMessageIsError = true;
            ServerMessage = "Bad request. Please, review inputs and resubmit.";
            isSubmitting = false;
            return false;
        }

        private void ResetErrorMessage()
        {
            leaderInputError = false;
            memberInputError = false;
            serverMessageIsError = false;
            ServerMessage = string.Empty;
        }

        private async Task ResetInputFields()
        {
            if(teamDetails != null)
            {
                teamDetails.Name = string.Empty;
                teamDetails.LeaderNameAndEmail = string.Empty;
                teamDetails.MemberNamesAndEmails = new List<string>();
            }
            if (initialized)
            {
                await JsInterop("resetCreateTeamLeader");
                await JsInterop("resetCreateTeamMember");
            }
        }

        private async Task InitializeCreateTeam()
        {
            await JsInterop("setPageTitle", "Create Team");
            hasAccess = await ApiConn.IsProcessingUserAnAdmin();
            if (hasAccess)
            {
                await GetEligibleUsers();
                teamDetails = new EctTeamRequestDetails
                {
                    MemberNamesAndEmails = new List<string>()
                };
            }
            initialized = true;
        }

        private async Task InitializeManageTeam()
        {
            await JsInterop("setPageTitle", "Manage Team");
            teamDetails = await ApiConn.IsProcessingUserLeaderForTeam(HashedTeamId);
            if (teamDetails == null) hasAccess = false;
            else hasAccess = true;

            if (hasAccess)
            {
                await JsInterop("setPageTitle", teamDetails.Name);
                await GetEligibleUsers();

                initialized = true;                                                                         // Reload the componet to reveal the input field to let JavaScript populate it
                StateHasChanged();
                await JsInterop("setCreateTeamLeader", teamDetails.LeaderNameAndEmail);
            }
            initialized = true;
        }

        private bool IsCurrentMemberSelectionEmpty()
        {
            if (string.IsNullOrWhiteSpace(CurrentMemberSelection))
            {
                memberInputError = true;
                serverMessageIsError = true;
                ServerMessage = "Please, select a member first.";
                return true;
            }
            return false;
        }

        private bool IsCurrentLeaderSelectionBadlyFormatted(out string leaderEmail)
        {
            bool result = IsInputBadlyFormatted(teamDetails.LeaderNameAndEmail, out leaderEmail);
            if (result)
                leaderInputError = true;

            return result;
        }
        private bool IsCurrentMemberSelectionBadlyFormatted(out string selectedEmail)
        {
            bool result = IsInputBadlyFormatted(CurrentMemberSelection, out selectedEmail);
            if(result)
                memberInputError = true;
            
            return result;
        }
        private bool IsInputBadlyFormatted(string input, out string email)                                   // method invoking it needs to set the style of the input
        {
            if (IsStringInMemberFormat(input) == false)
            {
                serverMessageIsError = true;
                ServerMessage = "Please, enter the details in the reqired format.";
                email = string.Empty;
                return true;
            }
            email = GetEmailFromFormattedString(input);
            return false;
        }

        private bool IsCurrentMemberSelectionAlreadyLeader(string selectedEmail)
        {
            if (string.IsNullOrWhiteSpace(teamDetails.LeaderNameAndEmail))
                return false;

            if (teamDetails.LeaderNameAndEmail.Contains(selectedEmail))
            {
                memberInputError = true;
                serverMessageIsError = true;
                ServerMessage = "User already selected as team lead.";
                return true;
            }
            return false;
        }

        private bool IsCurrentMemberSelectionAlreadySelected(string selectedEmail)
        {
            if (teamDetails.MemberEmails.Contains(selectedEmail))
            {
                memberInputError = true;
                serverMessageIsError = true;
                ServerMessage = "This user has already beeen selected.";
                return true;
            }
            return false;
        }

        private bool IsCurrentLeaderSelectionIneligible(string selectedEmail)
        {
            bool result = IsEmailIneligibleForSelection(selectedEmail, AllAvailableLeaders);
            if (result)
                leaderInputError = true;

            return result;
        }
        private bool IsCurrentMemberSelectionIneligible(string selectedEmail)
        {
            bool result = IsEmailIneligibleForSelection(selectedEmail, AvailableMembers);
            if(result)
                memberInputError = true;

            return result;
        }
        private bool IsEmailIneligibleForSelection(string email, IEnumerable<string> list)
        {
            if (list.Any(a => a.Contains(email)) == false)
            {
                serverMessageIsError = true;
                ServerMessage = "This user is not eligible for selection.";
                return true;
            }
            return false;
        }
    }
}
