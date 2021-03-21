using EctBlazorApp.Client.Graph;
using System;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Shared
{
    public abstract class CustomState
    {
        public event Action OnChange;

        protected void NotifyStateChanged() => OnChange?.Invoke();

    }

    public class CustomAuthState : CustomState
    {
        public bool IsInitialized { get; private set; } = false;
        public virtual bool IsAdmin { get; private set; }
        public virtual bool IsLeader { get; private set; }

        public void SetIsAdmin(bool isAdmin)
        {
            IsAdmin = isAdmin;
            NotifyStateChanged();
        }

        public void SetIsLeader(bool isLeader)
        {
            IsLeader = isLeader;
            NotifyStateChanged();
        }

        public void SetIsInitialized(bool isInitialized)
        {
            IsInitialized = isInitialized;
            NotifyStateChanged();
        }

        public static async Task GetUserPermissions(CustomAuthState authState, IControllerConnection apiConn)                                     // Refresh the state only if necessary - i.e. the user refreshed the page or navigated via address bar
        {
            if (authState.IsInitialized == false)
            {
                authState.SetIsAdmin(await apiConn.IsProcessingUserAnAdmin());
                authState.SetIsLeader(await apiConn.IsProcessingUserALeader());
                authState.SetIsInitialized(true);
            }
        }
    }

    public class DashboardState : CustomState
    {
        public bool IsDrillDown { get; private set; } = false;
        public void SetIsDrillDown(bool isDrillDown)
        {
            IsDrillDown = isDrillDown;
            NotifyStateChanged();
        }
    }
}
