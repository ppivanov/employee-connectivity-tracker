using EctBlazorApp.Client.Graph;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Pages
{
    public class CommunicationPointsClass : ComponentBase
    {
        [Inject]
        HttpClient Http { get; set; }

        [Inject]
        IJSRuntime jsRuntime { get; set; }

        [Inject]
        IControllerConnection ApiConn { get; set; }

        private bool serverMessageIsError = false;
        protected const int maxPointsPerMedium = 100;
        protected bool isAdmin = false;
        protected bool isSubmitting = false;
        protected bool initialized = false;
        protected bool inputError = false;
        protected string serverMessage = "";
        protected Dictionary<CommunicationPoint, bool> pointsDict;          // holds true if user selected for edit

        protected int TotalPoints
        {
            get
            {
                int total = 0;
                foreach (var percentage in pointsDict.Keys)
                {
                    total += percentage.Points;
                }
                return total;
            }
        }

        protected string PointInputStyle
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


        protected override async Task OnInitializedAsync()
        {
            await jsRuntime.InvokeVoidAsync("setPageTitle", "Communication Points");
            isAdmin = await ApiConn.IsProcessingUserAnAdmin(Http);
            if (isAdmin)
                await FetchCommunicationPoints();

            initialized = true;
        }

        protected async Task FetchCommunicationPoints()
        {
            var response = await ApiConn.FetchCommunicationPoints();
            var percentages = new List<CommunicationPoint> { response.Item1, response.Item2 };
            InitializeDictionary(percentages);
        }

        private void InitializeDictionary(List<CommunicationPoint> commPoints)
        {
            pointsDict = new Dictionary<CommunicationPoint, bool>();
            foreach (var medium in commPoints)
            {
                pointsDict.Add(medium, false);
            }
        }

        protected bool SavePercentage(CommunicationPoint selectedMedium)
        {
            inputError = false;
            bool lessThanMax = selectedMedium.Points <= maxPointsPerMedium;
            if (TotalPoints >= 0 && lessThanMax)
            {
                pointsDict[selectedMedium] = false;
                return true;
            }
            else
            {
                selectedMedium.Points = maxPointsPerMedium;
                inputError = true;
                return false;
            }
        }
        protected void EditPercentage(CommunicationPoint selectedMedium)
        {
            var toggledMedium = GetToggledMedium();
            if (toggledMedium != null)
            {
                if (SavePercentage(toggledMedium))                          // If input value is less than % left only then toggle the other medium
                    pointsDict[selectedMedium] = true;
            }
            else
                pointsDict[selectedMedium] = true;                          // If no previous medium is toggled

        }
        protected void ClearPoints(CommunicationPoint selectedMedium)
        {
            selectedMedium.Points = 0;
        }
        private CommunicationPoint GetToggledMedium()
        {
            foreach (var medium in pointsDict.Keys)
            {
                if (pointsDict[medium])
                    return medium;
            }
            return null;
        }

        protected async Task SubmitPoints()
        {
            if (TotalPoints < 1)
            {
                serverMessageIsError = true;
                serverMessage = "You must assign at least one point (1 point) before submitting.";
                return;
            }
            isSubmitting = true;
            serverMessageIsError = false;

            var toggledMedium = GetToggledMedium();
            if (toggledMedium != null) pointsDict[toggledMedium] = false;
            var response = await ApiConn.SubmitPoints(pointsDict.Keys.ToList());
            serverMessageIsError = response.Item1;
            serverMessage = response.Item2;
           
            isSubmitting = false;
        }
    }
}
