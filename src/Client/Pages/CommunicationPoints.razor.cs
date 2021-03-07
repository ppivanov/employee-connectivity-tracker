using EctBlazorApp.Client.Graph;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Pages
{
    public class CommunicationPointsClass : ComponentBase
    {
        [Inject]
        protected IControllerConnection ApiConn { get; set; }
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        private bool serverMessageIsError = false;
        public const int maxPointsPerMedium = 100;
        protected bool isAdmin = false;
        protected bool isSubmitting = false;
        protected bool initialized = false;
        protected bool inputError = false;
        protected string serverMessage = "";
        public Dictionary<CommunicationPoint, bool> PointsAndToggles { get; set; }          // holds true if user selected for edit

        protected int TotalPoints
        {
            get
            {
                int total = 0;
                foreach (var percentage in PointsAndToggles.Keys)
                {
                    total += percentage.Points;
                }
                return total;
            }
        }

        protected string PointInputStyle
        {
            get => inputError ? "border: 1px solid red" : "";
        }

        protected string ServerMessageInlineStyle
        {
            get => serverMessageIsError ? "color: red;" : "color: green;";
        }

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Communication Points");
            isAdmin = await ApiConn.IsProcessingUserAnAdmin();
            if (isAdmin)
                await FetchCommunicationPoints();

            initialized = true;
        }

        protected async Task FetchCommunicationPoints()
        {
            var response = await ApiConn.FetchCommunicationPoints();
            var points = new List<CommunicationPoint> { response.Item1, response.Item2 };
            InitializeDictionary(points);
        }

        private void InitializeDictionary(List<CommunicationPoint> commPoints)
        {
            PointsAndToggles = new Dictionary<CommunicationPoint, bool>();
            foreach (var medium in commPoints)
            {
                PointsAndToggles.Add(medium, false);
            }
        }

        protected bool SavePoints(CommunicationPoint selectedMedium)
        {
            inputError = false;
            bool returnValue = false;
            bool lessThanMax = selectedMedium.Points <= maxPointsPerMedium;
            if (selectedMedium.Points >= 0 && lessThanMax)
            {
                PointsAndToggles[selectedMedium] = false;
                returnValue = true;
            }
            else if (selectedMedium.Points < 0)
            {
                selectedMedium.Points = 0;
                inputError = true;
                returnValue = false;
            }
            else
            {
                selectedMedium.Points = maxPointsPerMedium;
                inputError = true;
                returnValue = false;
            }
            return returnValue;
        }

        public void EditPoints(CommunicationPoint selectedMedium)
        {
            var toggledMedium = GetToggledMedium();
            if (toggledMedium != null)
            {
                if (SavePoints(toggledMedium))                          // If input value is less than % left only then toggle the other medium
                    PointsAndToggles[selectedMedium] = true;
            }
            else
                PointsAndToggles[selectedMedium] = true;                          // If no previous medium is toggled
        }

        protected void ClearPoints(CommunicationPoint selectedMedium)
        {
            selectedMedium.Points = 0;
        }

        private CommunicationPoint GetToggledMedium()
        {
            foreach (var medium in PointsAndToggles.Keys)
            {
                if (PointsAndToggles[medium])
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
            if (toggledMedium != null) PointsAndToggles[toggledMedium] = false;
            var response = await ApiConn.SubmitPoints(PointsAndToggles.Keys.ToList());
            serverMessageIsError = response.Item1 == false;
            serverMessage = response.Item2;
           
            isSubmitting = false;
        }
    }
}
