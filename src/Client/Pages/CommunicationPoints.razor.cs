using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Shared;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Pages
{
    public class CommunicationPointsClass : ComponentBase
    {
        [Inject]
        protected CustomAuthState AuthState { get; set; }
        [Inject]
        protected IControllerConnection ApiConn { get; set; }
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        private bool serverMessageIsError = false;

        public static int MaxPointsPerMedium => 100;
        public Dictionary<CommunicationPoint, bool> PointsAndToggles { get; set; }          // holds true if user selected for edit

        protected bool Initialized { get; set; } = false;
        protected bool InputError { get; set; } = false;
        protected bool HasAccess => AuthState.IsAdmin;
        protected bool IsSubmitting { get; set; } = false;
        protected string PointInputStyle
        {
            get => InputError ? "border: 1px solid red" : string.Empty;
        }
        protected string ServerMessage { get; set; } = string.Empty;
        protected string ServerMessageInlineStyle => serverMessageIsError ? "color: red;" : "color: green;";
        protected int TotalPoints
        {
            get
            {
                int total = 0;
                foreach (var percentage in PointsAndToggles.Keys)
                    total += percentage.Points;
                
                return total;
            }
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

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Communication Points");
            await CustomAuthState.GetUserPermissions(AuthState, ApiConn);
            
            if (HasAccess)
                await FetchCommunicationPoints();

            Initialized = true;
        }

        protected void ClearPoints(CommunicationPoint selectedMedium)
        {
            selectedMedium.Points = 0;
        }

        protected async Task FetchCommunicationPoints()
        {
            var response = await ApiConn.FetchCommunicationPoints();
            var points = new List<CommunicationPoint> { response.Item1, response.Item2 };
            InitializeDictionary(points);
        }

        protected bool SavePoints(CommunicationPoint selectedMedium)
        {
            InputError = false;
            bool returnValue = false;
            bool lessThanMax = selectedMedium.Points <= MaxPointsPerMedium;
            if (selectedMedium.Points >= 0 && lessThanMax)
            {
                PointsAndToggles[selectedMedium] = false;
                returnValue = true;
            }
            else if (selectedMedium.Points < 0)
            {
                selectedMedium.Points = 0;
                InputError = true;
                returnValue = false;
            }
            else
            {
                selectedMedium.Points = MaxPointsPerMedium;
                InputError = true;
                returnValue = false;
            }
            return returnValue;
        }

        protected async Task SubmitPoints()
        {
            if (TotalPoints < 1)
            {
                serverMessageIsError = true;
                ServerMessage = "You must assign at least one point (1 point) before submitting.";
                return;
            }
            IsSubmitting = true;
            serverMessageIsError = false;

            var toggledMedium = GetToggledMedium();
            if (toggledMedium != null) PointsAndToggles[toggledMedium] = false;
            var response = await ApiConn.SubmitPoints(PointsAndToggles.Keys.ToList());
            serverMessageIsError = response.Item1 == false;
            ServerMessage = response.Item2;
           
            IsSubmitting = false;
        }


        private void InitializeDictionary(List<CommunicationPoint> commPoints)
        {
            PointsAndToggles = new Dictionary<CommunicationPoint, bool>();
            foreach (var medium in commPoints)
            {
                PointsAndToggles.Add(medium, false);
            }
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
    }
}
