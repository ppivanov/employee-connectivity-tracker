using EctBlazorApp.Client.Graph;
using System;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Shared
{
    public class AuthState
    {
        public bool IsInitialized { get; private set; } = false;
        public bool IsAdmin { get; private set; }
        public bool IsLeader { get; private set; }

        public event Action OnChange;

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

        public static async Task GetUserPermissions(AuthState authState, IControllerConnection apiConn)                                     // Refresh the state only if necessary - i.e. the user refreshed the page or navigated via address bar
        {
            if (authState.IsInitialized == false)
            {
                authState.SetIsAdmin(await apiConn.IsProcessingUserAnAdmin());
                authState.SetIsLeader(await apiConn.IsProcessingUserALeader());
                authState.SetIsInitialized(true);
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
