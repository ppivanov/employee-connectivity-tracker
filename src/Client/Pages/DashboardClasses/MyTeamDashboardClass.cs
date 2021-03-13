using EctBlazorApp.Client.Shared;
using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Client.Pages.DashboardClasses
{
    public class MyTeamDashboardClass : DashboardClass
    {
        [Inject]
        protected CustomAuthState AuthState { get; set; }
        [Inject]
        protected NavigationManager NavManager { get; set; }

        private bool inputError = false;

        // public properties for unit tests
        public bool AddNotifyUserInputError { get; set; } = false;
        public bool ServerMessageIsError { get; set; } = false;
        public string UserToNotify_Email { get; set; } = string.Empty;
        public string UserToNotify_Name { get; set; } = string.Empty;
        public string ServerMessage { get; set; } = string.Empty;
        public List<EctUser> Administrators { get; set; }
        public List<EctUser> TeamMembers { get; set; }
        public NotificationOptionsResponse CurrentNotificationOptions { get; set; } = null;
        public NotificationOptionsResponse NewNotificationOptions { get; set; } = null;

        protected string AddNotifyUserInputStyle 
        {
            get => AddNotifyUserInputError ? "border: 1px solid red" : string.Empty; 
        }
        protected bool AllowsEdit { get; set; } = false;
        protected List<EctUser> AvailableUsersForNotification
        {
            get
            {
                var users = TeamMembers.ToList();
                users.AddRange(Administrators);
                users = users.GroupBy(u => u.Email).Select(u => u.FirstOrDefault()).ToList();
                foreach (var selectedUser in NewNotificationOptions.UsersToNotify)
                {
                    var duplicate = users.FirstOrDefault(u => u.Email.Equals(GetEmailFromFormattedString(selectedUser)));
                    if (duplicate != null)
                        users.Remove(duplicate);
                }
                return users;
            }
        }
        protected int EmailsSent { get; set; } = 0;
        protected int EmailsReceived { get; set; } = 0;
        protected string InputStyle => inputError ? "border: 1px solid red" : string.Empty;
        protected bool IsLeader => AuthState.IsLeader;
        protected bool IsSubmitting { get; set; } = false;
        protected string LeaderNameAndEmail { get; set; } = string.Empty;
        protected override int TotalEmailsCount => EmailsSent + EmailsReceived;
        protected string ServerMessageInlineStyle => ServerMessageIsError ? "color: red;" : "color: green;";

        public MyTeamDashboardClass() { }

        public MyTeamDashboardClass(                                                                // Used to initalize an instance of the class for unit tests
            List<EctUser> administrators,
            List<EctUser> teamMembers,
            NotificationOptionsResponse currentNotificationOptions,
            NotificationOptionsResponse newNotificationOptions
            )
        {
            Administrators = administrators;
            this.TeamMembers = teamMembers;
            this.CurrentNotificationOptions = currentNotificationOptions;
            this.NewNotificationOptions = newNotificationOptions;
        }

        public async Task AddUserToNotify()
        {
            if (UserToNotifyFieldsAreEmpty() || UserToNotifyAlreadyInList())
                return;
            
            string nameAndEmail = FormatFullNameAndEmail(UserToNotify_Name, UserToNotify_Email);
            NewNotificationOptions.UsersToNotify.Add(nameAndEmail);
            UserToNotify_Name = string.Empty;
            UserToNotify_Email = string.Empty;
            await JsInterop("resetUserToNotifyEmail");
            await JsInterop("resetUserToNotifyName");
        }

        public virtual async Task JsInterop(string function, string parameter = "")
        {
            await JsRuntime.InvokeVoidAsync(function, parameter);
        }               // Used to mock JavaScript function calls

        public void RemoveUserToNotify(string toRemove)
        {
            NewNotificationOptions.UsersToNotify.Remove(toRemove);
        }

        public async Task SetUserToNotifyEmail(ChangeEventArgs args)
        {
            UserToNotify_Email = args.Value.ToString();
            var matchingMember = AvailableUsersForNotification.FirstOrDefault(m => m.Email.Equals(UserToNotify_Email));
            if (matchingMember != null)
            {
                await JsInterop("setUserToNotifyName", matchingMember.FullName);
                UserToNotify_Name = matchingMember.FullName;
            }
            else
                await JsInterop("resetUserToNotifyName");
        }

        public async Task SetUserToNotifyName(ChangeEventArgs args)
        {
            UserToNotify_Name = args.Value.ToString();
            var matchingMember = AvailableUsersForNotification.FirstOrDefault(m => m.FullName.Equals(UserToNotify_Name));
            if (matchingMember != null)
            {
                await JsInterop("setUserToNotifyEmail", matchingMember.Email);
                UserToNotify_Email = matchingMember.Email;
            }
            else
                await JsInterop("resetUserToNotifyEmail");
        }


        protected void CancelEditNotificationOptions()
        {
            AllowsEdit = false;
            NewNotificationOptions.PointsThreshold = CurrentNotificationOptions.PointsThreshold;
            NewNotificationOptions.MarginForNotification = CurrentNotificationOptions.MarginForNotification;
            NewNotificationOptions.UsersToNotify = CurrentNotificationOptions.UsersToNotify.ToList();
        }

        protected void EditNotificationOptions()
        {
            AllowsEdit = true;
        }

        protected override Task FindCollaborators()
        {
            // Meeting collaborators added to dictionary in GetCalendarEventData to avoid looping over the user list again

            foreach (var member in TeamMembers)
            {
                int totalEmails = member.SentEmails.Count + member.ReceivedEmails.Count;
                double pointsToAdd = totalEmails * EmailCommPoints.Points;
                AddPointsToCollaborators(member.FullName, pointsToAdd);
            }

            return Task.CompletedTask;
        }

        protected override object[][] GetCalendarEventsData()
        {
            Dictionary<string, HashSet<string>> eventsBySubject = new Dictionary<string, HashSet<string>>();

            foreach (var member in TeamMembers)
            {
                int numberOfMeetingsBefore = NumberOfMeetings;
                foreach (var calendarEvent in member.CalendarEvents)
                {
                    string eventDateTimeRange = $"{calendarEvent.Start}-{calendarEvent.End}";
                    if (eventsBySubject.ContainsKey(calendarEvent.Subject))
                    {
                        if (eventsBySubject[calendarEvent.Subject].Contains(eventDateTimeRange) == false)                                               // only add if the specific meeting at the specific time has not been added
                        {
                            eventsBySubject[calendarEvent.Subject].Add(eventDateTimeRange);
                            SecondsInMeeting += GetSecondsFromDateTimeRange(calendarEvent.Start, calendarEvent.End);
                            NumberOfMeetings++;
                        }
                    }
                    else
                    {
                        eventsBySubject.Add(calendarEvent.Subject, new HashSet<string>() { { eventDateTimeRange } });                                   // if none of the meetings so far have had the subject, add a new one and initialize a set for the times
                        SecondsInMeeting += GetSecondsFromDateTimeRange(calendarEvent.Start, calendarEvent.End);
                        NumberOfMeetings++;
                    }
                }
                double pointsToAdd = (NumberOfMeetings - numberOfMeetingsBefore) * MeetingCommPoints.Points;
                AddPointsToCollaborators(member.FullName, pointsToAdd);
            }

            // loop over the dictionary and count the number of elements in the set
            object[][] newList = new object[eventsBySubject.Count][];
            int i = 0;
            foreach (KeyValuePair<string, HashSet<string>> dictionaryEntry in eventsBySubject)
            {
                int count = dictionaryEntry.Value.Count;
                newList[i++] = new object[] { dictionaryEntry.Key, count };
            }
            return newList;
        }

        protected override object[][] GetEmailData()
        {
            var dates = SplitDateRangeToChunks(FromDate.Value, ToDate.Value);
            object[][] newList = new object[dates.Count][];

            for (int i = 0; i < dates.Count; i++)
            {
                DateTime date = dates[i];
                int totalReceivedOnDate = 0;
                int totalSentOnDate = 0;
                string tooltipDate = date.ToString("dd MMM");
                StringBuilder sentMailTooltipText = new StringBuilder($"{tooltipDate}\n");
                StringBuilder receivedMailTooltipText = new StringBuilder($"{tooltipDate}\n");

                foreach (var member in TeamMembers)
                {
                    string memberFirstName = member.FullName.Split(" ")[0];
                    int countOfSentMail = member.SentEmails.Count(sm => sm.SentAt.Date == date);
                    int countOfReceivedMail = member.ReceivedEmails.Count(sm => sm.ReceivedAt.Date == date);

                    sentMailTooltipText.Append($"{memberFirstName}: {countOfSentMail}\n");
                    receivedMailTooltipText.Append($"{memberFirstName}: {countOfReceivedMail}\n");

                    totalSentOnDate += countOfSentMail;
                    totalReceivedOnDate += countOfReceivedMail;
                }

                newList[i] = new object[] { tooltipDate, totalSentOnDate,
                    sentMailTooltipText.ToString(), totalReceivedOnDate, receivedMailTooltipText.ToString() };
                EmailsReceived += totalReceivedOnDate;
                EmailsSent += totalSentOnDate;
            }
            return newList;
        }

        protected override async Task OnInitializedAsync()
        {
            await JsInterop("setPageTitle", "My Team");
            await CustomAuthState.GetUserPermissions(AuthState, ApiConn);
            if (IsLeader)
            {
                CurrentNotificationOptions = await ApiConn.FetchCurrentNotificationOptions();
                NewNotificationOptions = new NotificationOptionsResponse(CurrentNotificationOptions);
                Administrators = (await ApiConn.FetchAdminstrators()).ToList();
                await FetchCommunicationPoints();
                await UpdateDashboard();
            }
            Initialized = true;
            await InvokeAsync(StateHasChanged);
        }

        protected void RedirectToDasboard(string userFullName)
        {
            int userId = TeamMembers.FirstOrDefault(u => u.FullName.Equals(userFullName)).Id;
            string hasedUserId = ComputeSha256Hash(userId.ToString());
            NavManager.NavigateTo($"/dashboard/{hasedUserId}");
        }

        protected async Task SubmitNotificationOptions()
        {
            if (NewNotificationOptions.PointsThreshold < 0)
            {
                NewNotificationOptions.PointsThreshold = 0;
                ServerMessageIsError = true;
                inputError = true;
                ServerMessage = "You cannot set the threshold below 0.";
                return;
            }
            IsSubmitting = true;
            ServerMessageIsError = false;

            var response = await ApiConn.SubmitNotificationOptions(NewNotificationOptions);
            ServerMessageIsError = response.Item1 == false;
            ServerMessage = response.Item2;
            inputError = ServerMessageIsError;

            IsSubmitting = false;
            AllowsEdit = false;
            if (ServerMessageIsError == false) CurrentNotificationOptions = NewNotificationOptions;
        }

        protected override async Task UpdateDashboard()
        {
            ResetAttributeValues();
            string queryString = GetDateRangeQueryString(FromDate.Value, ToDate.Value);

            var response = await ApiConn.FetchTeamDashboardResponse(queryString);
            TeamMembers = response.TeamMembers;
            LeaderNameAndEmail = response.LeaderNameAndEmail;

            await FindCollaborators();
            Initialized = true;
            await InvokeAsync(StateHasChanged);                                                                                                                                     // Force a refresh of the component before trying to load the js graphs
            await JsInterop("setPageTitle", response.TeamName);
            await JsRuntime.InvokeVoidAsync("loadMyTeamDashboardGraph", (object)GetEmailData(), (object)GetCalendarEventsData());                                                   // GetCalendarEventsData is adding only some of the collaborators to the dictionary
        }


        private void ResetAttributeValues()
        {
            NumberOfMeetings = 0;
            SecondsInMeeting = 0;
            CollaboratorsDict.Clear();

            ServerMessageIsError = false;
            inputError = false;
            IsSubmitting = false;
            TeamMembers = null;
            EmailsSent = 0;
            EmailsReceived = 0;
            ServerMessage = string.Empty;
            LeaderNameAndEmail = string.Empty;
        }
        private void SetNotifyUserInputErrorMessage(string message)
        {
            AddNotifyUserInputError = true;
            ServerMessageIsError = true;
            ServerMessage = message;
        }
        private bool UserToNotifyFieldsAreEmpty()
        {
            if (String.IsNullOrWhiteSpace(UserToNotify_Name)
                || String.IsNullOrWhiteSpace(UserToNotify_Email))
            {
                SetNotifyUserInputErrorMessage("You must provide values for both Name and Email fields.");
                return true;
            }
            return false;
        }
        private bool UserToNotifyAlreadyInList()
        {
            if (NewNotificationOptions.UsersToNotify.Any(utn => utn.Contains($"<{UserToNotify_Email}>")))
            {
                SetNotifyUserInputErrorMessage("The entered email is already in the list.");
                return true;
            }
            return false;
        }
    }
}
