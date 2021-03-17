using System.Threading.Tasks;
using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Shared;
using Microsoft.AspNetCore.Components;

namespace EctBlazorApp.Client.Pages
{
    public class ViewTeamsClass : ComponentBase
    {
        [Inject]
        protected CustomAuthState AuthState { get; set; }
        [Inject]
        protected IControllerConnection ApiConn { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await CustomAuthState.GetUserPermissions(AuthState, ApiConn);
        }
    }
}