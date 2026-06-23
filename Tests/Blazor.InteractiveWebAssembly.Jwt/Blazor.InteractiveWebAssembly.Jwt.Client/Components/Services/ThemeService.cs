using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Common.ThemeServiceTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.Jwt.Client.Components.Services
{
    public class ThemeService
    {
        public bool IsDark { get; private set; } = true;

        public void Toggle()
        {
            IsDark = !IsDark;
            OnChange?.Invoke();
        }

        public void SetDark(bool isDark)
        {
            if (IsDark == isDark)
            {
                return;
            }
            IsDark = isDark;
            OnChange?.Invoke();
        }

        public event Action? OnChange;
    }
}