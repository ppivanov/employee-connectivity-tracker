using BlazorDateRangePicker;
using EctBlazorApp.Client.Graph;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Client.Pages.DashboardClasses
{
    public abstract class DashboardClass : ComponentBase
    {
        [Inject]
        protected IControllerConnection ApiConn { get; set; }

        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        protected bool initialized = false;
        protected int numberOfMeetings = 0;
        protected double secondsInMeeting = 0;
        protected CommunicationPoint emailCommPoints;
        protected CommunicationPoint meetingCommPoints;
        protected readonly Dictionary<string, double> collaboratorsDict = new Dictionary<string, double>();

        protected DateTimeOffset? FromDate { get; set; } = DateTimeOffset.Now; 
        protected DateTimeOffset? ToDate { get; set; } = DateTimeOffset.Now.AddDays(1);
        protected string FormattedTimeInMeeting
        {
            get
            {
                return FormatSecondsToHoursAndMinutes(secondsInMeeting);
            }
        }
        protected int TotalMinutesInMeetings
        {
            get
            {
                return GetMinutesFromSeconds(secondsInMeeting);
            }
        }
        protected int TotalPoints
        {
            get
            {
                double totalMeetingPoints = TotalMinutesInMeetings / 10.0 * meetingCommPoints.Points;
                int totalEmailPoints = (TotalEmailsCount) * emailCommPoints.Points;
                return (int)(totalEmailPoints + totalMeetingPoints);
            }
        }
        protected IEnumerable<KeyValuePair<string, double>> Collaborators
        {
            get
            {
                /* Adapted from: https://stackoverflow.com/a/298 */
                List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>();
                foreach (var key in collaboratorsDict.Keys)
                {
                    double percentage = collaboratorsDict[key] / TotalPoints * 100;
                    double percentageToAdd = collaboratorsDict[key] > 0 ? percentage : 0;
                    list.Add(new KeyValuePair<string, double>(key, percentageToAdd));
                }

                list.Sort(
                    (KeyValuePair<string, double> pair1, KeyValuePair<string, double> pair2) =>
                    {
                        return pair2.Value.CompareTo(pair1.Value);                              // descending order
                    }
                );
                return list.Take(10);
            }
        }

        protected async Task CustomApply(MouseEventArgs e, DateRangePicker picker)
        {
            await picker.ClickApply(e);

            await UpdateDashboard();
        }
        protected async Task ResetClick(MouseEventArgs e, DateRangePicker picker)
        {
            FromDate = DateTimeOffset.Now;
            ToDate = DateTimeOffset.Now.AddDays(1);
            // Close the picker
            await picker.Close();
            // Fire OnRangeSelectEvent
            await picker.OnRangeSelect.InvokeAsync(new DateRange());

            await UpdateDashboard();
        }

        protected async Task FetchCommunicationPoints()
        {
            var result = await ApiConn.FetchCommunicationPoints();

            emailCommPoints = result.Item1;
            meetingCommPoints = result.Item2;
        }

        protected void AddPointsToCollaborators(string fullName, double points)
        {
            if (collaboratorsDict.ContainsKey(fullName))
                collaboratorsDict[fullName] += points;
            else
                collaboratorsDict.Add(fullName, points);
        }

        protected abstract int TotalEmailsCount { get; }
        protected abstract object[][] GetCalendarEventsData();
        protected abstract object[][] GetEmailData();
        protected abstract Task UpdateDashboard();
        protected abstract Task FindCollaborators();
    }
}
