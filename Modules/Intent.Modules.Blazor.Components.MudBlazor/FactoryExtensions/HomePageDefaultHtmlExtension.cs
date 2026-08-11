using System;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentStyle;
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

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var homeTemplate = application
                .FindTemplateInstances<RazorComponentTemplate>(RazorComponentTemplate.TemplateId)
                .FirstOrDefault(x => string.Equals(
                    RazorComponentTemplate.GetOutputFileName(x.Model),
                    "Home",
                    StringComparison.OrdinalIgnoreCase));

            if (homeTemplate is null)
            {
                return;
            }

            var styleTemplate = application
                .FindTemplateInstances<RazorComponentStyleTemplate>(RazorComponentStyleTemplate.TemplateId)
                .FirstOrDefault(x => x.Model.InternalElement.Id == homeTemplate.Model.InternalElement.Id);

            if (styleTemplate is null || File.Exists(styleTemplate.GetMetadata().GetFilePath()))
            {
                // RazorComponentStyleTemplate pins no OverwriteBehaviour, so seeding an existing file
                // would overwrite whatever the developer has since written into it.
                return;
            }

            styleTemplate.StyleContentOverride = GetHomePageDefaultStyleContent();
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

        private static string GetHomePageDefaultStyleContent() =>
            """
            /* ============================================================================
            Home.razor.css
            Blazor CSS isolation — styles scoped to the Home page component only.
            Home-page-specific keyframes are defined here so they are co-located
            with the component that uses them.
            ============================================================================ */


            /* ============================================================================
            SECTION 1: Home-page Keyframes
            ============================================================================ */

            @keyframes orb-float {
            0%, 100% {
            transform: translate(0, 0) scale(1);
            }

            33% {
            transform: translate(20px, -15px) scale(1.04);
            }

            66% {
            transform: translate(-15px, 10px) scale(0.97);
            }
            }

            @keyframes shimmer {
            0% {
            background-position: -200% center;
            }

            100% {
            background-position: 200% center;
            }
            }

            @keyframes pulse-dot {
            0%, 100% {
            opacity: 1;
            transform: scale(1);
            }

            50% {
            opacity: 0.45;
            transform: scale(0.65);
            }
            }


            /* ============================================================================
            SECTION 2: Full-page Wrapper
            ============================================================================ */

            .home-page {
            position: relative;
            overflow: hidden;
            min-height: calc(100vh - 64px);
            margin: -1rem; /* bleed out of layout padding */
            }


            /* ============================================================================
            SECTION 3: Hero
            ============================================================================ */

            .home-hero {
            position: relative;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 2rem 1.5rem 1.5rem;
            }

            /* Ambient orbs */
            .hero-orb {
            position: absolute;
            border-radius: 50%;
            pointer-events: none;
            filter: blur(90px);
            }

            .hero-orb-1 {
            width: 600px;
            height: 600px;
            top: -160px;
            left: -120px;
            background: radial-gradient(circle, rgba(14,165,233,0.20) 0%, transparent 70%);
            animation: orb-float 14s ease-in-out infinite;
            }

            .hero-orb-2 {
            width: 500px;
            height: 500px;
            bottom: -120px;
            right: -100px;
            background: radial-gradient(circle, rgba(99,102,241,0.16) 0%, transparent 70%);
            animation: orb-float 18s ease-in-out infinite reverse;
            }

            .hero-orb-3 {
            width: 320px;
            height: 320px;
            top: 35%;
            left: 55%;
            background: radial-gradient(circle, rgba(14,165,233,0.10) 0%, transparent 70%);
            animation: orb-float 22s ease-in-out infinite;
            }

            /* Hero content */
            .hero-content {
            position: relative;
            z-index: 1;
            text-align: center;
            max-width: 800px;
            width: 100%;
            }

            /* Animated badge */
            .hero-badge {
            display: inline-flex;
            align-items: center;
            gap: 0.5rem;
            font-size: 0.72rem;
            font-weight: 600;
            letter-spacing: 0.08em;
            text-transform: uppercase;
            color: var(--primary);
            background: rgba(14,165,233,0.07);
            border: 1px solid rgba(14,165,233,0.22);
            border-radius: var(--radius-full);
            padding: 0.4em 1.1em;
            margin-bottom: 1rem;
            }

            .hero-badge-dot {
            width: 6px;
            height: 6px;
            border-radius: 50%;
            background: var(--primary);
            box-shadow: 0 0 8px var(--primary);
            flex-shrink: 0;
            animation: pulse-dot 2s ease-in-out infinite;
            }

            /* Big headline */
            .hero-title {
            font-size: clamp(2rem, 4.5vw, 3rem);
            font-weight: 700;
            line-height: 1.08;
            letter-spacing: -0.035em;
            color: var(--text);
            margin: 0 0 0.75rem;
            text-wrap: balance;
            }

            .hero-title-gradient {
            background: var(--gradient-brand);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
            }

            /* Subtitle */
            .hero-subtitle {
            font-size: var(--type-body-md);
            color: var(--text-muted);
            line-height: 1.6;
            margin: 0 auto 1.5rem;
            max-width: 560px;
            text-wrap: pretty;
            }

            .hero-actions {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 0.875rem;
            flex-wrap: wrap;
            margin-bottom: 0;
            }

            /* Stats strip */
            .hero-stats {
            display: inline-flex;
            align-items: center;
            gap: 2rem;
            background: rgba(14,165,233,0.04);
            border: 1px solid rgba(14,165,233,0.12);
            border-radius: var(--radius-full);
            padding: 0.85rem 2.25rem;
            flex-wrap: wrap;
            justify-content: center;
            }

            .hero-stat {
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 0.1rem;
            line-height: 1.2;
            }

            .hero-stat-value {
            font-size: 1.05rem;
            font-weight: 700;
            color: var(--text);
            letter-spacing: -0.01em;
            }

            .hero-stat-label {
            font-size: 0.65rem;
            font-weight: 500;
            letter-spacing: 0.07em;
            text-transform: uppercase;
            color: var(--text-secondary);
            }

            .hero-stat-sep {
            width: 1px;
            height: 2.2rem;
            background: var(--border);
            flex-shrink: 0;
            }


            /* ============================================================================
            SECTION 4: Module Bento Grid
            ============================================================================ */

            .bento-section {
            padding-bottom: 1.5rem;
            }

            .bento-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            grid-template-rows: repeat(2, 130px);
            gap: 0.625rem;
            }

            .bento-cell {
            display: block;
            }

            /* Tile shell */
            .bento-tile {
            height: 100%;
            border-radius: var(--radius-lg);
            background: var(--surface);
            border: 1px solid var(--border);
            overflow: hidden;
            position: relative;
            display: flex;
            flex-direction: column;
            justify-content: flex-start;
            transition: border-color var(--dur-med) var(--ease-out), box-shadow var(--dur-med) var(--ease-out), transform var(--dur-med) var(--ease-out);
            }

            /* Gradient top-line on hover */
            .bento-tile::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 2px;
            background: linear-gradient(90deg, var(--tc, var(--primary)), transparent);
            opacity: 0;
            transition: opacity var(--dur-med) var(--ease-out);
            }

            /* Soft gradient tint overlay on hover */
            .bento-tile::after {
            content: '';
            position: absolute;
            inset: 0;
            background: radial-gradient(ellipse at 30% 30%, color-mix(in srgb, var(--tc, var(--primary)) 8%, transparent) 0%, transparent 65%);
            opacity: 0;
            transition: opacity var(--dur-med) var(--ease-out);
            pointer-events: none;
            }

            .bento-cell:hover .bento-tile {
            border-color: color-mix(in srgb, var(--tc, var(--primary)) 45%, transparent);
            box-shadow: 0 0 30px color-mix(in srgb, var(--tc, var(--primary)) 12%, transparent), var(--shadow-3);
            transform: translateY(-3px);
            }

            .bento-cell:hover .bento-tile::before {
            opacity: 1;
            }

            .bento-cell:hover .bento-tile::after {
            opacity: 1;
            }

            /* Icon area */
            .bento-tile-top {
            padding: 1rem 1rem 0;
            flex-shrink: 0;
            position: relative;
            z-index: 1;
            }

            .bento-icon {
            width: 36px;
            height: 36px;
            border-radius: var(--radius-sm);
            background: color-mix(in srgb, var(--tc, var(--primary)) 13%, transparent);
            display: flex;
            align-items: center;
            justify-content: center;
            color: var(--tc, var(--primary));
            transition: box-shadow var(--dur-med) var(--ease-out);
            }

            .bento-cell:hover .bento-icon {
            box-shadow: 0 0 16px color-mix(in srgb, var(--tc, var(--primary)) 45%, transparent);
            }

            /* Text area */
            .bento-tile-bottom {
            padding: 0 1rem 0.75rem;
            margin-top: auto;
            flex-shrink: 0;
            display: flex;
            flex-direction: column;
            position: relative;
            z-index: 1;
            }

            .bento-tile-name {
            font-size: var(--type-body-md);
            font-weight: 600;
            letter-spacing: -0.015em;
            color: var(--text);
            margin-bottom: 0.2rem;
            }

            .bento-tile-desc {
            font-size: var(--type-label-lg);
            color: var(--text-muted);
            line-height: 1.4;
            }


            /* ============================================================================
            SECTION 5: Responsive
            ============================================================================ */

            @media (max-width: 700px) {
            .bento-grid {
            grid-template-columns: 1fr 1fr;
            grid-template-rows: repeat(3, 115px);
            }

            .hero-title {
            font-size: 2rem;
            }
            }

            @media (max-width: 420px) {
            .bento-grid {
            grid-template-columns: 1fr;
            grid-auto-rows: 120px;
            }
            }

            """;
    }
}
