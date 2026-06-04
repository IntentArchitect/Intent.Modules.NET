using System;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class HomePageDefaultHtmlExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Components.MudBlazor.HomePageDefaultHtmlExtension";

        public override int Order => 100;

        private readonly IChanges _changeManager;

        public HomePageDefaultHtmlExtension(IChanges changeManager)
        {
            _changeManager = changeManager;
        }

        protected override void OnAfterTemplateExecution(IApplication application)
        {
            const string templateId = "Intent.Blazor.Templates.Client.RazorComponentTemplate";

            var homeTemplate = application
                .FindTemplateInstances<IRazorFileTemplate>(templateId)
                .FirstOrDefault(t =>
                    t.GetMetadata().GetFilePath()
                     .EndsWith("Home.razor", StringComparison.OrdinalIgnoreCase));

            if (homeTemplate is null)
            {
                return;
            }

            var filePath = homeTemplate.GetMetadata().GetFilePath();
            var change = _changeManager.FindChange(filePath);
            if (change is null)
            {
                return;
            }

            var onDisk = File.Exists(filePath) ? File.ReadAllText(filePath) : null;
            var templateOutput = change.Content;

            if (onDisk is null || Normalize(onDisk) == Normalize(templateOutput))
            {
                var homePageContent = GetHomePageDefaultContent();
                change.ChangeContent(homePageContent, homePageContent);
            }
            else
            {
                // User has customised the file — cancel the overwrite
                change.ChangeContent(onDisk, onDisk);
            }
        }

        private static string Normalize(string s) => s.Trim().ReplaceLineEndings("\n");

        private static string GetHomePageDefaultContent() =>
            """
            @page "/"
            <PageTitle>Home</PageTitle>

            <div class="home-page">
                <div class="hero-orb hero-orb-1" aria-hidden="true"></div>
                <div class="hero-orb hero-orb-2" aria-hidden="true"></div>
                <div class="hero-orb hero-orb-3" aria-hidden="true"></div>

            @* ── HERO ────────────────────────────────────────────────────── *@
                <div class="home-hero">
                    <div class="hero-content ux-fade-in-up">

                        <div class="hero-badge">
                            <span class="hero-badge-dot"></span>
                            Intent Architect · Blazor
                        </div>

                        <h1 class="hero-title">
                            Build smarter apps<br />
                            <span class="hero-title-gradient">faster than ever</span>
                        </h1>

                        <p class="hero-subtitle">
                            A full-stack Blazor application powered by model-driven automation —
                            clean architecture, AI-generated code, and a polished UI from a single design model.
                        </p>

                        <div class="hero-actions">
                            <MudButton Variant="Variant.Filled"
                                       Color="Color.Primary"
                                       Href="https://docs.intentarchitect.com"
                                       Target="_blank"
                                       Class="hero-cta-primary"
                                       StartIcon="@Icons.Material.Filled.RocketLaunch">
                                Get Started
                            </MudButton>
                            <MudButton Variant="Variant.Outlined"
                                       Color="Color.Secondary"
                                       Href="https://docs.intentarchitect.com/articles/application-development/modelling/ui-designer/blazor-modeling/blazor-modeling.html"
                                       Target="_blank"
                                       Rel="noopener noreferrer"
                                       StartIcon="@Icons.Material.Filled.MenuBook">
                                Documentation
                            </MudButton>
                        </div>

                    </div>
                </div>

            @* ── MODULES ─────────────────────────────────────────────────── *@
                <MudContainer MaxWidth="MaxWidth.Large" Class="bento-section">

                    <div class="bento-grid">

                        <MudLink Href="https://docs.intentarchitect.com" Target="_blank" Underline="Underline.None" Class="bento-cell">
                            <div class="bento-tile" style="--tc:#0EA5E9">
                                <div class="bento-tile-top">
                                    <div class="bento-icon">
                                        <MudIcon Icon="@Icons.Material.Filled.MenuBook" />
                                    </div>
                                </div>
                                <div class="bento-tile-bottom">
                                    <div class="bento-tile-name">Documentation</div>
                                    <div class="bento-tile-desc">Guides and API references for Intent Architect</div>
                                </div>
                            </div>
                        </MudLink>

                        <MudLink Href="https://docs.intentarchitect.com/articles/getting-started/welcome/welcome.html" Target="_blank" Underline="Underline.None" Class="bento-cell">
                            <div class="bento-tile" style="--tc:#22D3EE">
                                <div class="bento-tile-top">
                                    <div class="bento-icon"><MudIcon Icon="@Icons.Material.Filled.School" /></div>
                                </div>
                                <div class="bento-tile-bottom">
                                    <div class="bento-tile-name">Get Started</div>
                                    <div class="bento-tile-desc">Step-by-step tutorials to build your first application</div>
                                </div>
                            </div>
                        </MudLink>

                        <MudLink Href="https://github.com/IntentArchitect" Target="_blank" Underline="Underline.None" Class="bento-cell">
                            <div class="bento-tile" style="--tc:#A78BFA">
                                <div class="bento-tile-top">
                                    <div class="bento-icon"><MudIcon Icon="@Icons.Material.Filled.Code" /></div>
                                </div>
                                <div class="bento-tile-bottom">
                                    <div class="bento-tile-name">Open Source</div>
                                    <div class="bento-tile-desc">Explore modules and samples on GitHub</div>
                                </div>
                            </div>
                        </MudLink>

                    </div>
                </MudContainer>

            </div>
            """;
    }
}