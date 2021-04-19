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
                /***************************************************************************************
	            *    Usage: Used
	            *    Title: How do you sort a dictionary by value?
	            *    Author: Leon Bambrick [StackOverflow] (edited by Peter Mortensen [StackOverflow])
	            *	 Date posted: 2 August 2008
	            *	 Type: Source code
	            *    Availability: https://stackoverflow.com/a/298
	            *    Accessed on: 20 April 2021
	            *
	            ***************************************************************************************/

                List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>();
                foreach (var key in CollaboratorsDict.Keys)
                {
                    double percentage = CollaboratorsDict[key] / TotalPoints * 100;
                    double percentageToAdd = CollaboratorsDict[key] > 0 ? percentage : 0;
                    list.Add(new KeyValuePair<string, double>(key, percentageToAdd));
                }

                list.Sort(                                                                                              // sort the dictionaly in descending order
                    (KeyValuePair<string, double> pair1, KeyValuePair<string, double> pair2) => 
                        pair2.Value.CompareTo(pair1.Value)                              
                );
                return list.Take(10);                                                                                   // return only the first 10 entries
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
            
            await picker.Close();                                                                                       // Close the picker

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
