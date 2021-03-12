using System;

namespace EctBlazorApp.Client.Shared
{
    public class NavState
    {
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

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
