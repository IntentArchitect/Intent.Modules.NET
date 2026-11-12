using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.UI.Guardrails.Contracts.Templates.UserInterfaceDesigner1Reference
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UserInterfaceDesigner1ReferenceTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.UI.Guardrails.Contracts.UserInterfaceDesigner1Reference";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UserInterfaceDesigner1ReferenceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("user-interface-designer-1", relativeLocation: "reverse-engineer-ui/references")
                .FromMarkdown("""
                    # User Interface designer 1.x: modelling an existing UI

                    For Intent.Modelers.UI 1.x, with Intent.Modelers.UI.Core for `Show Dialog`; this module installs both. It covers how each concept the reverse-engineer-ui skill models is expressed in this designer, and the scripts that create it.

                    ## Concepts

                    | Concept | How to model it |
                    | --- | --- |
                    | Package | The application's `User Interface Package`. If there is none, create one named `{ApplicationName}.UI`; if there are several, ask which to use |
                    | Folder | `Folder`, named as the code's folder |
                    | Page | `Component` with the `Page` stereotype: `Route` as the code states it, without the leading `/` (`orders/{Id:int}`); a `:name` parameter (Angular, React Router) as `{name}`. `Title` is optional |
                    | Dialog | `Component` with the `Dialog` stereotype |
                    | Component | `Component` |
                    | Layout | `Layout` |
                    | Security | `Secured` on the Component: `Roles` and `Policy` as comma-separated names, in the code's order. Leave both empty for a page that only requires a signed-in user |
                    | Backend call | `Call Service Operation Action` from the Component or Layout itself to the Command, Query or Service operation. Never from a Component Operation, and with no Map Invocation or Map Response mapping |
                    | Navigation | `Navigation` from the Component or Layout to the page. One association per pair; a page each navigates to from the other is one bidirectional association |
                    | Dialog opened | `Show Dialog` from the Component to the dialog |
                    | Component rendered | `Composition` from the Component or Layout to the component it renders |

                    The check also reads interactions an older model put on a Component Operation, or a Layout's header, body, sider or footer, as the component's own, and a `Display Component` in a view as a component rendered. Model new ones as the table says.

                    ## Reference the backend's Services package

                    The calls' targets live in a Services designer. When they are in another application, or in this application's Services package, add that package as a reference to the User Interface package first:

                    ```js
                    const cands = await getAvailablePackageReferences();
                    const services = cands.find(c => c.name === "Acme.Ordering.Services");
                    const ui = lookupPackage("Acme.Web.UI");
                    if (!services || !ui) throw new Error(`Missing - services: ${!!services}, ui: ${!!ui}`);
                    await ui.addReference(services.absolutePath, services.sourceType === "module" ? services.source : undefined);
                    ```

                    The designer's own rules require a Command, Query or operation in another application to be exposed over HTTP there (its `Http Settings`); one the backend serves over HTTP already is.

                    ## Scripts

                    One script per area, creating its folders and components first, then the associations between them:

                    ```js
                    const ui = lookupPackage("Acme.Web.UI");
                    const checkoutFolder = createElementUnder(ui, "Folder", "Checkout");

                    const checkout = createElementUnder(checkoutFolder, "Component", "Checkout");
                    checkout.ensureStereotype("Page").setProperty("Route", "checkout");
                    const secured = checkout.ensureStereotype("Secured");
                    secured.setProperty("Roles", "Customer,Admin");

                    const confirm = createElementUnder(checkoutFolder, "Component", "ConfirmOrderDialog");
                    confirm.ensureStereotype("Dialog");

                    const summary = createElementUnder(checkoutFolder, "Component", "OrderSummary");

                    const layout = createElementUnder(createElementUnder(ui, "Folder", "Layout"), "Layout", "MainLayout");

                    const orders = lookupByPath("Acme.Web.UI/User/Orders");
                    const createOrder = lookupByName("CreateOrderCommand");
                    if (!orders || !createOrder) throw new Error(`Missing - Orders: ${!!orders}, CreateOrderCommand: ${!!createOrder}`);

                    createAssociation("Call Service Operation Action", checkout.id, createOrder.id);
                    createAssociation("Navigation", checkout.id, orders.id);
                    createAssociation("Show Dialog", checkout.id, confirm.id);
                    createAssociation("Composition", checkout.id, summary.id);
                    createAssociation("Navigation", layout.id, checkout.id);
                    return [checkout, confirm, summary, layout].map(x => x.getName());
                    ```

                    - Apply `Page` or `Dialog` before creating an association to the element: `Navigation` only accepts a page, and `Show Dialog` only a dialog.
                    - A new Component gets a `Component View` and the `Composable` trait by itself. Leave the view empty.
                    - **A mapping dialog.** If creating a `Call Service Operation Action` opens a mapping dialog, close it with `resolve_designer_dialog`, leaving the mapping empty. The call is modelled; its mapping isn't.
                    - Read the script's `changes`: each element appears once as `created`. A name the designer changed (`Checkout1`, a casing change) means a duplicate or a naming convention: fix it before carrying on.

                    ## Diagrams

                    One diagram per area (a folder of pages), named after it, showing its pages, dialogs and components, the operations they call on the right, and their navigations. The designer's own diagram rules apply.

                    ## Don't model

                    - The `Component View` and its widgets (Button, Form, Table, Text Input and the rest).
                    - Component Operations, Properties (route and query parameters included: the route carries them), Event Emitters and Model Definitions.
                    - Any mapping, on a call or anywhere else.
                    - Several elements for one component, or the same chrome on every page.
                    """);
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
