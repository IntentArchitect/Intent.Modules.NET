using Blazor.InteractiveAuto.Oidc.Client.Components.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorLayoutCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveAuto.Oidc.Client.Layout
{
    public partial class MainLayout
    {
        [Inject]
        public ThemeService _themeService { get; set; } = default!;
        [Inject]
        public IJSRuntime JS { get; set; } = default!;
        [CascadingParameter]
        public HttpContext? HttpContext { get; set; }
        private bool IsDarkMode => HttpContext is not null ? !(HttpContext.Request.Cookies.TryGetValue("theme", out var theme) && theme == "light") : _themeService.IsDark;

        public void Dispose()
        {
            _themeService.OnChange -= StateHasChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
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