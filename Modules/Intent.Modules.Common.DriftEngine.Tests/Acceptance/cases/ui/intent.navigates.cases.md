# intent.navigates

A component navigates to a page: a Navigation in the User Interface designer, or a Navigates To association in the Java spike's Front End designer. It is a `presence` kind, an existence kind, `stated: always`, and the claim sits on the association that states it.

The Blazor cases use the `intent.ui` pack's vocabulary, and the Blazor reader reads the navigation: an `href`, a form `action` or a `NavigateTo` whose URI is the target's route. A navigation whose URI is computed is can't verify, `unreadable`. The Vue cases use the Java spike's house pack, which gives the Front End designer's vocabulary too. The `mvc-*` cases read ASP.NET Core MVC views with the MVC reader (`intent.mvc`): a link reaching an action that takes a GET and renders the target's view, or the target Razor Page.

## verified-href

The cart page links to checkout with an `href`, and the model has a Navigation from the cart page to the Checkout page.

```json case
{ "phase": 6, "kinds": ["intent.navigates"], "traces": ["ESC-FE-32"], "requirements": ["R2", "R8", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "Pages",
            "children": [
              { "id": "cart-page", "type": "Component", "name": "CartPage",
                "stereotypes": { "Page": { "Route": "/cart" } } },
              { "id": "checkout-page", "type": "Component", "name": "Checkout",
                "stereotypes": { "Page": { "Route": "/checkout" } } }
            ] }
        ]
      }],
      "associations": [
        { "id": "cart-navigates-to-checkout", "type": "Navigation",
          "source": { "element": "cart-page" }, "target": { "element": "checkout-page", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@page "/cart"
@inject BasketState Basket

<PageTitle>Shopping Bag | AdventureWorks</PageTitle>

<div class="cart-summary">
    <a href="checkout" class="button button-primary">Check out</a>
</div>
```

```razor file src/WebApp/Components/Pages/Checkout/Checkout.razor
@page "/checkout"
@inject BasketState Basket

<PageTitle>Checkout | AdventureWorks</PageTitle>
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "cart-navigates-to-checkout", "outcome": "verified",
      "expected": "present", "actual": "present", "at": "src/WebApp/Components/Pages/Cart/CartPage.razor:7" }
  ]
}
```

## verified-navigate-to

eShopCustom's checkout page sends the user to their orders after placing one. The model's Navigation starts at the page's `HandleSubmitAsync` Component Operation, and the code's `NavigateTo` is in that method.

```json case
{ "phase": 6, "base": "verified-href", "kinds": ["intent.navigates"], "traces": ["ESC-FE-32"], "requirements": ["R2", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "Pages",
            "children": [
              { "id": "checkout-page", "type": "Component", "name": "Checkout",
                "stereotypes": { "Page": { "Route": "/checkout" } },
                "children": [
                  { "id": "checkout-handle-submit", "type": "Component Operation", "name": "HandleSubmitAsync" }
                ] },
              { "id": "orders-page", "type": "Component", "name": "Orders",
                "stereotypes": { "Page": { "Route": "/user/orders" } } }
            ] }
        ]
      }],
      "associations": [
        { "id": "checkout-navigates-to-orders", "type": "Navigation",
          "source": { "element": "checkout-handle-submit" }, "target": { "element": "orders-page", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```razor file src/WebApp/Components/Pages/Checkout/Checkout.razor
@page "/checkout"
@inject NavigationManager Nav
@inject BasketState Basket

<PageTitle>Checkout | AdventureWorks</PageTitle>

<EditForm Model="@Info" FormName="checkout" OnSubmit="@HandleSubmitAsync">
    <button type="submit" class="button button-primary">Place order</button>
</EditForm>

@code {
    private async Task HandleSubmitAsync()
    {
        await Basket.CheckoutAsync(Info.CreateOrderRequest());
        Nav.NavigateTo("user/orders");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "checkout-navigates-to-orders", "outcome": "verified",
      "at": "src/WebApp/Components/Pages/Checkout/Checkout.razor:15" }
  ]
}
```

## contradicted-missing

eShopCustom's `navigation.missing`: the cart page has lost its link to checkout, and the model's Navigation still says it navigates there. The page is found and nothing in it navigates to the checkout route, so its silence says `absent`.

```json case
{ "phase": 6, "base": "verified-href", "kinds": ["intent.navigates"], "cases": ["engine/silence"], "traces": ["ESC-FE-32"], "requirements": ["R3", "R8", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@page "/cart"
@inject BasketState Basket

<PageTitle>Shopping Bag | AdventureWorks</PageTitle>

<div class="cart-summary">
    <a href="" class="cart-summary-link">Continue shopping</a>
</div>
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "cart-navigates-to-checkout", "outcome": "contradicted",
      "expected": "present", "actual": "none", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "severity": "error", "subject": "cart-navigates-to-checkout", "messageContains": "`Checkout` navigates: in the model, not in the code." }
  ]
}
```

## cant-verify-computed-uri

The cart page's link builds its URI in code, so the reader can't tell where it goes. The claim is can't verify, `unreadable`, at the link's line, rather than passing or failing on a guess.

```json case
{ "phase": 6, "base": "verified-href", "kinds": ["intent.navigates"], "cases": ["engine/unreadable"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@page "/cart"
@inject BasketState Basket

<PageTitle>Shopping Bag | AdventureWorks</PageTitle>

<div class="cart-summary">
    <a href="@CheckoutUri" class="button button-primary">Check out</a>
</div>

@code {
    private string CheckoutUri => Basket.IsEmpty ? "" : "checkout";
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "cart-navigates-to-checkout", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/WebApp/Components/Pages/Cart/CartPage.razor:7" }
  ]
}
```

## java-router-link

The Java spike's cart page links to checkout with a `RouterLink`, and the model has a Navigates To from the cart page to the checkout page. The checkout page has no link back to the cart, so its Navigates To is contradicted. No stock pack knows the Front End designer, so its house pack extends `intent.ui`: it claims where each page is (`type.exists` with its Implementation Path) and the stock `intent.navigates` on its own association, with the target's route as the `Route` binding, and the stock Vue reader (`intent.vue`) reads the page's `to` attributes and `router.push` calls.

```json case
{ "phase": 6, "kinds": ["intent.navigates"], "traces": ["JAV-FE-06"], "requirements": ["R2", "R3", "R28", "R38"], "packs": ["intent.core", "intent.ui", "house.shop"], "language": "typescript" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.ui", "version": "^1.0.0" }],
  "languages": ["typescript"],
  "designers": ["Front End"],
  "vocabulary": {
    "Front End": {
      "entries": [
        {
          "kind": "type.exists",
          "for": { "element": "Page" },
          "value": "self",
          "bindings": { "Name": "name", "Component": "name", "Path": "stereotype('Implementation').Path" },
          "own": ["Name"],
          "source": "the page's Implementation Path"
        },
        {
          "kind": "intent.navigates",
          "for": { "association": "Navigates To" },
          "value": "'present'",
          "bindings": {
            "Page": "target.element.name",
            "Route": "target.element.stereotype('Route').Path"
          },
          "own": ["Page"],
          "source": "the Navigates To association"
        }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Front End": {
      "packages": [{
        "id": "storefront", "name": "Storefront",
        "elements": [
          { "id": "cart-page", "type": "Page", "name": "CartPage",
            "stereotypes": {
              "Route": { "Path": "/cart" },
              "Implementation": { "Path": "src/pages/CartPage.vue" }
            } },
          { "id": "checkout-page", "type": "Page", "name": "CheckoutPage",
            "stereotypes": {
              "Route": { "Path": "/checkout" },
              "Implementation": { "Path": "src/pages/CheckoutPage.vue" }
            } }
        ]
      }],
      "associations": [
        { "id": "cart-navigates-to-checkout", "type": "Navigates To",
          "source": { "element": "cart-page" }, "target": { "element": "checkout-page" } },
        { "id": "checkout-navigates-to-cart", "type": "Navigates To",
          "source": { "element": "checkout-page" }, "target": { "element": "cart-page" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```vue file src/pages/CartPage.vue
<script setup lang="ts">
import { RouterLink } from 'vue-router';
import { useCartStore } from '../stores/cart';

const cart = useCartStore();
</script>

<template>
    <section class="cart">
        <RouterLink to="/" class="btn">Browse the catalog</RouterLink>
        <RouterLink to="/checkout" class="btn btn-block" data-testid="proceed-to-checkout">Proceed to checkout</RouterLink>
    </section>
</template>
```

```vue file src/pages/CheckoutPage.vue
<script setup lang="ts">
import { useRouter } from 'vue-router';

const router = useRouter();

async function placed(orderId: string) {
    await router.push(`/orders/${orderId}`);
}
</script>

<template>
    <form class="checkout" @submit.prevent="placed('')"></form>
</template>
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "cart-navigates-to-checkout", "outcome": "verified", "at": "src/pages/CartPage.vue:11" },
    { "kind": "intent.navigates", "element": "checkout-navigates-to-cart", "outcome": "contradicted", "expected": "present" }
  ]
}
```

## vue-standard-navigation

The standard User Interface designer (2.0.0): a Navigation from the layout to the catalog page, whose route is the root `/`, and from the checkout page to the order page at `/orders/{id}`. The stock Vue reader takes each target's routes from its `page.route` claim and reads the source's `<RouterLink to>` and `router.push(...)`: the layout's `to="/"` goes to the root, and the checkout's `` router.push(`/orders/${orderId}`) `` to the order page, its value standing for the `{id}` segment. The checkout page has no link back to the catalog, so that Navigation is contradicted.

```json case
{ "phase": 6, "kinds": ["intent.navigates"], "requirements": ["R2", "R3", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "app", "type": "Layout", "name": "App" },
          { "id": "pages", "type": "Folder", "name": "pages", "children": [
            { "id": "catalog-page", "type": "Page", "name": "CatalogPage", "stereotypes": { "Page": { "Route": "/" } } },
            { "id": "checkout-page", "type": "Page", "name": "CheckoutPage", "stereotypes": { "Page": { "Route": "/checkout" } } },
            { "id": "order-page", "type": "Page", "name": "OrderPage", "stereotypes": { "Page": { "Route": "/orders/{id}" } } }
          ] }
        ]
      }],
      "associations": [
        { "id": "app-to-catalog", "type": "Navigation", "source": { "element": "app" }, "target": { "element": "catalog-page" } },
        { "id": "checkout-to-order", "type": "Navigation", "source": { "element": "checkout-page" }, "target": { "element": "order-page" } },
        { "id": "checkout-to-catalog", "type": "Navigation", "source": { "element": "checkout-page" }, "target": { "element": "catalog-page" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```typescript file src/router.ts
import type { RouteRecordRaw } from 'vue-router';
import CatalogPage from './pages/CatalogPage.vue';
import CheckoutPage from './pages/CheckoutPage.vue';
import OrderPage from './pages/OrderPage.vue';

export const routes: RouteRecordRaw[] = [
    { path: '/', component: CatalogPage },
    { path: '/checkout', component: CheckoutPage },
    { path: '/orders/:id', component: OrderPage },
];
```

```vue file src/App.vue
<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router';
</script>

<template>
    <RouterLink to="/" class="brand">eShop</RouterLink>
    <RouterView />
</template>
```

```vue file src/pages/CatalogPage.vue
<script setup lang="ts">
const title = 'Catalog';
</script>
```

```vue file src/pages/CheckoutPage.vue
<script setup lang="ts">
import { useRouter } from 'vue-router';

const router = useRouter();

async function placed(orderId: string) {
    await router.push(`/orders/${orderId}`);
}
</script>
```

```vue file src/pages/OrderPage.vue
<script setup lang="ts">
const title = 'Order';
</script>
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "app-to-catalog", "outcome": "verified", "at": "src/App.vue:6" },
    { "kind": "intent.navigates", "element": "checkout-to-order", "outcome": "verified", "at": "src/pages/CheckoutPage.vue:7" },
    { "kind": "intent.navigates", "element": "checkout-to-catalog", "outcome": "contradicted", "expected": "present" }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "severity": "error", "subject": "checkout-to-catalog" }
  ]
}
```

## react-standard-navigation

The same Navigations read against React by the stock React reader. It reads the source component's `<Link to>` and its code's `navigate(...)` from react-router's `useNavigate()`: the layout's `to="/"` goes to the root, and the checkout's `` navigate(`/orders/${orderId}`) ``, in a handler the component declares, to the order page, its value standing for the `{id}` segment. The checkout page has no link back to the catalog, so that Navigation is contradicted.

```json case
{ "phase": 6, "kinds": ["intent.navigates"], "requirements": ["R2", "R3", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "app", "type": "Layout", "name": "App" },
          { "id": "pages", "type": "Folder", "name": "pages", "children": [
            { "id": "catalog-page", "type": "Page", "name": "CatalogPage", "stereotypes": { "Page": { "Route": "/" } } },
            { "id": "checkout-page", "type": "Page", "name": "CheckoutPage", "stereotypes": { "Page": { "Route": "/checkout" } } },
            { "id": "order-page", "type": "Page", "name": "OrderPage", "stereotypes": { "Page": { "Route": "/orders/{id}" } } }
          ] }
        ]
      }],
      "associations": [
        { "id": "app-to-catalog", "type": "Navigation", "source": { "element": "app" }, "target": { "element": "catalog-page" } },
        { "id": "checkout-to-order", "type": "Navigation", "source": { "element": "checkout-page" }, "target": { "element": "order-page" } },
        { "id": "checkout-to-catalog", "type": "Navigation", "source": { "element": "checkout-page" }, "target": { "element": "catalog-page" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```tsx file src/router.tsx
import { createBrowserRouter } from 'react-router-dom';
import App from './App';
import CatalogPage from './pages/CatalogPage';
import CheckoutPage from './pages/CheckoutPage';
import OrderPage from './pages/OrderPage';

export const router = createBrowserRouter([
    { path: '/', element: <App />, children: [
        { index: true, element: <CatalogPage /> },
        { path: 'checkout', element: <CheckoutPage /> },
        { path: 'orders/:id', element: <OrderPage /> },
    ] },
]);
```

```tsx file src/App.tsx
import { Link, Outlet } from 'react-router-dom';

export default function App() {
    return (
        <>
            <Link to="/" className="brand">eShop</Link>
            <Outlet />
        </>
    );
}
```

```tsx file src/pages/CatalogPage.tsx
export default function CatalogPage() {
    return <h1>Catalog</h1>;
}
```

```tsx file src/pages/CheckoutPage.tsx
import { useNavigate } from 'react-router-dom';

export default function CheckoutPage() {
    const navigate = useNavigate();

    async function placed(orderId: string) {
        navigate(`/orders/${orderId}`);
    }

    return <button onClick={() => placed('42')}>Place order</button>;
}
```

```tsx file src/pages/OrderPage.tsx
export default function OrderPage() {
    return <h1>Order</h1>;
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "app-to-catalog", "outcome": "verified", "at": "src/App.tsx:6" },
    { "kind": "intent.navigates", "element": "checkout-to-order", "outcome": "verified", "at": "src/pages/CheckoutPage.tsx:7" },
    { "kind": "intent.navigates", "element": "checkout-to-catalog", "outcome": "contradicted", "expected": "present" }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "severity": "error", "subject": "checkout-to-catalog" }
  ]
}
```

## react-navigation-forms

The other ways react-router navigates, in a JavaScript front end: a `<NavLink>` whose `to` is a path object, a `<Navigate>` that redirects as it renders, and a `redirect(...)` from the route module's `loader`, which runs for the page's route. A link whose path the code computes whole can't be read, so a Navigation it would be the only evidence of can't be verified.

```json case
{ "phase": 6, "kinds": ["intent.navigates"], "requirements": ["R2", "R3", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "nav-bar", "type": "Component", "name": "NavBar" },
          { "id": "account-page", "type": "Page", "name": "AccountPage", "stereotypes": { "Page": { "Route": "/account" } } },
          { "id": "orders-page", "type": "Page", "name": "OrdersPage", "stereotypes": { "Page": { "Route": "/orders" } } },
          { "id": "cart-page", "type": "Page", "name": "CartPage", "stereotypes": { "Page": { "Route": "/cart" } } },
          { "id": "login-page", "type": "Page", "name": "LoginPage", "stereotypes": { "Page": { "Route": "/login" } } },
          { "id": "help-page", "type": "Page", "name": "HelpPage", "stereotypes": { "Page": { "Route": "/help" } } }
        ]
      }],
      "associations": [
        { "id": "nav-to-cart", "type": "Navigation", "source": { "element": "nav-bar" }, "target": { "element": "cart-page" } },
        { "id": "nav-to-help", "type": "Navigation", "source": { "element": "nav-bar" }, "target": { "element": "help-page" } },
        { "id": "account-to-login", "type": "Navigation", "source": { "element": "account-page" }, "target": { "element": "login-page" } },
        { "id": "orders-to-login", "type": "Navigation", "source": { "element": "orders-page" }, "target": { "element": "login-page" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```jsx file src/routes.jsx
import AccountPage from './AccountPage';
import OrdersPage, { loader as ordersLoader } from './OrdersPage';
import CartPage from './CartPage';
import LoginPage from './LoginPage';
import HelpPage from './HelpPage';

export const routes = [
    { path: '/account', element: <AccountPage /> },
    { path: '/orders', element: <OrdersPage />, loader: ordersLoader },
    { path: '/cart', element: <CartPage /> },
    { path: '/login', element: <LoginPage /> },
    { path: '/help', element: <HelpPage /> },
];
```

```jsx file src/NavBar.jsx
import { NavLink } from 'react-router-dom';

export function NavBar({ helpPath }) {
    return (
        <nav>
            <NavLink to={{ pathname: '/cart' }}>Cart</NavLink>
            <NavLink to={helpPath}>Help</NavLink>
        </nav>
    );
}
```

```jsx file src/AccountPage.jsx
import { Navigate } from 'react-router-dom';

export default function AccountPage({ user }) {
    if (!user) {
        return <Navigate to="/login" replace />;
    }

    return <h1>{user.name}</h1>;
}
```

```jsx file src/OrdersPage.jsx
import { redirect } from 'react-router-dom';

export async function loader() {
    if (!localStorage.getItem('token')) {
        throw redirect('/login');
    }

    return null;
}

export default function OrdersPage() {
    return <h1>Orders</h1>;
}
```

```jsx file src/CartPage.jsx
export default function CartPage() {
    return <h1>Cart</h1>;
}
```

```jsx file src/LoginPage.jsx
export default function LoginPage() {
    return <h1>Log in</h1>;
}
```

```jsx file src/HelpPage.jsx
export default function HelpPage() {
    return <h1>Help</h1>;
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "nav-to-cart", "outcome": "verified", "at": "src/NavBar.jsx:6" },
    { "kind": "intent.navigates", "element": "nav-to-help", "outcome": "cantVerify", "reason": "unreadable" },
    { "kind": "intent.navigates", "element": "account-to-login", "outcome": "verified", "at": "src/AccountPage.jsx:5" },
    { "kind": "intent.navigates", "element": "orders-to-login", "outcome": "verified", "at": "src/OrdersPage.jsx:5" }
  ]
}
```

## mvc-links

ASP.NET Core MVC views, read by the MVC reader (`intent.mvc`): a link navigates to a page when it reaches an action that takes a GET and renders the page's view. An `<a href>` reaches the actions served at its path (`~/orders` is `OrdersController.Index`, served by the default conventional route at `/Orders`), an `<a asp-controller asp-action>` the action it names, and an `<a asp-page>` the Razor Page at that path. A link to another site reaches no page of this one.

```json case
{ "phase": 6, "kinds": ["intent.navigates"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop.Web", "type": "User Interface Package",
        "elements": [
          { "id": "views", "type": "Folder", "name": "Views", "children": [
            { "id": "home", "type": "Folder", "name": "Home", "children": [
              { "id": "home-index", "type": "Page", "name": "Index", "stereotypes": { "Page": { "Route": "/" } } }
            ] },
            { "id": "orders", "type": "Folder", "name": "Orders", "children": [
              { "id": "orders-index", "type": "Page", "name": "Index", "stereotypes": { "Page": { "Route": "/Orders" } } },
              { "id": "cancelled", "type": "Page", "name": "Cancelled", "stereotypes": { "Page": { } } }
            ] }
          ] },
          { "id": "pages", "type": "Folder", "name": "Pages", "children": [
            { "id": "privacy", "type": "Page", "name": "Privacy", "stereotypes": { "Page": { "Route": "/Privacy" } } }
          ] }
        ]
      }],
      "associations": [
        { "id": "home-to-orders", "type": "Navigation", "source": { "element": "home-index" }, "target": { "element": "orders-index" } },
        { "id": "home-to-privacy", "type": "Navigation", "source": { "element": "home-index" }, "target": { "element": "privacy" } },
        { "id": "orders-to-home", "type": "Navigation", "source": { "element": "orders-index" }, "target": { "element": "home-index" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```xml file src/Shop.Web/Shop.Web.csproj
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
```

```csharp file src/Shop.Web/Program.cs
var app = WebApplication.CreateBuilder(args).Build();
app.MapDefaultControllerRoute();
app.MapRazorPages();
app.Run();
```

```csharp file src/Shop.Web/Controllers/HomeController.cs
namespace Shop.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
```

```csharp file src/Shop.Web/Controllers/OrdersController.cs
namespace Shop.Web.Controllers;

public class OrdersController : Controller
{
    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult Cancel(int id) => View("Cancelled");
}
```

```cshtml file src/Shop.Web/Views/Home/Index.cshtml
<h1>Welcome</h1>
<a href="~/orders">Your orders</a>
<a asp-page="/Privacy">Privacy</a>
<a href="https://learn.microsoft.com/aspnet/core">About ASP.NET Core</a>
```

```cshtml file src/Shop.Web/Views/Orders/Index.cshtml
<h1>Your orders</h1>
<a asp-controller="Home" asp-action="Index">Home</a>
<a asp-action="Cancel" asp-route-id="5">Cancel order 5</a>
```

```cshtml file src/Shop.Web/Views/Orders/Cancelled.cshtml
<h1>Order cancelled</h1>
```

```cshtml file src/Shop.Web/Pages/Privacy.cshtml
@page
<h1>Privacy</h1>
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "home-to-orders", "outcome": "verified", "at": "src/Shop.Web/Views/Home/Index.cshtml:2" },
    { "kind": "intent.navigates", "element": "home-to-privacy", "outcome": "verified", "at": "src/Shop.Web/Views/Home/Index.cshtml:3" },
    { "kind": "intent.navigates", "element": "orders-to-home", "outcome": "verified", "at": "src/Shop.Web/Views/Orders/Index.cshtml:2" }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "count": 0 }
  ]
}
```

## mvc-link-reaches-another-page

A planted drift: the home page's link now goes to `~/home`, the home page itself. One finding, naming the target's URLs and where the view links to.

```json case
{ "phase": 6, "base": "mvc-links", "kinds": ["intent.navigates"], "requirements": ["R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```cshtml file src/Shop.Web/Views/Home/Index.cshtml
<h1>Welcome</h1>
<a href="~/home">Your orders</a>
<a asp-page="/Privacy">Privacy</a>
<a href="https://learn.microsoft.com/aspnet/core">About ASP.NET Core</a>
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "home-to-orders", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "severity": "error", "subject": "home-to-orders", "messageContains": "`Index` navigates to `Index`: in the model, not in the code. `Index` is at `/Orders`, `/Orders/Index`, and `src/Shop.Web/Views/Home/Index.cshtml` links to `href=\"~/home\"`, `asp-page=\"/Privacy\"`, `href=\"https://learn.microsoft.com/aspnet/core\"`." },
    { "rule": "scope/coverage", "severity": "info" }
  ],
  "noOtherFindings": true
}
```

## mvc-link-to-a-post-action-states-nothing

A link sends a GET, so a link to an action that takes only a POST reaches nothing, though that action renders a page: the orders page's `<a asp-action="Cancel">` doesn't navigate to `Cancelled`, which only the POST `Cancel` renders.

```json case
{ "phase": 6, "base": "mvc-links", "kinds": ["intent.navigates"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop.Web", "type": "User Interface Package",
        "elements": [
          { "id": "views", "type": "Folder", "name": "Views", "children": [
            { "id": "orders", "type": "Folder", "name": "Orders", "children": [
              { "id": "orders-index", "type": "Page", "name": "Index", "stereotypes": { "Page": { "Route": "/Orders" } } },
              { "id": "cancelled", "type": "Page", "name": "Cancelled", "stereotypes": { "Page": { } } }
            ] }
          ] }
        ]
      }],
      "associations": [
        { "id": "orders-to-cancelled", "type": "Navigation", "source": { "element": "orders-index" }, "target": { "element": "cancelled" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "orders-to-cancelled", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "severity": "error", "subject": "orders-to-cancelled", "messageContains": "No GET shows `Cancelled`" },
    { "rule": "scope/coverage", "severity": "info" }
  ],
  "noOtherFindings": true
}
```

## mvc-helper-links

The same links written with the HTML and URL helpers: `Html.ActionLink("Your orders", "Index", "Orders")` links to the action and controller it names, `href="@Url.Page("/Privacy")"` to the Razor Page, and `href="@Url.Action("Index", "Home")"` to the home page, each a GET.

```json case
{ "phase": 6, "base": "mvc-links", "kinds": ["intent.navigates"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```cshtml file src/Shop.Web/Views/Home/Index.cshtml
<h1>Welcome</h1>
<p>@Html.ActionLink("Your orders", "Index", "Orders")</p>
<a href="@Url.Page("/Privacy")">Privacy</a>
```

```cshtml file src/Shop.Web/Views/Orders/Index.cshtml
<h1>Your orders</h1>
<a href="@Url.Action("Index", "Home")">Home</a>
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "home-to-orders", "outcome": "verified", "at": "src/Shop.Web/Views/Home/Index.cshtml:2" },
    { "kind": "intent.navigates", "element": "home-to-privacy", "outcome": "verified", "at": "src/Shop.Web/Views/Home/Index.cshtml:3" },
    { "kind": "intent.navigates", "element": "orders-to-home", "outcome": "verified", "at": "src/Shop.Web/Views/Orders/Index.cshtml:2" }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "count": 0 }
  ]
}
```

## maui-navigations

A .NET MAUI page's navigations (`intent.maui`) are its own code's and its binding context's, as a view model navigates in MVVM: a Shell navigation (`Shell.Current.GoToAsync(route)`, or a call of the code's own method that takes the route as a parameter and makes one, whatever it is named, here eShop's `INavigationService.NavigateToAsync` reached through its implementation), resolved against the Shell's contents and the registered routes, or a page pushed on the navigation stack. A Shell's navigation to a page is the menu entry its content is.

```json case
{ "phase": 6, "kinds": ["intent.navigates"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "AppShell" },
          { "id": "login", "type": "Page", "name": "LoginView", "stereotypes": { "Page": { } } },
          { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { } } },
          { "id": "basket", "type": "Page", "name": "BasketView", "stereotypes": { "Page": { } } },
          { "id": "order", "type": "Page", "name": "OrderView", "stereotypes": { "Page": { } } }
        ]
      }],
      "associations": [
        { "id": "shell-to-catalog", "type": "Navigation", "source": { "element": "shell" }, "target": { "element": "catalog" } },
        { "id": "login-to-catalog", "type": "Navigation", "source": { "element": "login" }, "target": { "element": "catalog" } },
        { "id": "catalog-to-basket", "type": "Navigation", "source": { "element": "catalog" }, "target": { "element": "basket" } },
        { "id": "basket-to-order", "type": "Navigation", "source": { "element": "basket" }, "target": { "element": "order" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```xml file src/Shop/Shop.csproj
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <UseMaui>true</UseMaui>
  </PropertyGroup>
</Project>
```

```xml file src/Shop/AppShell.xaml
<Shell x:Class="Shop.AppShell"
       xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:Shop.Views">
    <FlyoutItem>
        <ShellContent ContentTemplate="{DataTemplate views:LoginView}" Route="Login" />
    </FlyoutItem>
    <TabBar Route="Main">
        <ShellContent ContentTemplate="{DataTemplate views:CatalogView}" Route="Catalog" />
    </TabBar>
</Shell>
```

```csharp file src/Shop/AppShell.xaml.cs
using Shop.Views;

namespace Shop;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Routing.RegisterRoute("Basket", typeof(BasketView));
        InitializeComponent();
    }
}
```

```csharp file src/Shop/Services/Navigation.cs
namespace Shop.Services;

public interface INavigationService
{
    Task NavigateToAsync(string route);
}

public class MauiNavigationService : INavigationService
{
    public Task NavigateToAsync(string route)
    {
        var state = new ShellNavigationState(route);
        return Shell.Current.GoToAsync(state);
    }
}
```

```csharp file src/Shop/ViewModels/CatalogViewModel.cs
using Shop.Services;

namespace Shop.ViewModels;

public class CatalogViewModel(INavigationService navigation)
{
    public Task ViewBasketAsync() => navigation.NavigateToAsync("Basket");
}
```

```csharp file src/Shop/Views/CatalogView.cs
using Shop.ViewModels;

namespace Shop.Views;

public class CatalogView : ContentPage
{
    public CatalogView(CatalogViewModel viewModel)
    {
        BindingContext = viewModel;
    }
}
```

```csharp file src/Shop/Views/Pages.cs
namespace Shop.Views;

public class LoginView : ContentPage
{
    private async void SignIn() => await Shell.Current.GoToAsync("//Main/Catalog");
}

public class BasketView : ContentPage
{
    private Task OpenOrder() => Navigation.PushAsync(new OrderView());
}

public class OrderView : ContentPage
{
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "shell-to-catalog", "outcome": "verified", "at": "src/Shop/AppShell.xaml:9" },
    { "kind": "intent.navigates", "element": "login-to-catalog", "outcome": "verified", "at": "src/Shop/Views/Pages.cs:5" },
    { "kind": "intent.navigates", "element": "catalog-to-basket", "outcome": "verified", "at": "src/Shop/ViewModels/CatalogViewModel.cs:7" },
    { "kind": "intent.navigates", "element": "basket-to-order", "outcome": "verified", "at": "src/Shop/Views/Pages.cs:10" }
  ],
  "others": "verified",
  "noOtherFindings": true
}
```

## maui-navigation-changed

A planted drift: the catalog's view model navigates to a route no page is at. One finding, the Navigation's, naming the page's routes and where the code goes.

```json case
{ "phase": 6, "base": "maui-navigations", "kinds": ["intent.navigates"], "requirements": ["R3", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/Shop/ViewModels/CatalogViewModel.cs
using Shop.Services;

namespace Shop.ViewModels;

public class CatalogViewModel(INavigationService navigation)
{
    public Task ViewBasketAsync() => navigation.NavigateToAsync("Filter");
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "catalog-to-basket", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "severity": "error", "subject": "catalog-to-basket", "messageContains": "`CatalogView` navigates to `BasketView`: in the model, not in the code. `BasketView` is at `/Basket`, and `CatalogView` and its binding context `CatalogViewModel` navigate to `\"Filter\"`." }
  ],
  "noOtherFindings": true
}
```

## maui-computed-route

The view model navigates to a route it picks from a list: the route isn't text the code states, so the Navigation can't be verified.

```json case
{ "phase": 6, "base": "maui-navigations", "kinds": ["intent.navigates"], "cases": ["engine/unreadable"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/Shop/ViewModels/CatalogViewModel.cs
using Shop.Services;

namespace Shop.ViewModels;

public class CatalogViewModel(INavigationService navigation)
{
    private readonly string[] _routes = ["Basket", "Filter"];

    public Task OpenAsync(int index) => navigation.NavigateToAsync(_routes[index]);
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "catalog-to-basket", "outcome": "cantVerify", "reason": "unreadable", "at": "src/Shop/ViewModels/CatalogViewModel.cs:9" }
  ]
}
```

## react-links-from-a-constant-array

A React header renders its links from a constant array of `{ title, path }` literals: `links.map(({ title, path }) => <NavLink to={path} />)`. The callback's `path` stands for each item's `path`, so the header links to each of them, as a footer does through `footerLinks.map((link) => <Link to={link.path} />)`.

```json case
{ "phase": 6, "kinds": ["intent.navigates"], "requirements": ["R2", "R3", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "header", "type": "Component", "name": "Header" },
          { "id": "footer", "type": "Component", "name": "Footer" },
          { "id": "catalog-page", "type": "Page", "name": "CatalogPage", "stereotypes": { "Page": { "Route": "/catalog" } } },
          { "id": "about-page", "type": "Page", "name": "AboutPage", "stereotypes": { "Page": { "Route": "/about" } } },
          { "id": "contact-page", "type": "Page", "name": "ContactPage", "stereotypes": { "Page": { "Route": "/contact" } } }
        ]
      }],
      "associations": [
        { "id": "header-to-catalog", "type": "Navigation", "source": { "element": "header" }, "target": { "element": "catalog-page" } },
        { "id": "header-to-about", "type": "Navigation", "source": { "element": "header" }, "target": { "element": "about-page" } },
        { "id": "footer-to-contact", "type": "Navigation", "source": { "element": "footer" }, "target": { "element": "contact-page" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```tsx file src/router.tsx
import { createBrowserRouter } from 'react-router-dom';
import CatalogPage from './pages/CatalogPage';
import AboutPage from './pages/AboutPage';
import ContactPage from './pages/ContactPage';

export const router = createBrowserRouter([
    { path: '/catalog', element: <CatalogPage /> },
    { path: '/about', element: <AboutPage /> },
    { path: '/contact', element: <ContactPage /> },
]);
```

```tsx file src/layout/Header.tsx
import { NavLink } from 'react-router-dom';

const links = [
    { title: 'catalog', path: '/catalog' },
    { title: 'about', path: '/about' },
];

export default function Header() {
    return (
        <nav>
            {links.map(({ title, path }) => (
                <NavLink to={path} key={path}>{title}</NavLink>
            ))}
        </nav>
    );
}
```

```tsx file src/layout/Footer.tsx
import { Link } from 'react-router-dom';

const footerLinks = [{ title: 'contact', path: '/contact' }];

export default function Footer() {
    return (
        <footer>
            {footerLinks.map((link) => (
                <Link to={link.path} key={link.title}>{link.title}</Link>
            ))}
        </footer>
    );
}
```

```tsx file src/pages/CatalogPage.tsx
export default function CatalogPage() {
    return <h1>Catalog</h1>;
}
```

```tsx file src/pages/AboutPage.tsx
export default function AboutPage() {
    return <h1>About</h1>;
}
```

```tsx file src/pages/ContactPage.tsx
export default function ContactPage() {
    return <h1>Contact</h1>;
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "header-to-catalog", "outcome": "verified", "at": "src/layout/Header.tsx:12" },
    { "kind": "intent.navigates", "element": "header-to-about", "outcome": "verified", "at": "src/layout/Header.tsx:12" },
    { "kind": "intent.navigates", "element": "footer-to-contact", "outcome": "verified", "at": "src/layout/Footer.tsx:9" }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "count": 0 }
  ]
}
```

## react-links-from-a-constant-array-contradicted

The header's array no longer has the about page's path, so its Navigation to it is contradicted.

```json case
{ "phase": 6, "base": "react-links-from-a-constant-array", "kinds": ["intent.navigates"], "requirements": ["R14"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```tsx file src/layout/Header.tsx
import { NavLink } from 'react-router-dom';

const links = [
    { title: 'catalog', path: '/catalog' },
    { title: 'about', path: '/about-us' },
];

export default function Header() {
    return (
        <nav>
            {links.map(({ title, path }) => (
                <NavLink to={path} key={path}>{title}</NavLink>
            ))}
        </nav>
    );
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "header-to-catalog", "outcome": "verified", "at": "src/layout/Header.tsx:12" },
    { "kind": "intent.navigates", "element": "header-to-about", "outcome": "contradicted", "expected": "present" }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "severity": "error", "subject": "header-to-about" }
  ]
}
```

## angular-links-through-url-constants

An Angular header whose links come from the app's URL constants: `[routerLink]="authUrls.logIn"` reads the component's `authUrls`, which holds `AUTH_URLS`, whose `logIn` is built from the path constants; `[routerLink]="detailUrl()"` runs the `computed` the component holds, `POKEMON_URLS.detail(...)`; and `register()` calls the app's own `LanguageService.navigateWithUserLanguage(language, AUTH_URLS.register)`, whose `router.navigate([pathToRedirect])` navigates to what it is handed. Each link is read through the constants and the calls, the helper's parameters bound to what the header passes, and is placed at the header's own call.

```json case
{ "phase": 6, "kinds": ["intent.navigates"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "app", "name": "App", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "App" },
          { "id": "header", "type": "Component", "name": "Header" },
          { "id": "log-in", "type": "Page", "name": "LogIn", "stereotypes": { "Page": { "Route": "/auth/log-in" } } },
          { "id": "register", "type": "Page", "name": "Register", "stereotypes": { "Page": { "Route": "/auth/register" } } },
          { "id": "my-account", "type": "Page", "name": "MyAccount", "stereotypes": { "Page": { "Route": "/auth/my-account" } } },
          { "id": "pokemon-detail", "type": "Page", "name": "PokemonDetail", "stereotypes": { "Page": { "Route": "/pokemon/{pokemonId}" } } }
        ]
      }],
      "associations": [
        { "id": "header-log-in", "type": "Navigation", "source": { "element": "header" }, "target": { "element": "log-in" } },
        { "id": "header-register", "type": "Navigation", "source": { "element": "header" }, "target": { "element": "register" } },
        { "id": "header-pokemon", "type": "Navigation", "source": { "element": "header" }, "target": { "element": "pokemon-detail" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "app", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```typescript file src/main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { App } from './app/app';
import { routes } from './app/app.routes';

bootstrapApplication(App, { providers: [provideRouter(routes)] });
```

```typescript file src/app/app.ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from './header';

@Component({ selector: 'app-root', imports: [RouterOutlet, Header], template: `<app-header /><router-outlet />` })
export class App {}
```

```typescript file src/app/paths.constants.ts
export const AUTHENTICATION_PATHS = {
  base: 'auth',
  logIn: 'log-in',
  register: 'register',
  myAccount: 'my-account',
};

export const POKEMON_PATHS = {
  base: 'pokemon',
};
```

```typescript file src/app/urls.constants.ts
import { AUTHENTICATION_PATHS, POKEMON_PATHS } from './paths.constants';

export const AUTH_URLS = {
  logIn: `/${AUTHENTICATION_PATHS.base}/${AUTHENTICATION_PATHS.logIn}`,
  register: `/${AUTHENTICATION_PATHS.base}/${AUTHENTICATION_PATHS.register}`,
  myAccount: `/${AUTHENTICATION_PATHS.base}/${AUTHENTICATION_PATHS.myAccount}`,
};

export const POKEMON_URLS = {
  detail: (id: string) => `/${POKEMON_PATHS.base}/${id}`,
};
```

```typescript file src/app/app.routes.ts
import { Routes } from '@angular/router';
import { AUTHENTICATION_PATHS, POKEMON_PATHS } from './paths.constants';
import { LogIn, MyAccount, PokemonDetail, Register } from './pages';

export const routes: Routes = [
  {
    path: AUTHENTICATION_PATHS.base,
    children: [
      { path: AUTHENTICATION_PATHS.logIn, component: LogIn },
      { path: AUTHENTICATION_PATHS.register, component: Register },
      { path: AUTHENTICATION_PATHS.myAccount, component: MyAccount },
    ],
  },
  { path: `${POKEMON_PATHS.base}/:pokemonId`, component: PokemonDetail },
];
```

```typescript file src/app/pages.ts
import { Component } from '@angular/core';

@Component({ selector: 'app-log-in', template: `<h1>Log in</h1>` })
export class LogIn {}

@Component({ selector: 'app-register', template: `<h1>Register</h1>` })
export class Register {}

@Component({ selector: 'app-my-account', template: `<h1>My account</h1>` })
export class MyAccount {}

@Component({ selector: 'app-pokemon-detail', template: `<h1>Pokemon</h1>` })
export class PokemonDetail {}
```

```typescript file src/app/language.service.ts
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly router = inject(Router);

  navigateWithUserLanguage(language: string, pathToRedirect: string) {
    if (language) {
      void this.router.navigate([pathToRedirect]);
    }
  }
}
```

```typescript file src/app/header.ts
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LanguageService } from './language.service';
import { AUTH_URLS, POKEMON_URLS } from './urls.constants';

@Component({
  selector: 'app-header',
  imports: [RouterLink],
  template: `
    <a [routerLink]="authUrls.logIn">Log in</a>
    <a [routerLink]="detailUrl()">Pokemon</a>
    <button (click)="register()">Register</button>
  `,
})
export class Header {
  private readonly languageService = inject(LanguageService);
  readonly authUrls = AUTH_URLS;
  readonly pokemonId = signal('pikachu');
  readonly detailUrl = computed(() => POKEMON_URLS.detail(this.pokemonId()));

  register() {
    this.languageService.navigateWithUserLanguage('en', AUTH_URLS.register);
  }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "header-log-in", "outcome": "verified" },
    { "kind": "intent.navigates", "element": "header-pokemon", "outcome": "verified" },
    { "kind": "intent.navigates", "element": "header-register", "outcome": "verified", "at": "src/app/header.ts:22" }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "count": 0 }
  ]
}
```

## angular-links-through-url-constants-contradicted

The model says the header also navigates to `MyAccount`, but none of its links, read through the constants and the helper, is `/auth/my-account`.

```json case
{ "phase": 6, "base": "angular-links-through-url-constants", "kinds": ["intent.navigates"], "requirements": ["R2", "R14", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "app", "name": "App", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "App" },
          { "id": "header", "type": "Component", "name": "Header" },
          { "id": "log-in", "type": "Page", "name": "LogIn", "stereotypes": { "Page": { "Route": "/auth/log-in" } } },
          { "id": "register", "type": "Page", "name": "Register", "stereotypes": { "Page": { "Route": "/auth/register" } } },
          { "id": "my-account", "type": "Page", "name": "MyAccount", "stereotypes": { "Page": { "Route": "/auth/my-account" } } },
          { "id": "pokemon-detail", "type": "Page", "name": "PokemonDetail", "stereotypes": { "Page": { "Route": "/pokemon/{pokemonId}" } } }
        ]
      }],
      "associations": [
        { "id": "header-my-account", "type": "Navigation", "source": { "element": "header" }, "target": { "element": "my-account" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "app", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "header-my-account", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "severity": "error", "subject": "header-my-account" }
  ]
}
```

## angular-navigation-in-callbacks

A component that navigates once something it waits for arrives: in a `subscribe` callback, `router.navigate([POKEMON_URLS.detail(String(id))])`, and in an observer's `next`, `router.navigate([POKEMON_URLS.detail(pokemon.name)])`. A command segment the code builds with a value in it, `/pokemon/${id}`, keeps its text with the value as a hole, which stands for the page's route parameter; only a segment the code computes whole is a hole.

```json case
{ "phase": 6, "kinds": ["intent.navigates"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "app", "name": "App", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "App" },
          { "id": "register", "type": "Page", "name": "Register", "stereotypes": { "Page": { "Route": "/register" } } },
          { "id": "search", "type": "Component", "name": "Search" },
          { "id": "pokemon-detail", "type": "Page", "name": "PokemonDetail", "stereotypes": { "Page": { "Route": "/pokemon/{pokemonId}" } } }
        ]
      }],
      "associations": [
        { "id": "register-pokemon", "type": "Navigation", "source": { "element": "register" }, "target": { "element": "pokemon-detail" } },
        { "id": "search-pokemon", "type": "Navigation", "source": { "element": "search" }, "target": { "element": "pokemon-detail" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "app", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```typescript file src/main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { App } from './app/app';
import { routes } from './app/app.routes';

bootstrapApplication(App, { providers: [provideRouter(routes)] });
```

```typescript file src/app/app.ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({ selector: 'app-root', imports: [RouterOutlet], template: `<router-outlet />` })
export class App {}
```

```typescript file src/app/urls.constants.ts
export const POKEMON_URLS = {
  detail: (id: string) => `/pokemon/${id}`,
};
```

```typescript file src/app/app.routes.ts
import { Routes } from '@angular/router';
import { PokemonDetail, Register } from './pages';

export const routes: Routes = [
  { path: 'register', component: Register },
  { path: 'pokemon/:pokemonId', component: PokemonDetail },
];
```

```typescript file src/app/pages.ts
import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { timer } from 'rxjs';
import { POKEMON_URLS } from './urls.constants';

@Component({ selector: 'app-register', template: `<h1>Register</h1>` })
export class Register {
  private readonly router = inject(Router);

  celebrate(): void {
    timer(500).subscribe(() => {
      const id = Math.floor(Math.random() * 1000) + 1;
      void this.router.navigate([POKEMON_URLS.detail(String(id))]);
    });
  }
}

@Component({ selector: 'app-pokemon-detail', template: `<h1>Pokemon</h1>` })
export class PokemonDetail {}
```

```typescript file src/app/search.ts
import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { POKEMON_URLS } from './urls.constants';

@Component({ selector: 'app-search', template: `<button (click)="search('pikachu')">Search</button>` })
export class Search {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  search(name: string): void {
    this.http.get<{ name: string }>(`/api/pokemon/${name}`).subscribe({
      next: (pokemon) => {
        void this.router.navigate([POKEMON_URLS.detail(pokemon.name)]);
      },
    });
  }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "register-pokemon", "outcome": "verified", "at": "src/app/pages.ts:13" },
    { "kind": "intent.navigates", "element": "search-pokemon", "outcome": "verified", "at": "src/app/search.ts:14" }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "count": 0 }
  ]
}
```
