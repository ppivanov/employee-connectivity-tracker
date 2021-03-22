using BlazorDateRangePicker;
using EctBlazorApp.Client.Graph;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Client.Pages.Dashboards
{
    public abstract class DashboardClass : ComponentBase
    {
        [Inject]
        protected IControllerConnection ApiConn { get; set; }
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        protected readonly Dictionary<string, double> CollaboratorsDict = new Dictionary<string, double>();

        protected IEnumerable<KeyValuePair<string, double>> Collaborators
        {
            get
            {
                /* Adapted from: https://stackoverflow.com/a/298 */
                List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>();
                foreach (var key in CollaboratorsDict.Keys)
                {
                    double percentage = CollaboratorsDict[key] / TotalPoints * 100;
                    double percentageToAdd = CollaboratorsDict[key] > 0 ? percentage : 0;
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
        protected CommunicationPoint EmailCommPoints { get; set; }
        protected string FormattedTimeInMeeting => FormatSecondsToHoursAndMinutes(SecondsInMeeting);
        protected DateTimeOffset? FromDate { get; set; } = DateTimeOffset.Now; 
        protected bool Initialized { get; set; } = false;
        protected CommunicationPoint MeetingCommPoints { get; set; }
        protected int NumberOfMeetings { get; set; } = 0;
        protected double SecondsInMeeting { get; set; } = 0;
        protected DateTimeOffset? ToDate { get; set; } = DateTimeOffset.Now.AddDays(1);
        protected int TotalMinutesInMeetings => GetMinutesFromSeconds(SecondsInMeeting);
        protected int TotalPoints
        {
            get
            {
                double totalMeetingPoints = TotalMinutesInMeetings / 10.0 * MeetingCommPoints.Points;
                int totalEmailPoints = (TotalEmailsCount) * EmailCommPoints.Points;
                return (int)(totalEmailPoints + totalMeetingPoints);
            }
        }

        protected void AddPointsToCollaborators(string fullName, double points)
        {
            if (CollaboratorsDict.ContainsKey(fullName))
                CollaboratorsDict[fullName] += points;
            else
                CollaboratorsDict.Add(fullName, points);
        }

        protected async Task CustomApply(MouseEventArgs e, DateRangePicker picker)
        {
            await picker.ClickApply(e);

            await UpdateDashboard();
        }

        protected async Task FetchCommunicationPoints()
        {
            var result = await ApiConn.FetchCommunicationPoints();

            EmailCommPoints = result.Item1;
            MeetingCommPoints = result.Item2;
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

        protected abstract int TotalEmailsCount { get; }
        protected abstract object[][] GetCalendarEventsData();
        protected abstract object[][] GetEmailData();
        protected abstract Task UpdateDashboard();
        protected abstract Task FindCollaborators();
    }
}
