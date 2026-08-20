using BlazorServerTests.Api.Components.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorLayoutCodeBehindTemplate", Version = "1.0")]

namespace BlazorServerTests.Api.Components.Layout
{
    public partial class MainLayout
    {
        private bool _drawerOpen = true;
        [Inject]
        public ThemeService _themeService { get; set; } = default!;
        [Inject]
        public IJSRuntime JS { get; set; } = default!;
        [CascadingParameter]
        public HttpContext? HttpContext { get; set; }
        private bool IsDarkMode => HttpContext is not null ? !(HttpContext.Request.Cookies.TryGetValue("theme", out var theme) && theme == "light") : _themeService.IsDark;

        public void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        public async Task ToggleTheme()
        {
            _themeService.Toggle();
            await JS.InvokeVoidAsync("themeStorage.set", _themeService.IsDark ? "dark" : "light");
        }

        public void Dispose()
        {
            _themeService.OnChange -= StateHasChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _themeService.OnChange += StateHasChanged;
                var saved = await JS.InvokeAsync<string>("themeStorage.get");

                if (saved == "dark")
                {
                    _themeService.SetDark(true);
                }
                else if (saved == "light")
                {
                    _themeService.SetDark(false);
                }

                if (!string.IsNullOrEmpty(saved))
                {
                    StateHasChanged();
                }
            }
            if (firstRender && RendererInfo.IsInteractive)
            {
                _themeService.OnChange += StateHasChanged;
                var saved = await JS.InvokeAsync<string>("themeStorage.get");

                if (saved == "dark")
                {
                    _themeService.SetDark(true);
                }
                else if (saved == "light")
                {
                    _themeService.SetDark(false);
                }

                if (!string.IsNullOrEmpty(saved))
                {
                    StateHasChanged();
                }
            }
        }
    }
}