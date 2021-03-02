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
        protected NavigationManager NavManager { get; set; }

        private bool inputError = false;
        private bool serverMessageIsError = false;
        private List<EctUser> administrators;

        protected bool allowsEdit = false;
        protected bool isLeader = false;
        protected bool isSubmitting = false;
        protected int emailsSent = 0;
        protected int emailsReceived = 0;
        protected string leaderNameAndEmail = "";
        protected string newUserToNotify_Email = "";
        protected string newUserToNotify_Name = "";
        protected string serverMessage = "";
        protected List<EctUser> teamMembers;
        protected NotificationOptionsResponse currentNotificationOptions = null;
        protected NotificationOptionsResponse newNotificationOptions = null;
        

        protected List<EctUser> AvailableUsersForNotification
        {
            get
            {
                var users = teamMembers.ToList();
                users.AddRange(administrators);
                users = users.GroupBy(u => u.Email).Select(u => u.First()).ToList();
                foreach (var selectedUser in newNotificationOptions.UsersToNotify)
                {
                    var duplicate = users.FirstOrDefault(u => u.Email.Equals(GetEmailFromFormattedString(selectedUser)));
                    if (duplicate != null)
                        users.Remove(duplicate);
                }
                return users;
            }
        }
        protected override int TotalEmailsCount
        {
            get
            {
                return emailsSent + emailsReceived;
            }
        }
        protected string InputStyle
        {
            get
            {
                return inputError ? "border: 1px solid red" : "";
            }
        }
        protected string ServerMessageInlineStyle
        {
            get
            {
                return serverMessageIsError ? "color: red;" : "color: green;";
            }
        }

        protected void AddUserToNotify()
        {
            string nameAndEmail = FormatFullNameAndEmail(newUserToNotify_Name, newUserToNotify_Email);
            newNotificationOptions.UsersToNotify.Add(nameAndEmail);
            newUserToNotify_Name = "";
            newUserToNotify_Email = "";
        }
        
        protected void CancelEditNotificationOptions()
        {
            allowsEdit = false;
            newNotificationOptions.PointsThreshold = currentNotificationOptions.PointsThreshold;
            newNotificationOptions.MarginForNotification = currentNotificationOptions.MarginForNotification;
            newNotificationOptions.UsersToNotify = currentNotificationOptions.UsersToNotify.ToList();
        }
        
        protected void EditNotificationOptions()
        {
            allowsEdit = true;
        }
        
        protected override Task FindCollaborators()
        {
            // Meeting collaborators added to dictionary in GetCalendarEventData to avoid looping over the user list again

            foreach (var member in teamMembers)
            {
                int totalEmails = member.SentEmails.Count + member.ReceivedEmails.Count;
                double pointsToAdd = totalEmails * emailCommPoints.Points;
                AddPointsToCollaborators(member.FullName, pointsToAdd);
            }

            return Task.CompletedTask;
        }
       
        protected override object[][] GetCalendarEventsData()
        {
            Dictionary<string, HashSet<string>> eventsBySubject = new Dictionary<string, HashSet<string>>();

            foreach (var member in teamMembers)
            {
                int numberOfMeetingsBefore = numberOfMeetings;
                foreach (var calendarEvent in member.CalendarEvents)
                {
                    string eventDateTimeRange = $"{calendarEvent.Start}-{calendarEvent.End}";
                    if (eventsBySubject.ContainsKey(calendarEvent.Subject))
                    {
                        if (eventsBySubject[calendarEvent.Subject].Contains(eventDateTimeRange) == false)                                               // only add if the specific meeting at the specific time has not been added
                        {
                            eventsBySubject[calendarEvent.Subject].Add(eventDateTimeRange);
                            secondsInMeeting += GetSecondsFromDateTimeRange(calendarEvent.Start, calendarEvent.End);
                            numberOfMeetings++;
                        }
                    }
                    else
                    {
                        eventsBySubject.Add(calendarEvent.Subject, new HashSet<string>() { { eventDateTimeRange } });                                   // if none of the meetings so far have had the subject, add a new one and initialize a set for the times
                        secondsInMeeting += GetSecondsFromDateTimeRange(calendarEvent.Start, calendarEvent.End);
                        numberOfMeetings++;
                    }
                }
                double pointsToAdd = (numberOfMeetings - numberOfMeetingsBefore) * meetingCommPoints.Points;
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

                foreach (var member in teamMembers)
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
                emailsReceived += totalReceivedOnDate;
                emailsSent += totalSentOnDate;
            }
            return newList;
        }
        
        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "My Team");
            isLeader = await ApiConn.IsProcessingUserALeader();
            if (isLeader)
            {
                currentNotificationOptions = await ApiConn.FetchCurrentNotificationOptions();
                newNotificationOptions = new NotificationOptionsResponse(currentNotificationOptions);
                administrators = await ApiConn.FetchAdminstrators();
                await FetchCommunicationPoints();
                await UpdateDashboard();
            }
        }
        
        protected void RedirectToDasboard(string userFullName)
        {
            int userId = teamMembers.First(u => u.FullName.Equals(userFullName)).Id;
            string hasedUserId = ComputeSha256Hash(userId.ToString());
            NavManager.NavigateTo($"/dashboard/{hasedUserId}");
        }
       
        protected void RemoveUserToNotify(string toRemove)
        {
            newNotificationOptions.UsersToNotify.Remove(toRemove);
        }
        
        protected async Task SubmitNotificationOptions()
        {
            if (newNotificationOptions.PointsThreshold < 0)
            {
                newNotificationOptions.PointsThreshold = 0;
                serverMessageIsError = true;
                inputError = true;
                serverMessage = "You cannot set the threshold below 0.";
                return;
            }
            isSubmitting = true;
            serverMessageIsError = false;

            var response = await ApiConn.SubmitNotificationOptions(newNotificationOptions);
            serverMessageIsError = response.Item1;
            serverMessage = response.Item2;
            inputError = serverMessageIsError;

            isSubmitting = false;
            allowsEdit = false;
            if (serverMessageIsError == false) currentNotificationOptions = newNotificationOptions;
        }
        
        protected async Task SetUserToNotifyEmail(ChangeEventArgs args)
        {
            newUserToNotify_Email = args.Value.ToString();
            var matchingMember = teamMembers.FirstOrDefault(m => m.Email.Equals(newUserToNotify_Email));
            if (matchingMember != null)
            {
                await JsRuntime.InvokeVoidAsync("setUserToNotifyName", matchingMember.FullName);
                newUserToNotify_Name = matchingMember.FullName;
            }
        }
       
        protected async Task SetUserToNotifyName(ChangeEventArgs args)
        {
            newUserToNotify_Name = args.Value.ToString();
            var matchingMember = teamMembers.FirstOrDefault(m => m.FullName.Equals(newUserToNotify_Name));
            if(matchingMember != null)
            {
                await JsRuntime.InvokeVoidAsync("setUserToNotifyEmail", matchingMember.Email);
                newUserToNotify_Email = matchingMember.Email;
            }
        }

        protected override async Task UpdateDashboard()
        {
            ResetAttributeValues();
            string queryString = GetDateRangeQueryString(FromDate.Value, ToDate.Value);

            var response = await ApiConn.FetchTeamDashboardResponse(queryString);
            teamMembers = response.TeamMembers;
            leaderNameAndEmail = response.LeaderNameAndEmail;

            await FindCollaborators();
            initialized = true;
            await InvokeAsync(StateHasChanged);                                                                                                                                     // Force a refresh of the component before trying to load the js graphs
            await JsRuntime.InvokeVoidAsync("setPageTitle", response.TeamName);
            await JsRuntime.InvokeVoidAsync("loadMyTeamDashboardGraph", (object)GetEmailData(), (object)GetCalendarEventsData());                                                   // GetCalendarEventsData is adding only some of the collaborators to the dictionary
        }


        private void ResetAttributeValues()
        {
            numberOfMeetings = 0;
            secondsInMeeting = 0;
            collaboratorsDict.Clear();

            serverMessageIsError = false;
            inputError = false;
            isSubmitting = false;
            teamMembers = null;
            emailsSent = 0;
            emailsReceived = 0;
            serverMessage = "";
            leaderNameAndEmail = "";
        }
    }
}
