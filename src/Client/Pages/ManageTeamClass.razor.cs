using EctBlazorApp.Client.Graph;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Client.Pages
{
    public class ManageTeamClass : ComponentBase
    {
        [Inject]
        protected IControllerConnection ApiConn { get; set; }
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }
        [Inject]
        protected HttpClient Http { get; set; }

        private bool serverMessageIsError = false;
        
        protected string CurrentMemberSelection { get; set; }
        protected string InputStyle { get => inputError ? "border: 1px solid red" : ""; }
        protected string ServerMessage { get; set; } = "";
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
        HashSet<string> MembersFromApi { get; set; }
        protected HashSet<string> AvailableLeaders { get; set; }
        protected HashSet<string> AvailableMembers
        {
            get
            {
                var selectableMembers = AvailableLeaders.ToHashSet();                                   // Copy the available set of members
                selectableMembers.Remove(teamDetails.LeaderNameAndEmail);                                      // Remove the selected leader
                return selectableMembers;
            }
        }

        protected bool isAdmin = false;
        protected bool isSubmitting = false;
        protected bool initialized = false;
        protected bool inputError = false;
        protected EctTeamRequestDetails teamDetails = new EctTeamRequestDetails();

        public async Task SetLeaderNameEmail(ChangeEventArgs args)
        {
            teamDetails.LeaderNameAndEmail = args.Value.ToString();
            // if the email has already been added to the list of members -> display error
        }

        public async Task SetMemberNameEmail(ChangeEventArgs args)
        {
            CurrentMemberSelection = args.Value.ToString();
            //AvailableLeaders.Remove(CurrentMemberSelection);
            // if the email has already been added to the list of members -> display error
        }

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Create Team");
            isAdmin = await ApiConn.IsProcessingUserAnAdmin();
            if (isAdmin)
            {
                await GetEligibleUsers();
                teamDetails.MemberNamesAndEmails = new List<string>();
            }
            initialized = true;
        }

        protected async Task AddSelectedMember()
        {
            if (IsCurrentMemberSelectionEmpty()
                || IsCurrentMemberSelectionFormatted()
                || IsCurrentMemberSelectionAlreadyLeader()
                || IsCurrentMemberSelectionAlreadySelected())
                return;

            teamDetails.MemberNamesAndEmails.Add(CurrentMemberSelection);
            AvailableLeaders.Remove(CurrentMemberSelection);
            CurrentMemberSelection = "";
            await InvokeAsync(StateHasChanged);
        }

        protected async Task GetEligibleUsers()
        {
            var response = await ApiConn.GetUsersEligibleForMembers();
            MembersFromApi = response.ToHashSet();
            AvailableLeaders = MembersFromApi.ToHashSet();                                      // Copy the set not the reference
        }

        protected async Task RemoveFromSelected(string member)
        {
            teamDetails.MemberNamesAndEmails.Remove(member);
            AvailableLeaders.Add(member);
            await InvokeAsync(StateHasChanged);
        }

        protected async Task SendTeamData()
        {
            if (isSubmitting)
                return;

            isSubmitting = true;

            var response = await ApiConn.SubmitTeamData(teamDetails);
            serverMessageIsError = response.Item1 == false;                                     // is StatusCode of response successful?
            ServerMessage = response.Item2;

            ResetInputFields();
            isSubmitting = false;
        }


        private void ResetInputFields()
        {
            teamDetails.Name = "";
            teamDetails.LeaderNameAndEmail = "";
            teamDetails.MemberNamesAndEmails = new List<string>();
        }

        private bool IsCurrentMemberSelectionEmpty()
        {
            if (string.IsNullOrEmpty(CurrentMemberSelection))
            {
                inputError = true;
                serverMessageIsError = true;
                ServerMessage = "Please, select a member first.";
                return true;
            }
            return false;
        }

        private bool IsCurrentMemberSelectionFormatted()
        {
            if (IsStringInMemberFormat(CurrentMemberSelection) == false)
            {
                inputError = true;
                serverMessageIsError = true;
                ServerMessage = "Please, enter the details in the reqired format.";
                return true;
            }
            return false;
        }

        private bool IsCurrentMemberSelectionAlreadyLeader()
        {
            if (CurrentMemberSelection.Equals(teamDetails.LeaderNameAndEmail))
            {
                inputError = true;
                serverMessageIsError = true;
                ServerMessage = "User already selected as team lead.";
                return true;
            }
            return false;
        }

        private bool IsCurrentMemberSelectionAlreadySelected()
        {
            if (teamDetails.MemberNamesAndEmails.Contains(CurrentMemberSelection))
            {
                inputError = true;
                serverMessageIsError = true;
                ServerMessage = "This user has already beeen selected.";
                return true;
            }
            return false;
        }
    }
}
