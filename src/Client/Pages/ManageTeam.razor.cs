using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Shared;
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
        protected AuthState AuthState { get; set; }
        [Inject]
        protected IControllerConnection ApiConn { get; set; }
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        private bool serverMessageIsError = false;
        private HashSet<string> AllAvailableLeaders { get; set; }
        private HashSet<string> MembersFromApi { get; set; }

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
                selectableMembers.Remove(TeamDetails.LeaderNameAndEmail);                                      // Remove the selected leader
                return selectableMembers;
            }
        }
        protected string CurrentMemberSelection { get; set; } = string.Empty;
        protected bool HasTeamId { get => string.IsNullOrEmpty(HashedTeamId) == false; }
        protected string LeaderInputStyle { get => LeaderInputError ? "border: 1px solid red" : string.Empty; }
        protected string MemberInputStyle { get => MemberInputError ? "border: 1px solid red" : string.Empty; }
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

        protected bool HasAccess { get; set; } = false;
        protected bool IsSubmitting { get; set; } = false;
        protected bool Initialized { get; set; } = false;
        protected bool LeaderInputError { get; set; } = false;
        protected bool MemberInputError { get; set; } = false;
        protected EctTeamRequestDetails TeamDetails { get; set; }

        public virtual async Task JsInterop(string function, string parameter = "")
        {
            await JsRuntime.InvokeVoidAsync(function, parameter);
        }               // Used to mock JavaScript function calls

        public void SetLeaderNameEmail(ChangeEventArgs args)
        {
            ResetErrorMessage();
            TeamDetails.LeaderNameAndEmail = args.Value.ToString();
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

            TeamDetails.MemberNamesAndEmails.Add(CurrentMemberSelection);
            AllAvailableLeaders = AllAvailableLeaders.Where(a => a.Contains(selectedEmail) == false).ToHashSet();
            CurrentMemberSelection = string.Empty;

            await InvokeAsync(StateHasChanged);
            await JsInterop("resetCreateTeamMember");
        }

        protected async Task GetEligibleUsers()
        {
            var response = await ApiConn.GetUsersEligibleForMembers();
            MembersFromApi = response.ToHashSet();
            AllAvailableLeaders = MembersFromApi.ToHashSet();                                       // Copy the set not the reference
        }

        protected override async Task OnInitializedAsync()
        {
            await Initialize();
        }

        protected override async Task OnParametersSetAsync()
        {
            await Initialize();
        }

        protected async Task RemoveFromSelected(string member)
        {
            TeamDetails.MemberNamesAndEmails.Remove(member);
            AvailableLeaders.Add(member);
            await InvokeAsync(StateHasChanged);
        }

        protected async Task SendTeamData()
        {
            if (IsSubmitting
                || AreTeamDetailsValid() == false
                || string.IsNullOrWhiteSpace(TeamDetails.LeaderNameAndEmail)
                || IsLeaderAlreadySelectedAsMember()
                || IsCurrentLeaderSelectionBadlyFormatted(out string leaderEmail)
                || IsCurrentLeaderSelectionIneligible(leaderEmail)
                )
                return;

            IsSubmitting = true;
            TeamDetails.TeamId = HasTeamId ? HashedTeamId : string.Empty;
            bool isNewTeam = HasTeamId == false;

            var response = await ApiConn.SubmitTeamData(isNewTeam, TeamDetails);
            serverMessageIsError = response.Item1 == false;                                     // is StatusCode of response successful?
            ServerMessage = response.Item2;
            LeaderInputError = false;
            MemberInputError = false;

            if (isNewTeam && serverMessageIsError == false)
            {
                AllAvailableLeaders.Remove(TeamDetails.LeaderNameAndEmail);
                await ResetInputFields();
            }
            IsSubmitting = false;
        }


        private bool IsLeaderAlreadySelectedAsMember()
        {
            if (TeamDetails.MemberEmails.Contains(TeamDetails.LeaderEmail) == false)
                return false;

            serverMessageIsError = true;
            LeaderInputError = true;
            ServerMessage = "Team lead is already in member list.";
            IsSubmitting = false;
            return true;
        }
        private bool AreTeamDetailsValid()
        {
            if (TeamDetails.AreDetailsValid())
                return true;

            serverMessageIsError = true;
            ServerMessage = "Bad request. Please, review inputs and resubmit.";
            IsSubmitting = false;
            return false;
        }
        private void ResetErrorMessage()
        {
            LeaderInputError = false;
            MemberInputError = false;
            serverMessageIsError = false;
            ServerMessage = string.Empty;
        }
        private async Task ResetInputFields()
        {
            if(TeamDetails != null)
            {
                TeamDetails.Name = string.Empty;
                TeamDetails.LeaderNameAndEmail = string.Empty;
                TeamDetails.MemberNamesAndEmails = new List<string>();
            }
            if (Initialized)
            {
                await JsInterop("resetCreateTeamLeader");
                await JsInterop("resetCreateTeamMember");
            }
        }
        private async Task Initialize()
        {
            Initialized = false;
            await AuthState.GetUserPermissions(AuthState, ApiConn);
            ResetErrorMessage();
            await ResetInputFields();
            if (HasTeamId)
                await InitializeManageTeam();
            else
                await InitializeCreateTeam();

            await InvokeAsync(StateHasChanged);
        }
        private async Task InitializeCreateTeam()
        {
            await JsInterop("setPageTitle", "Create Team");
            HasAccess = AuthState.IsAdmin;
            if (HasAccess)
            {
                await GetEligibleUsers();
                TeamDetails = new EctTeamRequestDetails
                {
                    MemberNamesAndEmails = new List<string>()
                };
            }
            Initialized = true;
        }
        private async Task InitializeManageTeam()
        {
            await JsInterop("setPageTitle", "Manage Team");
            TeamDetails = await ApiConn.IsProcessingUserLeaderForTeam(HashedTeamId);

            if (TeamDetails == null) HasAccess = false;
            else HasAccess = true;

            if (HasAccess)
            {
                await JsInterop("setPageTitle", TeamDetails.Name);
                await GetEligibleUsers();

                Initialized = true;                                                                         // Reload the componet to reveal the input field to let JavaScript populate it
                StateHasChanged();
                await JsInterop("setCreateTeamLeader", TeamDetails.LeaderNameAndEmail);
            }
            Initialized = true;
        }
        private bool IsCurrentMemberSelectionEmpty()
        {
            if (string.IsNullOrWhiteSpace(CurrentMemberSelection))
            {
                MemberInputError = true;
                serverMessageIsError = true;
                ServerMessage = "Please, select a member first.";
                return true;
            }
            return false;
        }
        private bool IsCurrentLeaderSelectionBadlyFormatted(out string leaderEmail)
        {
            bool result = IsInputBadlyFormatted(TeamDetails.LeaderNameAndEmail, out leaderEmail);
            if (result)
                LeaderInputError = true;

            return result;
        }
        private bool IsCurrentMemberSelectionBadlyFormatted(out string selectedEmail)
        {
            bool result = IsInputBadlyFormatted(CurrentMemberSelection, out selectedEmail);
            if(result)
                MemberInputError = true;
            
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
            if (string.IsNullOrWhiteSpace(TeamDetails.LeaderNameAndEmail))
                return false;

            if (TeamDetails.LeaderNameAndEmail.Contains(selectedEmail))
            {
                MemberInputError = true;
                serverMessageIsError = true;
                ServerMessage = "User already selected as team lead.";
                return true;
            }
            return false;
        }
        private bool IsCurrentMemberSelectionAlreadySelected(string selectedEmail)
        {
            if (TeamDetails.MemberEmails.Contains(selectedEmail))
            {
                MemberInputError = true;
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
                LeaderInputError = true;

            return result;
        }
        private bool IsCurrentMemberSelectionIneligible(string selectedEmail)
        {
            bool result = IsEmailIneligibleForSelection(selectedEmail, AvailableMembers);
            if(result)
                MemberInputError = true;

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
