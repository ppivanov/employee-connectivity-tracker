﻿using EctBlazorApp.Client.Graph;
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
        private HashSet<string> AllAvailableLeaders { get; set; }

        protected string CurrentMemberSelection { get; set; } = "";
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

        protected bool isAdmin = false;
        protected bool isSubmitting = false;
        protected bool initialized = false;
        protected bool inputError = false;
        protected EctTeamRequestDetails teamDetails = new EctTeamRequestDetails();

        public async Task SetLeaderNameEmail(ChangeEventArgs args)
        {
            ResetErrorMessage();
            teamDetails.LeaderNameAndEmail = args.Value.ToString();
            // if the email has already been added to the list of members -> display error
        }

        public async Task SetMemberNameEmail(ChangeEventArgs args)
        {
            ResetErrorMessage();
            CurrentMemberSelection = args.Value.ToString();
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

            string selectedEmail = GetEmailFromFormattedString(CurrentMemberSelection);
            teamDetails.MemberNamesAndEmails.Add(CurrentMemberSelection);
            AllAvailableLeaders = AllAvailableLeaders.Where(a => a.Contains(selectedEmail) == false).ToHashSet();
            CurrentMemberSelection = "";

            await InvokeAsync(StateHasChanged);
            await JsInterop("resetCreateTeamMember");
        }

        protected async Task GetEligibleUsers()
        {
            var response = await ApiConn.GetUsersEligibleForMembers();
            MembersFromApi = response.ToHashSet();
            AllAvailableLeaders = MembersFromApi.ToHashSet();                                       // Copy the set not the reference
        }

        public virtual async Task JsInterop(string function, string parameter = "")
        {
            await JsRuntime.InvokeVoidAsync(function, parameter);
        }               // Used to mock JavaScript function calls

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

            if(serverMessageIsError == false)
                await ResetInputFields();
            isSubmitting = false;   
        }


        private async Task ResetInputFields()
        {
            teamDetails.Name = "";
            teamDetails.LeaderNameAndEmail = "";
            teamDetails.MemberNamesAndEmails = new List<string>();

            await JsInterop("resetCreateTeamLeader");
            await JsInterop("resetCreateTeamMember");
        }

        private bool IsCurrentMemberSelectionEmpty()
        {
            if (string.IsNullOrWhiteSpace(CurrentMemberSelection))
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
            if (string.IsNullOrWhiteSpace(teamDetails.LeaderNameAndEmail))
                return false;

            string selectedEmail = GetEmailFromFormattedString(CurrentMemberSelection);
            if (teamDetails.LeaderNameAndEmail.Contains(selectedEmail))
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

        private void ResetErrorMessage()
        {
            inputError = false;
            serverMessageIsError = false;
            ServerMessage = "";
        }
    }
}
