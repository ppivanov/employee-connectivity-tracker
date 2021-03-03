using EctBlazorApp.Client.Graph;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

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

        protected EctTeamRequestDetails teamDetails = new EctTeamRequestDetails();
        protected string CurrentMemberSelection { get; set; }
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
        bool serverMessageIsError = false;

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
            if (string.IsNullOrEmpty(CurrentMemberSelection)
                || CurrentMemberSelection.Equals("-")
                || CurrentMemberSelection.Equals(teamDetails.LeaderNameAndEmail)
                || teamDetails.MemberNamesAndEmails.Contains(CurrentMemberSelection))
                    return;

            teamDetails.MemberNamesAndEmails.Add(CurrentMemberSelection);
            AvailableLeaders.Remove(CurrentMemberSelection);
            await InvokeAsync(StateHasChanged);
        }

        protected async Task GetEligibleUsers()
        {
            var response = await ApiConn.GetUsersEligibleForMembers();
            MembersFromApi = response.ToHashSet();
            AvailableLeaders = MembersFromApi.ToHashSet();                                     // Copy the set not the reference
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
    }
}
