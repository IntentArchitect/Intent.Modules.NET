using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;



internal class MudBlazorComponentConfigurators
{
    // https://mudblazor.com/components/card
    public class Card
    {
        public static void CardHeaderContent(IRazorConfigurator configurator)
        {
            configurator.AllowMatchByTagNameOnly("CardHeaderContent");
        }

        public static void MudCard(IRazorConfigurator configurator) { }

        public static void MudCardActions(IRazorConfigurator configurator)
        {
            configurator.AllowMatchByTagNameOnly("MudCardActions");
        }

        public static void MudCardContent(IRazorConfigurator configurator)
        {
            configurator.AllowMatchByTagNameOnly("MudCardContent");
        }

        public static void MudCardHeader(IRazorConfigurator configurator)
        {
            configurator.AllowMatchByTagNameOnly("MudCardHeader");
        }
    }

    public class DataGrid
    {
        public static void CellTemplate(IRazorConfigurator configurator)
        {
            configurator.AllowMatchByTagNameOnly("CellTemplate");
        }

        public static void Columns(IRazorConfigurator configurator)
        {
            configurator.AllowMatchByTagNameOnly("Columns");
        }

        public static void MudDataGrid(IRazorConfigurator configurator) { }

        public static void MudDataGridPager(IRazorConfigurator configurator)
        {
            configurator.AllowMatchByTagNameOnly("MudDataGridPager");
        }

        public static void PropertyColumn(IRazorConfigurator configurator) { }

        public static void TemplateColumn(IRazorConfigurator configurator) { }
    }

    public class Dialog
    {
        public static void DialogActions(IRazorConfigurator configurator)
        {
            configurator.AllowMatchByTagNameOnly("DialogActions");
        }

        public static void MudDialog(IRazorConfigurator configurator) { }

        public static void MudDialogProvider(IRazorConfigurator configurator)
        {
            configurator.AllowMatchByTagNameOnly("MudDialogProvider");
        }
    }

    public static void PagerContent(IRazorConfigurator configurator)
    {
        configurator.AllowMatchByTagNameOnly("PagerContent");
    }

    public static void ToolBarContent(IRazorConfigurator configurator)
    {
        configurator.AllowMatchByTagNameOnly("ToolBarContent");
    }

    public class Table
    {
        public static void HeaderContent(IRazorConfigurator configurator)
        {
            configurator.AllowMatchByTagNameOnly("HeaderContent");
        }

        public static void MudTable(IRazorConfigurator configurator) { }

        public static void MudTd(IRazorConfigurator configurator) { }

        public static void MudTh(IRazorConfigurator configurator) { }

        public static void RowTemplate(IRazorConfigurator configurator)
        {
            configurator.AllowMatchByTagNameOnly("RowTemplate");
        }
    }

    public static void MudAlert(IRazorConfigurator configurator) { }

    public static void MudAppBar(IRazorConfigurator configurator) { }

    public static void MudButton(IRazorConfigurator configurator)
    {
        configurator.AddTagNameAttributeMatch("MudButton", "Href");
    }

    public static void MudCheckBox(IRazorConfigurator configurator) { }

    public static void MudContainer(IRazorConfigurator configurator) { }

    public static void MudDatePicker(IRazorConfigurator configurator)
    {
        configurator.AddTagNameAttributeMatch("MudDatePicker", "@bind-Date");
    }

    public static void MudDrawer(IRazorConfigurator configurator)
    {
        // Although possible to have more than one, I am guessing it's highly unusual.
        configurator.AllowMatchByTagNameOnly("MudDrawer");
    }

    public static void MudForm(IRazorConfigurator configurator) { }

    public static void MudGrid(IRazorConfigurator configurator) { }

    public static void MudIcon(IRazorConfigurator configurator) { }

    public static void MudIconButton(IRazorConfigurator configurator)
    {
        configurator.AddTagNameAttributeMatch("MudIconButton", "Href");
    }

    public static void MudImage(IRazorConfigurator configurator)
    {
        configurator.AddTagNameAttributeMatch("MudImage", "Src");
    }

    public static void MudItem(IRazorConfigurator configurator) { }

    public static void MudLayout(IRazorConfigurator configurator)
    {
        configurator.AllowMatchByTagNameOnly("MudLayout");
    }

    public static void MudMainContent(IRazorConfigurator configurator)
    {
        configurator.AllowMatchByTagNameOnly("MudMainContent");
    }

    public static void MudMenu(IRazorConfigurator configurator)
    {
        configurator.AddTagNameAttributeMatch("MudMenu", "Label");
    }

    public static void MudMenuItem(IRazorConfigurator configurator)
    {
        configurator.AddTagNameAttributeMatch("MudMenuItem", "Href");
    }

    public static void MudNavMenu(IRazorConfigurator configurator)
    {
        configurator.AllowMatchByTagNameOnly("MudNavMenu");
    }

    public static void MudPagination(IRazorConfigurator configurator)
    {
        configurator.AddTagNameAttributeMatch("MudPagination", "@bind-Selected");
    }

    public static void MudPopoverProvider(IRazorConfigurator configurator)
    {
        configurator.AllowMatchByTagNameOnly("MudPopoverProvider");
    }

    public static void MudProgressCircular(IRazorConfigurator configurator) { }

    public static void MudProgressLinear(IRazorConfigurator configurator)
    {
        configurator.AddTagNameAttributeMatch("MudProgressCircular", "MudProgressLinear");
    }

    public static void MudSelect(IRazorConfigurator configurator) { }

    public static void MudSelectItem(IRazorConfigurator configurator) { }

    public static void MudSnackbarProvider(IRazorConfigurator configurator)
    {
        configurator.AllowMatchByTagNameOnly("MudSnackbarProvider");
    }

    public static void MudSpacer(IRazorConfigurator configurator) { }

    public static void MudStack(IRazorConfigurator configurator) { }

    public static void MudText(IRazorConfigurator configurator) { }

    public static void MudTextField(IRazorConfigurator configurator) { }

    public static void MudThemeProvider(IRazorConfigurator configurator)
    {
        configurator.AllowMatchByTagNameOnly("MudThemeProvider");
    }

    public static void TitleContent(IRazorConfigurator configurator)
    {
        configurator.AllowMatchByTagNameOnly("TitleContent");
    }
}