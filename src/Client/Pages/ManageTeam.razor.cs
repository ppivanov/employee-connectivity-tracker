using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Shared;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Client.Pages
{
    public class ManageTeamClass : ComponentBase
    {
        [Parameter]
        public string HashedTeamId { get; set; }
        [Inject]
        protected CustomAuthState AuthState { get; set; }
        [Inject]
        protected IControllerConnection ApiConn { get; set; }
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }


        public ManageTeamClass() : base() { }

        public ManageTeamClass(EctTeamRequestDetails teamDetails)
        {
            TeamDetails = teamDetails;
        }

        // public properties for unit tests
        public bool AddNotifyUserInputError { get; private set; } = false;
        public NotificationOptionsResponse NewNotificationOptions => TeamDetails.NewNotificationOptions;
        public bool ServerMessageIsError { get; private set; } = false;
        public string ServerMessage { get; private set; } = string.Empty;
        public EctTeamRequestDetails TeamDetails { get; set; }
        public string UserToNotify_Email { get; set; } = string.Empty;
        public string UserToNotify_Name { get; set; } = string.Empty;

        private List<string> allAvailableLeaders;
        private bool leaderInputError = false;
        private bool marginInputError = false;
        private List<string> originalMembers;
        private List<string> originalAvailableLeaders;
        private bool pointsInputError = false;
        private bool isUserToNotifyMember = false;

        protected string AddNotifyUserInputStyle
        {
            get => AddNotifyUserInputError ? "border: 1px solid red" : string.Empty;
        }
        protected List<string> AvailableUsers
        {
            get
            {
                var selectableUsers = allAvailableLeaders.ToList();                                   // Copy the available set of members
                selectableUsers.Remove(TeamDetails.LeaderNameAndEmail);                                      // Remove the selected leader
                return selectableUsers;
            }
        }
        protected string GetUserRemovedFromMembersClass(string user)
        {
            if (originalMembers != null && originalMembers.Contains(user))
                return "removed-member";
            else
                return "";
        }
        protected string FilterUsersInput { get; private set; } = string.Empty;
        protected bool HasAccess { get; private set; } = false;
        protected bool HasTeamId => string.IsNullOrEmpty(HashedTeamId) == false;
        protected bool Initialized { get; private set; } = false;
        protected bool IsSubmitting { get; private set; } = false;
        protected string LeaderInputStyle => leaderInputError ? "border: 1px solid red" : string.Empty;
        protected string MarginInputStyle => marginInputError ? "border: 1px solid red" : string.Empty;
        protected string PointsInputStyle => pointsInputError ? "border: 1px solid red" : string.Empty;
        protected IEnumerable<string> PromptUsersForNotification =>
            TeamDetails.MemberNamesAndEmails.Where(
                    member => NewNotificationOptions.UsersToNotify.Contains(member) == false);

        public async Task AddUserToNotify()
        {
            if (UserToNotifyFieldsAreEmpty() || UserToNotifyAlreadyInList() || UserToNotifyValidEmail())
                return;

            string nameAndEmail = FormatFullNameAndEmail(UserToNotify_Name, UserToNotify_Email);
            TeamDetails.NewNotificationOptions.UsersToNotify.Add(nameAndEmail);
            UserToNotify_Name = string.Empty;
            UserToNotify_Email = string.Empty;
            await JsInterop("resetUserToNotifyEmail");
            await JsInterop("resetUserToNotifyName");
        }

        public virtual async Task JsInterop(string function, string parameter = "")
        {
            await JsRuntime.InvokeVoidAsync(function, parameter);
        }                                  // Used to mock JavaScript function calls in unit tests

        public void RemoveUserToNotify(string toRemove)
        {
            TeamDetails.NewNotificationOptions.UsersToNotify.Remove(toRemove);
        }

        public void SetLeaderNameEmail(ChangeEventArgs args)
        {
            ResetErrorMessage();
            TeamDetails.LeaderNameAndEmail = args.Value.ToString();
        }

        public async Task SetUserToNotifyEmail(ChangeEventArgs args)
        {
            ResetErrorMessage();
            UserToNotify_Email = args.Value.ToString();
            var matchingMember = PromptUsersForNotification.FirstOrDefault(m => GetEmailFromFormattedString(m).Equals(UserToNotify_Email));
            if (matchingMember != null)
            {
                isUserToNotifyMember = true;
                UserToNotify_Name = GetFullNameFromFormattedString(matchingMember);
                await JsInterop("setUserToNotifyName", UserToNotify_Name);
            }
            else if(isUserToNotifyMember)
            {
                isUserToNotifyMember = false;
                UserToNotify_Name = string.Empty;
                await JsInterop("resetUserToNotifyName");                                                               // if a user has entered the Email of a member, and then changes it, clear the FullName field
            }
        }

        public async Task SetUserToNotifyName(ChangeEventArgs args)
        {
            ResetErrorMessage();
            UserToNotify_Name = args.Value.ToString();
            var matchingMember = PromptUsersForNotification.FirstOrDefault(m => GetFullNameFromFormattedString(m).Equals(UserToNotify_Name));
            if (matchingMember != null)
            {
                isUserToNotifyMember = true;
                UserToNotify_Email = GetEmailFromFormattedString(matchingMember);
                await JsInterop("setUserToNotifyEmail", UserToNotify_Email);
            }
            else if (isUserToNotifyMember)
            {
                isUserToNotifyMember = false;
                UserToNotify_Email = string.Empty;
                await JsInterop("resetUserToNotifyEmail");                                                              // if a user has entered the FullName of a member, and then changes it, clear the Email field
            }
        }


        protected async Task AddSelectedMember(string memberToAdd)
        {
            TeamDetails.MemberNamesAndEmails.Add(memberToAdd);
            allAvailableLeaders = allAvailableLeaders.Where(a => a.Contains(memberToAdd) == false).ToList();            // this user can no longer be selected as leader

            await InvokeAsync(StateHasChanged);
        }

        protected List<string> GetAvailableLeaders() => allAvailableLeaders.ToList();

        protected async Task GetEligibleUsers()
        {
            var response = await ApiConn.GetUsersEligibleForMembers();
            allAvailableLeaders = response.ToList();
            originalAvailableLeaders = allAvailableLeaders.ToList();
        }

        protected override async Task OnParametersSetAsync()
        {
            await Initialize();
        }

        protected async Task RemoveFromSelected(string member)
        {
            TeamDetails.MemberNamesAndEmails.Remove(member);
            allAvailableLeaders.Add(member);                                                                            // this user can now be selected as leader
            await InvokeAsync(StateHasChanged);
        }

        protected void ResetTeamDetails()
        {
            ResetErrorMessage();
            allAvailableLeaders = originalAvailableLeaders.ToList();
            TeamDetails.NewNotificationOptions = new NotificationOptionsResponse(TeamDetails.CurrentNotificationOptions);
            TeamDetails.MemberNamesAndEmails = originalMembers.ToList();
        }

        protected async Task SendTeamData()
        {
            ResetErrorMessage();
            if (IsSubmitting ||
                AreTeamDetailsValid() == false ||
                string.IsNullOrWhiteSpace(TeamDetails.LeaderNameAndEmail) ||
                IsLeaderAlreadySelectedAsMember() ||
                IsCurrentLeaderSelectionBadlyFormatted() ||
                IsCurrentLeaderSelectionIneligible() ||
                AreNotificationOptionsValid() == false
                )
                return;

            IsSubmitting = true;
            TeamDetails.TeamId = HasTeamId ? HashedTeamId : string.Empty;
            bool isNewTeam = HasTeamId == false;

            var response = await ApiConn.SubmitTeamData(isNewTeam, TeamDetails);
            ServerMessageIsError = response.Item1 == false;                                     // is StatusCode of response successful?
            ServerMessage = response.Item2;
            leaderInputError = false;

            if (isNewTeam && ServerMessageIsError == false)
            {
                allAvailableLeaders.Remove(TeamDetails.LeaderNameAndEmail);
                await ResetInputFields();
            }
            else
            {
                originalAvailableLeaders = allAvailableLeaders.ToList();
                originalMembers = TeamDetails.MemberNamesAndEmails.ToList();
                TeamDetails.CurrentNotificationOptions = new NotificationOptionsResponse(NewNotificationOptions);
            }
            IsSubmitting = false;
        }

        protected void UpdateFilterUsersInput(ChangeEventArgs args)
        {
            FilterUsersInput = args.Value.ToString();
        }


        private bool AreNotificationOptionsValid()
        {
            if (HasTeamId == false) return true;

            int newPoints = NewNotificationOptions.PointsThreshold;
            int minPoints = NotificationOptionsResponse.MinPoints;
            int maxPoints = NotificationOptionsResponse.MaxPoints;

            double newMargin = NewNotificationOptions.MarginForNotification;
            int minMargin = NotificationOptionsResponse.MinMargin;
            int maxMargin = NotificationOptionsResponse.MaxMargin;

            if (newPoints < minPoints || newPoints > maxPoints)
            {
                ServerMessageIsError = true;
                ServerMessage = NotificationOptionsResponse.PointsErrorMessage;
                pointsInputError = true;
                return false;
            }

            if (newMargin < minMargin || newMargin > maxMargin)
            {
                ServerMessageIsError = true;
                ServerMessage = NotificationOptionsResponse.MarginErrorMessage;
                marginInputError = true;
                return false;
            }
            return true;
        }
        private bool AreTeamDetailsValid()
        {
            if (TeamDetails.AreDetailsValid())
                return true;

            ServerMessageIsError = true;
            ServerMessage = "Bad request. Please, review inputs and resubmit.";
            IsSubmitting = false;
            return false;
        }
        private void ResetErrorMessage()
        {
            leaderInputError = false;
            pointsInputError = false;
            marginInputError = false;
            AddNotifyUserInputError = false;
            ServerMessageIsError = false;
            ServerMessage = string.Empty;
        }
        private async Task ResetInputFields()
        {
            if (TeamDetails != null)
            {
                TeamDetails.Name = string.Empty;
                TeamDetails.LeaderNameAndEmail = string.Empty;
                TeamDetails.MemberNamesAndEmails = new List<string>();
            }
            if (Initialized)
            {
                await JsInterop("resetCreateTeamLeader");
            }
        }
        private async Task Initialize()
        {
            Initialized = false;
            await CustomAuthState.GetUserPermissions(AuthState, ApiConn);
            ResetErrorMessage();
            await ResetInputFields();
            if (HasTeamId)
                await InitializeManageTeam();
            else
                await InitializeCreateTeam();
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

            if (TeamDetails == null)
                HasAccess = false;
            else
                HasAccess = true;

            if (HasAccess)
            {
                originalMembers = TeamDetails.MemberNamesAndEmails.ToList();
                await JsInterop("setPageTitle", TeamDetails.Name);
                await GetEligibleUsers();

                Initialized = true;                                                                         // Reload the componet to reveal the input field to let JavaScript populate it
                StateHasChanged();
                await JsInterop("setCreateTeamLeader", TeamDetails.LeaderNameAndEmail);
            }
            Initialized = true;
        }
        private bool IsCurrentLeaderSelectionBadlyFormatted()
        {
            bool result = IsInputBadlyFormatted(TeamDetails.LeaderNameAndEmail);
            if (result)
                leaderInputError = true;

            return result;
        }
        private bool IsCurrentLeaderSelectionIneligible()
        {
            if (HasTeamId) return false;

            bool result = IsUserIneligibleForSelection(TeamDetails.LeaderNameAndEmail, allAvailableLeaders);
            if (result)
                leaderInputError = true;

            return result;
        }
        private bool IsUserIneligibleForSelection(string user, IEnumerable<string> list)
        {
            if (list.Any(a => a.Contains(user)) == false)
            {
                ServerMessageIsError = true;
                ServerMessage = "This user is not eligible for selection.";
                return true;
            }
            return false;
        }
        private bool IsInputBadlyFormatted(string input)
        {
            if (IsStringInMemberFormat(input) == false)
            {
                ServerMessageIsError = true;
                ServerMessage = "Please, enter the details in the reqired format.";
                return true;
            }
            return false;
        }
        private bool IsLeaderAlreadySelectedAsMember()
        {
            if (TeamDetails.MemberEmails.Contains(TeamDetails.LeaderEmail) == false)
                return false;

            ServerMessageIsError = true;
            leaderInputError = true;
            ServerMessage = "Team lead is already in member list.";
            IsSubmitting = false;
            return true;
        }
        private void SetNotifyUserInputErrorMessage(string message)
        {
            AddNotifyUserInputError = true;
            ServerMessageIsError = true;
            ServerMessage = message;
        }
        private bool UserToNotifyFieldsAreEmpty()
        {
            if (string.IsNullOrWhiteSpace(UserToNotify_Name) ||
                string.IsNullOrWhiteSpace(UserToNotify_Email))
            {
                SetNotifyUserInputErrorMessage("You must provide values for both Name and Email fields.");
                return true;
            }
            return false;
        }
        private bool UserToNotifyAlreadyInList()
        {
            if (TeamDetails.NewNotificationOptions.UsersToNotify.Any(utn => utn.Contains($"<{UserToNotify_Email}>")))
            {
                SetNotifyUserInputErrorMessage("The entered email is already in the list.");
                return true;
            }
            return false;
        }
        private bool UserToNotifyValidEmail()
        {
            if (UserToNotify_Email.IsValidEmail() == false)
            {
                SetNotifyUserInputErrorMessage("Please, enter a valid email.");
                return true;
            }
            return false;
        }
    }
}
