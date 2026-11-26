using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// What the User Interface model states about a .NET MAUI app, read by <c>intent.maui</c> from the views the XAML and C#
/// languages read, and by <c>intent.mvvmtoolkit</c> from the MVVM Toolkit's messenger: the views' kinds, security, routes and
/// properties, the Shell's Regions and the tab entries of the pages they show, compositions, view models, dependencies,
/// navigations, App Events and a Persisted Store. The fixture adapts eShop's ClientApp, as the eShop house module proved it.
/// </summary>
public class MauiTests
{
    private static readonly Dictionary<string, string> Code = new()
    {
        ["src/Shop/Shop.csproj"] = """<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net9.0</TargetFramework><UseMaui>true</UseMaui></PropertyGroup></Project>""",
        ["src/Shop/App.xaml"] = """
            <?xml version="1.0" encoding="UTF-8" ?>
            <Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Shop.App">
                <Application.Resources><ResourceDictionary /></Application.Resources>
            </Application>
            """,
        ["src/Shop/App.xaml.cs"] = """
            namespace Shop;
            public partial class App : Application
            {
                public App() { InitializeComponent(); }
            }
            """,
        ["src/Shop/Resources/Styles/Colors.xaml"] = """
            <?xml version="1.0" encoding="UTF-8" ?>
            <ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
                <Color x:Key="Accent">#000000</Color>
            </ResourceDictionary>
            """,
        ["src/Shop/AppShell.xaml"] = """
            <?xml version="1.0" encoding="UTF-8" ?>
            <Shell
                x:Class="Shop.AppShell"
                xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                xmlns:views="clr-namespace:Shop.Views">
                <Shell.Resources>
                    <ResourceDictionary />
                </Shell.Resources>
                <FlyoutItem>
                    <ShellContent ContentTemplate="{DataTemplate views:LoginView}" Route="Login" />
                </FlyoutItem>
                <TabBar Route="Main">
                    <ShellContent
                        Title="CATALOG"
                        ContentTemplate="{DataTemplate views:CatalogView}"
                        Icon="{StaticResource CatalogIcon}"
                        Route="Catalog" />
                </TabBar>
            </Shell>
            """,
        ["src/Shop/AppShell.xaml.cs"] = """
            using Shop.Services;
            using Shop.Views;
            namespace Shop;

            public partial class AppShell : Shell
            {
                private readonly INavigationService _navigationService;

                public AppShell(INavigationService navigationService)
                {
                    _navigationService = navigationService;
                    InitializeRouting();
                    InitializeComponent();
                }

                private static void InitializeRouting()
                {
                    //Routing.RegisterRoute("Login", typeof(LoginView));
                    Routing.RegisterRoute("Basket", typeof(BasketView));
                    Routing.RegisterRoute("Filter", typeof(FiltersView));
                }
            }
            """,
        ["src/Shop/Views/ContentPageBase.cs"] = """
            namespace Shop.Views;
            public abstract class ContentPageBase : ContentPage
            {
            }
            """,
        ["src/Shop/Views/CatalogView.xaml"] = """
            <?xml version="1.0" encoding="utf-8" ?>
            <views:ContentPageBase
                x:Class="Shop.Views.CatalogView"
                xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                xmlns:templates="clr-namespace:Shop.Views.Templates"
                xmlns:viewmodels="clr-namespace:Shop.ViewModels"
                xmlns:views="clr-namespace:Shop.Views"
                x:DataType="viewmodels:CatalogViewModel">
                <Grid>
                    <CollectionView ItemsSource="{Binding Products}">
                        <CollectionView.ItemTemplate>
                            <DataTemplate>
                                <templates:ProductTemplate />
                            </DataTemplate>
                        </CollectionView.ItemTemplate>
                    </CollectionView>
                    <views:BadgeView x:Name="badge" Text="{Binding BadgeCount}" />
                </Grid>
            </views:ContentPageBase>
            """,
        ["src/Shop/Views/CatalogView.xaml.cs"] = """
            using CommunityToolkit.Mvvm.Messaging;
            using Shop.Messages;
            using Shop.ViewModels;
            namespace Shop.Views;

            public partial class CatalogView
            {
                public CatalogView(CatalogViewModel viewModel)
                {
                    BindingContext = viewModel;
                    InitializeComponent();
                }

                protected override void OnAppearing()
                {
                    base.OnAppearing();
                    WeakReferenceMessenger.Default.Register<CatalogView, ProductCountChangedMessage>(this, (recipient, message) => { });
                }
            }
            """,
        ["src/Shop/Views/BasketView.xaml"] = """
            <?xml version="1.0" encoding="utf-8" ?>
            <views:ContentPageBase
                x:Class="Shop.Views.BasketView"
                xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                xmlns:views="clr-namespace:Shop.Views"
                Title="BASKET">
                <Label Text="{Binding Total}" />
            </views:ContentPageBase>
            """,
        ["src/Shop/Views/BasketView.xaml.cs"] = """
            using Shop.ViewModels;
            namespace Shop.Views;

            public partial class BasketView
            {
                private readonly BasketViewModel _viewModel;

                public BasketView(BasketViewModel viewModel)
                {
                    BindingContext = _viewModel = viewModel;
                    InitializeComponent();
                }
            }
            """,
        ["src/Shop/Views/LoginView.xaml"] = """
            <?xml version="1.0" encoding="utf-8" ?>
            <ContentPage
                x:Class="Shop.Views.LoginView"
                xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                xmlns:vm="clr-namespace:Shop.ViewModels">
                <ContentPage.BindingContext>
                    <vm:LoginViewModel />
                </ContentPage.BindingContext>
                <Button Text="Sign in" Command="{Binding SignInCommand}" />
            </ContentPage>
            """,
        ["src/Shop/Views/LoginView.xaml.cs"] = """
            namespace Shop.Views;

            public partial class LoginView : ContentPage
            {
                public LoginView()
                {
                    InitializeComponent();
                }
            }
            """,
        ["src/Shop/Views/FiltersView.xaml"] = """
            <?xml version="1.0" encoding="utf-8" ?>
            <ContentPage x:Class="Shop.Views.FiltersView" xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
                <Label Text="Filters" />
            </ContentPage>
            """,
        ["src/Shop/Views/FiltersView.xaml.cs"] = """
            using Shop.ViewModels;
            namespace Shop.Views;

            public partial class FiltersView : ContentPage
            {
                public FiltersView(CatalogViewModel viewModel)
                {
                    BindingContext = viewModel;
                    InitializeComponent();
                }
            }
            """,
        ["src/Shop/Views/Templates/ProductTemplate.xaml"] = """
            <?xml version="1.0" encoding="utf-8" ?>
            <ContentView x:Class="Shop.Views.Templates.ProductTemplate" xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
                <Label Text="{Binding Name}" />
            </ContentView>
            """,
        ["src/Shop/Views/Templates/ProductTemplate.xaml.cs"] = """
            namespace Shop.Views.Templates;

            public partial class ProductTemplate : ContentView
            {
                public ProductTemplate()
                {
                    InitializeComponent();
                }
            }
            """,
        ["src/Shop/Views/BadgeView.cs"] = """
            namespace Shop.Views;

            public class BadgeView : Grid
            {
                public static BindableProperty TextProperty =
                    BindableProperty.Create(nameof(Text), typeof(string), typeof(BadgeView), "0");

                public string Text
                {
                    get => (string)GetValue(TextProperty);
                    set => SetValue(TextProperty, value);
                }
            }
            """,
        ["src/Shop/Services/Navigation.cs"] = """
            namespace Shop.Services;

            public interface INavigationService
            {
                Task NavigateToAsync(string route, IDictionary<string, object> routeParameters = null);
            }

            public class MauiNavigationService : INavigationService
            {
                public Task NavigateToAsync(string route, IDictionary<string, object> routeParameters = null)
                {
                    var shellNavigation = new ShellNavigationState(route);
                    return routeParameters != null
                        ? Shell.Current.GoToAsync(shellNavigation, routeParameters)
                        : Shell.Current.GoToAsync(shellNavigation);
                }
            }
            """,
        ["src/Shop/Services/SettingsService.cs"] = """
            namespace Shop.Services;

            public class SettingsService
            {
                private const string IdUseMocks = "use_mocks";

                public bool UseMocks
                {
                    get => Preferences.Get(IdUseMocks, true);
                    set => Preferences.Set(IdUseMocks, value);
                }
            }
            """,
        ["src/Shop/Messages/ProductCountChangedMessage.cs"] = """
            using CommunityToolkit.Mvvm.Messaging.Messages;
            namespace Shop.Messages;

            public class ProductCountChangedMessage(int count) : ValueChangedMessage<int>(count);
            """,
        ["src/Shop/ViewModels/ViewModelBase.cs"] = """
            using Shop.Services;
            namespace Shop.ViewModels;

            public abstract partial class ViewModelBase : ObservableObject
            {
                public ViewModelBase(INavigationService navigationService)
                {
                    NavigationService = navigationService;
                }

                public INavigationService NavigationService { get; }
            }
            """,
        ["src/Shop/ViewModels/CatalogViewModel.cs"] = """
            using CommunityToolkit.Mvvm.Messaging;
            using Shop.Messages;
            using Shop.Services;
            namespace Shop.ViewModels;

            public partial class CatalogViewModel : ViewModelBase
            {
                [ObservableProperty] public partial int BadgeCount { get; set; }

                public CatalogViewModel(INavigationService navigationService) : base(navigationService)
                {
                    WeakReferenceMessenger.Default.Register<CatalogViewModel, ProductCountChangedMessage>(this, (_, message) => { BadgeCount = message.Value; });
                }

                [RelayCommand]
                private async Task ViewBasketAsync()
                {
                    await NavigationService.NavigateToAsync("Basket");
                }
            }
            """,
        ["src/Shop/ViewModels/BasketViewModel.cs"] = """
            using CommunityToolkit.Mvvm.Messaging;
            using Shop.Messages;
            using Shop.Services;
            namespace Shop.ViewModels;

            public partial class BasketViewModel : ViewModelBase
            {
                [ObservableProperty] public partial decimal Total { get; set; }

                public BasketViewModel(INavigationService navigationService) : base(navigationService)
                {
                }

                [RelayCommand]
                private async Task CheckoutAsync()
                {
                    WeakReferenceMessenger.Default.Send(new ProductCountChangedMessage(0));
                    await NavigationService.NavigateToAsync("//Main/Catalog");
                }
            }
            """,
        ["src/Shop/ViewModels/LoginViewModel.cs"] = """
            using Shop.Services;
            using Shop.Validations;
            namespace Shop.ViewModels;

            public partial class LoginViewModel : ObservableObject
            {
                [ObservableProperty] public partial ValidatableObject<string> UserName { get; set; } = new();

                [RelayCommand]
                private Task SignInAsync() => Shell.Current.GoToAsync("//Main/Catalog");
            }
            """,
        ["src/Shop/Validations/ValidatableObject.cs"] = """
            namespace Shop.Validations;

            public class ValidatableObject<T> : ObservableObject
            {
                public T Value { get; set; }
            }
            """
    };

    private static JsonNode Model(string productType = "Component", bool bindable = true, string catalogLabel = "CATALOG", string valueType = "object", bool persisted = true, string optIn = "") =>
        JsonNode.Parse($$"""
        {
          "designers": {
            "User Interface": {
              "packages": [{
                "id": "ui", "name": "Shop", "type": "User Interface Package",
                "elements": [
                  { "id": "shell", "type": "Layout", "name": "AppShell", "children": [
                    { "id": "login-region", "type": "Region", "name": "Login" },
                    { "id": "main-region", "type": "Region", "name": "Main" }
                  ] },
                  { "id": "views", "type": "Folder", "name": "Views", "children": [
                    { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { "Route": "/Main/Catalog" } } },
                    { "id": "basket", "type": "Page", "name": "BasketView", "stereotypes": { "Page": { "Route": "/Basket", "Title": "BASKET" } } },
                    { "id": "login", "type": "Page", "name": "LoginView", "stereotypes": { "Page": { "Route": "/Login" } } },
                    { "id": "filters", "type": "Page", "name": "FiltersView", "stereotypes": { "Page": { "Route": "/Filter" } } },
                    { "id": "badge", "type": "Component", "name": "BadgeView", "children": [
                      { "id": "badge-text", "type": "Property", "name": "Text", "typeRef": { "type": "string" }{{(bindable ? """, "stereotypes": { "[Bindable]": { } }""" : "")}} }
                    ] },
                    { "id": "templates", "type": "Folder", "name": "Templates", "children": [
                      { "id": "product", "type": "{{productType}}", "name": "ProductTemplate"{{(productType == "Page" ? """, "stereotypes": { "Page": { } }""" : "")}} }
                    ] }
                  ] },
                  { "id": "viewmodels", "type": "Folder", "name": "ViewModels", "children": [
                    { "id": "catalog-vm", "type": "View Model", "name": "CatalogViewModel", "children": [ { "id": "badge-count", "type": "Property", "name": "BadgeCount", "typeRef": { "type": "int" } } ] },
                    { "id": "basket-vm", "type": "View Model", "name": "BasketViewModel", "children": [ { "id": "total", "type": "Property", "name": "Total", "typeRef": { "type": "decimal" } } ] },
                    { "id": "login-vm", "type": "View Model", "name": "LoginViewModel" }
                  ] },
                  { "id": "validations", "type": "Folder", "name": "Validations", "children": [
                    { "id": "validatable", "type": "View Model", "name": "ValidatableObject", "children": [ { "id": "value", "type": "Property", "name": "Value", "typeRef": { "type": "{{valueType}}" } } ] }
                  ] },
                  { "id": "messages", "type": "Folder", "name": "Messages", "children": [
                    { "id": "count-changed", "type": "App Event", "name": "ProductCountChangedMessage", "children": [ { "id": "count-value", "type": "Property", "name": "Value", "typeRef": { "type": "int" } } ] }
                  ] },
                  { "id": "services", "type": "Folder", "name": "Services", "children": [
                    { "id": "navigation", "type": "UI Service", "name": "MauiNavigationService" },
                    { "id": "settings", "type": "Store", "name": "SettingsService"{{(persisted ? """, "stereotypes": { "Persisted": { } }""" : "")}}, "children": [ { "id": "use-mocks", "type": "Property", "name": "UseMocks", "typeRef": { "type": "bool" } } ] }
                  ] }
                ]
              }],
              "associations": [
                { "id": "login-shows", "type": "Composition", "source": { "element": "login-region" }, "target": { "element": "login", "stereotypes": { "Entry": { "Key": "Login" } } } },
                { "id": "main-shows", "type": "Composition", "source": { "element": "main-region" }, "target": { "element": "catalog", "stereotypes": { "Entry": { "Label": "{{catalogLabel}}", "Icon": "CatalogIcon", "Key": "Catalog" } } } },
                { "id": "shell-navigation", "type": "Uses", "source": { "element": "shell" }, "target": { "element": "navigation" } },
                { "id": "catalog-product", "type": "Composition", "source": { "element": "catalog" }, "target": { "element": "product" } },
                { "id": "catalog-badge", "type": "Composition", "source": { "element": "catalog" }, "target": { "element": "badge" } },
                { "id": "catalog-vm-uses", "type": "Uses", "source": { "element": "catalog" }, "target": { "element": "catalog-vm" } },
                { "id": "basket-vm-uses", "type": "Uses", "source": { "element": "basket" }, "target": { "element": "basket-vm" } },
                { "id": "login-vm-uses", "type": "Uses", "source": { "element": "login" }, "target": { "element": "login-vm" } },
                { "id": "filters-vm-uses", "type": "Uses", "source": { "element": "filters" }, "target": { "element": "catalog-vm" } },
                { "id": "catalog-to-basket", "type": "Navigation", "source": { "element": "catalog" }, "target": { "element": "basket" } },
                { "id": "basket-to-catalog", "type": "Navigation", "source": { "element": "basket" }, "target": { "element": "catalog" } },
                { "id": "login-to-catalog", "type": "Navigation", "source": { "element": "login" }, "target": { "element": "catalog" } },
                { "id": "basket-publishes", "type": "Publishes", "source": { "element": "basket-vm" }, "target": { "element": "count-changed" } },
                { "id": "catalog-vm-subscribes", "type": "Subscribes", "source": { "element": "catalog-vm" }, "target": { "element": "count-changed" } },
                { "id": "catalog-subscribes", "type": "Subscribes", "source": { "element": "catalog" }, "target": { "element": "count-changed" } }
              ]
            },
            "Codebase Structure": {
              "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
                "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] }{{optIn}} } }]
            }
          }
        }
        """)!;

    [Fact]
    public void A_MAUI_app_the_model_states_is_clean()
    {
        var result = Run(Model(), Code);

        Errors(result).ShouldBeEmpty(Describe(result));
        Verified(result, "component.kind").ShouldBe(7, Describe(result)); // AppShell, 4 pages, BadgeView, ProductTemplate
        Verified(result, "security").ShouldBe(7, Describe(result));
        Verified(result, "page.route").ShouldBe(4, Describe(result));
        Verified(result, "page.title").ShouldBe(1, Describe(result));
        Verified(result, "region.outlet").ShouldBe(2, Describe(result));
        Verified(result, "intent.uses").ShouldBe(9, Describe(result)); // 2 region and 2 view compositions, 4 view models, 1 service
        Verified(result, "intent.target").ShouldBe(4, Describe(result));
        Verified(result, "intent.navigates").ShouldBe(3, Describe(result));
        Verified(result, "entry.label").ShouldBe(1, Describe(result));
        Verified(result, "entry.icon").ShouldBe(1, Describe(result));
        Verified(result, "entry.key").ShouldBe(2, Describe(result));
        Verified(result, "appEvent.publishes").ShouldBe(1, Describe(result));
        Verified(result, "appEvent.subscribes").ShouldBe(2, Describe(result));
        Verified(result, "store.persisted").ShouldBe(1, Describe(result));
        Verified(result, "property.bindable").ShouldBe(1, Describe(result));
        Verified(result, "property.query").ShouldBe(1, Describe(result));
        Verified(result, "property.route").ShouldBe(1, Describe(result));
        Verified(result, "member.type").ShouldBe(6, Describe(result)); // the App Event's Value through ValueChangedMessage<int>, and ValidatableObject<T>'s
    }

    [Fact]
    public void Each_reading_is_at_the_code_that_states_it()
    {
        var result = Run(Model(), Code);

        Where(Claim(result, "shell", "component.kind")).ShouldBe("src/Shop/AppShell.xaml:2");
        Where(Claim(result, "filters", "page.route")).ShouldBe("src/Shop/AppShell.xaml.cs:20");
        Where(Claim(result, "catalog", "page.route")).ShouldBe("src/Shop/AppShell.xaml:14");
        Where(Claim(result, "main-region", "region.outlet")).ShouldBe("src/Shop/AppShell.xaml:13");
        Where(Claim(result, "basket-vm-uses", "intent.uses")).ShouldBe("src/Shop/Views/BasketView.xaml.cs:10");
        Where(Claim(result, "login-vm-uses", "intent.uses")).ShouldBe("src/Shop/Views/LoginView.xaml:8");
        Where(Claim(result, "catalog-to-basket", "intent.navigates")).ShouldBe("src/Shop/ViewModels/CatalogViewModel.cs:18");
        Where(Claim(result, "basket-publishes", "appEvent.publishes")).ShouldBe("src/Shop/ViewModels/BasketViewModel.cs:17");
        Where(Claim(result, "settings", "store.persisted")).ShouldBe("src/Shop/Services/SettingsService.cs:9");
    }

    [Fact]
    public void The_views_and_shell_markup_are_located_and_what_a_view_cant_state_is_never_asked_for()
    {
        const string optIn = """, "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" }, { "Rule": "optin/unmodelled.file" }, { "Rule": "optin/unmodelled.member" }, { "Rule": "optin/unmodelled.region" } ] }""";
        var result = Run(Model(optIn: optIn), Code);

        // The modelled views' markup and code (BadgeView's static TextProperty included), the Shell's items, and the app's own
        // and its resources' XAML report nothing; the unmodelled base classes and App are code the model doesn't describe.
        Errors(result).ShouldBeEmpty(Describe(result));
        result.Findings.Where(x => x.Rule.StartsWith("optin/", StringComparison.Ordinal) && x.Subject.File is { } file
                                   && (file.EndsWith(".xaml", StringComparison.Ordinal) || file.EndsWith(".xaml.cs", StringComparison.Ordinal) && !file.EndsWith("/App.xaml.cs", StringComparison.Ordinal)
                                       || file.EndsWith("/BadgeView.cs", StringComparison.Ordinal)))
            .ShouldBeEmpty(Describe(result));
        result.Findings.Where(x => x.Rule == "optin/unmodelled.region").ShouldBeEmpty(Describe(result));
        string.Join(", ", result.Findings.Where(x => x.Rule.StartsWith("optin/", StringComparison.Ordinal)).Select(x => x.Subject.File).Distinct().Order())
            .ShouldBe("src/Shop/App.xaml.cs, src/Shop/ViewModels/LoginViewModel.cs, src/Shop/ViewModels/ViewModelBase.cs, src/Shop/Views/ContentPageBase.cs", Describe(result));
    }

    // ---------------------------------------------------------------- one planted fact each

    [Fact]
    public void A_changed_registered_route_is_one_page_route_error()
    {
        var result = Run(Model(), With("src/Shop/AppShell.xaml.cs", x => x.Replace("RegisterRoute(\"Filter\"", "RegisterRoute(\"Filters\"")));

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/page.route");
        finding.Subject.Element.ShouldBe("filters");
    }

    [Fact]
    public void A_swapped_view_model_is_one_uses_error()
    {
        var result = Run(Model(), With("src/Shop/Views/FiltersView.xaml.cs", x => x.Replace("CatalogViewModel viewModel", "BasketViewModel viewModel")));

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/intent.uses");
        finding.Message.ShouldStartWith("`FiltersView` uses `CatalogViewModel`: in the model, not in the code. `FiltersView`'s binding context is `BasketViewModel`, and no field, property or constructor parameter of it is a `CatalogViewModel`");
    }

    [Fact]
    public void A_changed_navigation_through_the_view_models_service_is_one_navigates_error()
    {
        var result = Run(Model(), With("src/Shop/ViewModels/CatalogViewModel.cs", x => x.Replace("NavigateToAsync(\"Basket\")", "NavigateToAsync(\"Filter\")")));

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/intent.navigates");
        finding.Message.ShouldStartWith("`CatalogView` navigates to `BasketView`: in the model, not in the code. `BasketView` is at `/Basket`, and `CatalogView` and its binding context `CatalogViewModel` navigate to `\"Filter\"`.");
    }

    [Fact]
    public void A_template_the_view_no_longer_places_is_one_composition_error()
    {
        var result = Run(Model(), With("src/Shop/Views/CatalogView.xaml", x => x.Replace("<templates:ProductTemplate />", "<Label Text=\"{Binding Name}\" />")));

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/intent.uses");
        finding.Message.ShouldStartWith("`CatalogView` uses `ProductTemplate`: in the model, not in the code. `src/Shop/Views/CatalogView.xaml` uses `BadgeView`, and no element, `DataTemplate` or `x:Type` of it is a `ProductTemplate`.");
    }

    [Fact]
    public void A_message_no_longer_sent_is_one_publishes_error()
    {
        var result = Run(Model(), With("src/Shop/ViewModels/BasketViewModel.cs", x => x.Replace("WeakReferenceMessenger.Default.Send(new ProductCountChangedMessage(0));", "")));

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/appEvent.publishes");
        finding.Message.ShouldStartWith("`BasketViewModel` publishes `ProductCountChangedMessage`: in the model, not in the code. `BasketViewModel` sends no message on a messenger.");
    }

    [Fact]
    public void A_message_sent_through_the_codes_own_helper_or_made_by_the_messenger_is_published()
    {
        var helper = Run(Model(), With("src/Shop/ViewModels/BasketViewModel.cs", x => x.Replace("WeakReferenceMessenger.Default.Send(new ProductCountChangedMessage(0));", "Notify(new ProductCountChangedMessage(0));")
            .Replace("[RelayCommand]", "private void Notify<T>(T message) where T : class => WeakReferenceMessenger.Default.Send(message);\n\n    [RelayCommand]")));
        var made = Run(Model(), With("src/Shop/ViewModels/BasketViewModel.cs", x => x.Replace("WeakReferenceMessenger.Default.Send(new ProductCountChangedMessage(0));", "WeakReferenceMessenger.Default.Send<ProductCountChangedMessage>();")));

        Errors(helper).ShouldBeEmpty(Describe(helper));
        Errors(made).ShouldBeEmpty(Describe(made));
    }

    [Fact]
    public void A_registration_for_another_message_is_one_subscribes_error_and_a_recipient_subscribes()
    {
        var other = Run(Model(), With("src/Shop/Views/CatalogView.xaml.cs", x => x.Replace("Register<CatalogView, ProductCountChangedMessage>", "Register<CatalogView, ValueChangedMessage<int>>")));
        var recipient = Run(Model(), With("src/Shop/Views/CatalogView.xaml.cs", x => x.Replace("public partial class CatalogView", "public partial class CatalogView : IRecipient<ProductCountChangedMessage>")
            .Replace("WeakReferenceMessenger.Default.Register<CatalogView, ProductCountChangedMessage>(this, (recipient, message) => { });", "WeakReferenceMessenger.Default.RegisterAll(this);")
            .Replace("protected override void OnAppearing()", "public void Receive(ProductCountChangedMessage message) { }\n\n    protected override void OnAppearing()")));

        var finding = Single(other);
        finding.Rule.ShouldBe("claim/appEvent.subscribes");
        finding.Message.ShouldStartWith("`CatalogView` subscribes to `ProductCountChangedMessage`: in the model, not in the code. `CatalogView` registers `WeakReferenceMessenger.Default.Register<CatalogView, ValueChangedMessage<int>>(");
        Errors(recipient).ShouldBeEmpty(Describe(recipient));
    }

    [Fact]
    public void A_changed_tab_title_is_one_entry_label_error()
    {
        var result = Run(Model(catalogLabel: "SHOP"), Code);

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/entry.label");
        finding.Message.ShouldBe("`Main`'s entry for `CatalogView` Label: model `SHOP`, code `CATALOG`. The `ShellContent` showing it has `Title=\"CATALOG\"`. The model states it in the Composition's Entry Label.");
    }

    [Fact]
    public void A_template_modelled_as_a_page_is_one_kind_error()
    {
        var result = Run(Model(productType: "Page"), Code);

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/component.kind");
        finding.Message.ShouldBe("`ProductTemplate` component kind: model `Page`, code `Component`. `ProductTemplate` derives from `ContentView` (`src/Shop/Views/Templates/ProductTemplate.xaml`). Model it as a Component.");
    }

    [Fact]
    public void A_bindable_property_the_model_doesnt_mark_is_one_bindable_error()
    {
        var result = Run(Model(bindable: false), Code);

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/property.bindable");
        finding.Message.ShouldBe("`BadgeView.Text` bindable: model `false`, code `true`. `TextProperty` is a BindableProperty, so a parent binds it. Mark it [Bindable].");
    }

    [Fact]
    public void A_store_saving_nothing_on_the_device_is_one_persisted_error_and_one_saving_is_an_unmodelled_value()
    {
        var plain = With("src/Shop/Services/SettingsService.cs", x => x.Replace("Preferences.Get(IdUseMocks, true)", "_useMocks").Replace("Preferences.Set(IdUseMocks, value)", "_useMocks = value")
            .Replace("private const", "private bool _useMocks;\n    private const"));
        var result = Run(Model(), plain);
        var unset = Run(Model(persisted: false, optIn: """, "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ] }"""), Code);

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/store.persisted");
        finding.Message.ShouldStartWith("`SettingsService` persisted: model `true`, code `false`. `SettingsService` reads and writes neither `Preferences` nor `SecureStorage`");
        var unmodelled = unset.Findings.Single(x => x.Rule == "optin/unmodelled.value");
        unmodelled.Subject.File.ShouldBe("src/Shop/Services/SettingsService.cs");
        unmodelled.Message.ShouldStartWith("`SettingsService` persisted: the code states `true`, which the model leaves unset");
    }

    [Fact]
    public void A_member_typed_by_its_classs_type_parameter_is_at_least_an_object_and_any_other_type_is_an_error()
    {
        var result = Run(Model(valueType: "string"), Code);

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/member.type");
        finding.Message.ShouldStartWith("`ValidatableObject.Value` type: model `string`, code `T`.");
    }

    [Fact]
    public void A_renamed_shell_item_is_one_region_error_and_its_pages_route_and_its_composition_follows_it()
    {
        var result = Run(Model(), With("src/Shop/AppShell.xaml", x => x.Replace("<FlyoutItem>", "<FlyoutItem Route=\"Start\">")));

        var findings = Errors(result);
        string.Join(", ", findings.Select(x => x.Rule).Order()).ShouldBe("claim/page.route, claim/region.outlet", Describe(result));
        findings.Single(x => x.Rule == "claim/region.outlet").Message.ShouldStartWith("`AppShell` region `Login`: model `Login`, code `Start`. `AppShell` has the items `Start`, `Main`");
        Claim(result, "login-shows", "intent.uses").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void A_view_of_another_xaml_framework_isnt_mauis()
    {
        var code = new Dictionary<string, string>
        {
            ["src/Desk/Desk.csproj"] = """<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><UseWPF>true</UseWPF></PropertyGroup></Project>""",
            ["src/Desk/OrdersPage.xaml"] = """
                <Page x:Class="Desk.OrdersPage" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                    <Grid />
                </Page>
                """,
            ["src/Desk/OrdersPage.xaml.cs"] = "namespace Desk; public partial class OrdersPage { }"
        };
        var model = JsonNode.Parse("""
            {
              "designers": {
                "User Interface": { "packages": [{ "id": "ui", "name": "Desk", "type": "User Interface Package",
                  "elements": [ { "id": "orders", "type": "Page", "name": "OrdersPage", "stereotypes": { "Page": { } } } ] }] },
                "Codebase Structure": { "packages": [{ "id": "root", "name": "Desk", "type": "Root Folder",
                  "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }] }
              }
            }
            """)!;

        var result = Run(model, code);

        Claim(result, "orders", "type.exists").Outcome.ShouldBe("verified");
        Claim(result, "orders", "component.kind").Outcome.ShouldBe("cantVerify", Describe(result));
        Claim(result, "orders", "component.kind").Reason.ShouldBe("noRule");
    }

    /// <summary>
    /// eShop's real ClientApp, read in place: every XAML view is read and joined to its code-behind as a MAUI view, and the
    /// Shell's items and registered routes reach their pages. Skipped where eShop10 isn't on the machine.
    /// </summary>
    [Fact]
    public void Real_eShop_ClientApp_views_are_read_as_maui_views_with_their_shell_and_routes()
    {
        var codebase = Codebases.Get("eShop10");
        if (!codebase.IsPresent)
        {
            Assert.Skip($"eShop10 isn't on this machine (looked in {codebase.Root}).");
        }

        var app = Path.Combine(codebase.Root, "src", "ClientApp");
        var files = Directory.EnumerateFiles(app, "*", SearchOption.AllDirectories)
            .Select(x => Path.GetRelativePath(codebase.Root, x).Replace('\\', '/'))
            .Where(x => !x.Split('/').Any(s => s is "bin" or "obj") && (x.EndsWith(".cs") || x.EndsWith(".xaml") || x.EndsWith(".csproj")))
            .Order(StringComparer.Ordinal)
            .ToList();
        var graph = new Intent.Modules.Common.DriftEngine.Code.CodeGraph();
        var xaml = new Intent.Modules.Common.DriftEngine.Stacks.Xaml.XamlExtension();
        foreach (var language in new Intent.Modules.Common.DriftEngine.Stacks.CSharp.CSharpExtension().Languages.Concat(xaml.Languages))
        {
            language.Read(codebase.Root, files.Where(language.Reads).ToList(), graph, TestContext.Current.CancellationToken);
        }

        foreach (var reader in xaml.Readers)
        {
            reader.Read(graph);
        }

        var site = Intent.Modules.Common.DriftEngine.Stacks.Maui.MauiSite.Of(graph);
        var views = Intent.Modules.Common.DriftEngine.Stacks.Xaml.XamlSyntax.Of(graph)!.Documents.Values.ToList();

        graph.Failures.Where(x => x.File.EndsWith(".xaml", StringComparison.Ordinal)).ShouldBeEmpty();
        views.Count.ShouldBe(18); // 22 XAML files: App.xaml, the Windows head's App.xaml and two resource dictionaries add nothing
        views.Where(x => x.Type is null || !site.IsView(x.Type)).Select(x => x.Path).ShouldBeEmpty();
        string.Join(", ", site.ShellItems.Select(x => $"{x.Name}: {string.Join(" ", x.Contents.Select(c => $"{c.Path}={c.Type?.Name}"))}"))
            .ShouldBe("Login: Login=LoginView, Main: Main/Catalog=CatalogView Main/Map=MapView Main/Profile=ProfileView");
        string.Join(", ", site.RegisteredRoutes.Select(x => $"{x.Route}={x.Type?.Name}"))
            .ShouldBe("Filter=FiltersView, ViewCatalogItem=CatalogItemView, Basket=BasketView, Settings=SettingsView, OrderDetail=OrderDetailView, Checkout=CheckoutView");
    }

    // ------------------------------------------------------------------ harness

    private static Dictionary<string, string> With(string path, Func<string, string> change)
    {
        var code = new Dictionary<string, string>(Code);
        var changed = change(code[path]);
        changed.ShouldNotBe(code[path], $"The plant changed nothing in {path}.");
        code[path] = changed;
        return code;
    }

    /// <summary>The findings these tests are about: claims, and the opt-in rules on the views' markup.</summary>
    private static List<Finding> Errors(RunResult result) =>
        result.Findings.Where(x => x.Rule.StartsWith("claim/", StringComparison.Ordinal) || x.Rule == "engine/noVocabulary"
                                   || x.Rule.StartsWith("optin/", StringComparison.Ordinal) && x.Subject.File?.EndsWith(".xaml", StringComparison.Ordinal) == true).ToList();

    private static Finding Single(RunResult result)
    {
        var findings = Errors(result);
        findings.Count.ShouldBe(1, Describe(result));
        return findings[0];
    }

    private static int Verified(RunResult result, string kind) => result.Claims.Count(x => x.Kind == kind && x.Outcome == "verified");

    private static Claim Claim(RunResult result, string element, string kind) => result.Claims.Single(x => x.Element == element && x.Kind == kind);

    private static string Where(Claim claim) => $"{claim.Reading?.At?.File}:{claim.Reading?.At?.Line}";

    private static string Describe(RunResult result) =>
        "\nClaims:\n" + string.Join("\n", result.Claims.Select(x => $"  {x.Kind} {x.Element} expected={x.Expected} -> {x.Outcome} {x.Reason} {x.Reading?.Value} {x.Reading?.At?.File}:{x.Reading?.At?.Line}"))
        + "\nFindings:\n" + string.Join("\n", result.Findings.Select(x => $"  {x.Rule} [{x.Severity}] {x.Subject.File}:{x.Subject.Line} {x.Subject.Element} {x.Message}"));

    private static RunResult Run(JsonNode model, IReadOnlyDictionary<string, string> code)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-maui-").FullName;
        try
        {
            foreach (var (path, source) in code)
            {
                var full = Path.Combine(root, path);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, source);
            }

            var extensions = StockExtensions.All.Where(x => !x.ClaimReaders.Any())
                .Concat(new[] { "intent.blazor", "intent.mvc", "intent.mvvmtoolkit", "intent.maui" }.Select(x => StockExtensions.Find(x)!)).ToList();
            var designers = FixtureModel.Build(model, out var references);
            return DriftEngine.Run(new DriftInput(designers, root, [Pack.Stock("intent.core")!, Pack.Stock("intent.ui")!], extensions) { References = references });
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
