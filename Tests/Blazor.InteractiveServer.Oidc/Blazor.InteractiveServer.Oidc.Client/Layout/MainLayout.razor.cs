using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorLayoutCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.Oidc.Client.Layout
{
    public partial class MainLayout
    {
        private bool _drawerOpen = true;

        public void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }
    }
}