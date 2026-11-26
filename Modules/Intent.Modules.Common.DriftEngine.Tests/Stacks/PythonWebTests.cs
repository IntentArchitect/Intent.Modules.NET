using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Stacks.Jinja;
using Intent.Modules.Common.DriftEngine.Stacks.Python;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// A Python web app the Domain, Services and User Interface designers model, read by the stock stacks alone: SQLAlchemy's
/// declarative models (<c>intent.sqlalchemy</c>), FastAPI's routers and path operations (<c>intent.fastapi</c>), Jinja templates
/// (<c>intent.jinja</c>) and a plain script's <c>fetch</c> (<c>intent.typescript.ui</c>). The clean run has no finding, and each
/// planted break is one finding of its claim's kind.
/// </summary>
public class PythonWebTests
{
    private static readonly Dictionary<string, string> Code = new()
    {
        ["app/__init__.py"] = "",
        ["app/database.py"] = """
            from sqlalchemy.orm import DeclarativeBase


            class Base(DeclarativeBase):
                pass
            """,
        ["app/models.py"] = """
            from __future__ import annotations

            import enum
            from datetime import date

            from sqlalchemy import Date, Enum, ForeignKey, String, Text
            from sqlalchemy.orm import Mapped, mapped_column, relationship

            from app.database import Base


            class OrderStatus(str, enum.Enum):
                OPEN = "open"
                SHIPPED = "shipped"


            class Customer(Base):
                __tablename__ = "customers"

                id: Mapped[int] = mapped_column(primary_key=True)
                email: Mapped[str] = mapped_column(String(200), unique=True)

                orders: Mapped[list[Order]] = relationship(back_populates="customer")

                def __repr__(self) -> str:
                    return f"<Customer {self.id}>"


            class Order(Base):
                __tablename__ = "orders"

                id: Mapped[int] = mapped_column(primary_key=True)
                customer_id: Mapped[int] = mapped_column(ForeignKey("customers.id"), index=True)
                notes: Mapped[str] = mapped_column(Text, default="")
                status: Mapped[OrderStatus] = mapped_column(Enum(OrderStatus), default=OrderStatus.OPEN)
                ship_by: Mapped[date | None] = mapped_column(Date)

                customer: Mapped[Customer] = relationship(back_populates="orders")
            """,
        ["app/errors.py"] = """
            class NotFoundError(Exception):
                status_code = 404
            """,
        ["app/services.py"] = """
            from sqlalchemy import select
            from sqlalchemy.orm import Session

            from app.errors import NotFoundError
            from app.models import Order


            class OrderService:
                def __init__(self, session: Session) -> None:
                    self.session = session

                def list(self, *, limit: int = 20) -> list[Order]:
                    return list(self.session.scalars(select(Order).limit(limit)))

                def get(self, order_id: int) -> Order:
                    order = self.session.get(Order, order_id)
                    if order is None:
                        raise NotFoundError()
                    return order

                def update(self, order_id: int, changes: dict) -> Order:
                    order = self.get(order_id)
                    for field, value in changes.items():
                        setattr(order, field, value)
                    self.session.commit()
                    return order

                def delete(self, order_id: int) -> None:
                    self.session.delete(self.get(order_id))
                    self.session.commit()
            """,
        ["app/dependencies.py"] = """
            from typing import Annotated

            from fastapi import Depends, Request
            from sqlalchemy.orm import Session

            from app.services import OrderService


            def get_session(request: Request) -> Session:
                return request.app.state.session


            SessionDep = Annotated[Session, Depends(get_session)]


            def get_order_service(session: SessionDep) -> OrderService:
                return OrderService(session)


            OrderServiceDep = Annotated[OrderService, Depends(get_order_service)]
            """,
        ["app/schemas.py"] = """
            from pydantic import BaseModel

            from app.models import OrderStatus


            class OrderRead(BaseModel):
                id: int
                status: OrderStatus


            class OrderUpdate(BaseModel):
                status: OrderStatus
            """,
        ["app/api/__init__.py"] = """
            from fastapi import APIRouter

            from app.api import orders

            router = APIRouter(prefix="/api")
            router.include_router(orders.router)
            """,
        ["app/api/orders.py"] = """
            from typing import Annotated

            from fastapi import APIRouter, Query, Response, status

            from app.dependencies import OrderServiceDep
            from app.schemas import OrderRead, OrderUpdate

            router = APIRouter(prefix="/orders", tags=["orders"])

            Limit = Annotated[int, Query(ge=1, le=100)]


            @router.get("", response_model=list[OrderRead])
            def list_orders(service: OrderServiceDep, limit: Limit = 20):
                return service.list(limit=limit)


            @router.get("/{order_id}", response_model=OrderRead)
            def get_order(order_id: int, service: OrderServiceDep):
                return service.get(order_id)


            @router.patch("/{order_id}", response_model=OrderRead)
            def update_order(order_id: int, data: OrderUpdate, service: OrderServiceDep):
                return service.update(order_id, data.model_dump())


            @router.delete("/{order_id}", status_code=status.HTTP_204_NO_CONTENT)
            def delete_order(order_id: int, service: OrderServiceDep) -> Response:
                service.delete(order_id)
                return Response(status_code=status.HTTP_204_NO_CONTENT)
            """,
        ["app/web/__init__.py"] = "",
        ["app/web/templating.py"] = """
            from fastapi.templating import Jinja2Templates

            templates = Jinja2Templates(directory="app/templates")
            """,
        ["app/web/routes.py"] = """
            from typing import Annotated

            from fastapi import APIRouter, Form, Request
            from fastapi.responses import HTMLResponse, RedirectResponse

            from app.dependencies import OrderServiceDep
            from app.web.templating import templates

            router = APIRouter(include_in_schema=False)


            @router.get("/", response_class=HTMLResponse, name="index")
            def index(request: Request, service: OrderServiceDep):
                return templates.TemplateResponse(request, "index.html", {"orders": service.list()})


            @router.get("/orders/{order_id}", response_class=HTMLResponse, name="order_page")
            def order_page(request: Request, order_id: int, service: OrderServiceDep):
                return templates.TemplateResponse(request, "order.html", {"order": service.get(order_id)})


            @router.post("/orders/{order_id}/notes", name="save_notes")
            def save_notes(request: Request, order_id: int, service: OrderServiceDep, notes: Annotated[str, Form()] = ""):
                service.update(order_id, {"notes": notes})
                return RedirectResponse(str(request.url_for("order_page", order_id=order_id)), status_code=303)
            """,
        ["app/main.py"] = """
            from fastapi import FastAPI

            from app.api import router as api_router
            from app.web.routes import router as web_router

            app = FastAPI()
            app.include_router(api_router)
            app.include_router(web_router)
            """,
        ["app/templates/base.html"] = """
            <!doctype html>
            <html>
            <head><title>{% block title %}Shop{% endblock %}</title></head>
            <body>
              <a href="{{ url_for('index') }}">Orders</a>
              {% block content %}{% endblock %}
              {% block scripts %}{% endblock %}
            </body>
            </html>
            """,
        ["app/templates/index.html"] = """
            {% extends "base.html" %}
            {% block title %}Orders · Shop{% endblock %}
            {% block content %}
            <ul>
              {% for order in orders %}
              <li><a href="{{ url_for('order_page', order_id=order.id) }}">Order {{ order.id }}</a></li>
              {% endfor %}
            </ul>
            {% endblock %}
            """,
        ["app/templates/order.html"] = """
            {% extends "base.html" %}
            {% block title %}Order {{ order.id }} · Shop{% endblock %}
            {% block content %}
            <form method="post" action="{{ url_for('save_notes', order_id=order.id) }}">
              <textarea name="notes">{{ order.notes }}</textarea>
              <button type="submit">Save</button>
            </form>
            <select id="status" data-order-id="{{ order.id }}"></select>
            {% endblock %}
            {% block scripts %}<script src="{{ url_for('static', path='status.js') }}"></script>{% endblock %}
            """,
        ["app/static/status.js"] = """
            (() => {
              const select = document.getElementById("status");
              select.addEventListener("change", async () => {
                await fetch(`/api/orders/${select.dataset.orderId}`, {
                  method: "PATCH",
                  headers: { "Content-Type": "application/json" },
                  body: JSON.stringify({ status: select.value }),
                });
              });
            })();
            """,
        ["app/static/help.html"] = """
            <!doctype html>
            <html><body><h1>Help</h1><p>{% raw %}Not a template{% endraw %}</p></body></html>
            """
    };

    private const string Model = """
        {
          "designers": {
            "Domain": {
              "packages": [{
                "id": "domain", "name": "Shop", "type": "Domain Package", "stereotypes": { "Relational Database": {} },
                "elements": [
                  { "id": "order-status", "type": "Enum", "name": "OrderStatus", "children": [
                    { "id": "order-status-open", "type": "Enum-Literal", "name": "OPEN", "value": "\"open\"" },
                    { "id": "order-status-shipped", "type": "Enum-Literal", "name": "SHIPPED", "value": "shipped" }
                  ] },
                  { "id": "customer", "type": "Class", "name": "Customer", "stereotypes": { "Table": { "Name": "customers" } }, "children": [
                    { "id": "customer-id", "type": "Attribute", "name": "id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
                    { "id": "customer-email", "type": "Attribute", "name": "email", "typeRef": { "type": "string" },
                      "stereotypes": { "Text Constraints": { "MaxLength": "200" }, "Index": { "IsUnique": "true" } } }
                  ] },
                  { "id": "order", "type": "Class", "name": "Order", "stereotypes": { "Table": { "Name": "orders" } }, "children": [
                    { "id": "order-id", "type": "Attribute", "name": "id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
                    { "id": "order-customer-id", "type": "Attribute", "name": "customer_id", "typeRef": { "type": "int" },
                      "stereotypes": { "Index": { "IsUnique": "false" }, "Foreign Key": { "Association": { "ref": "customer-orders" } } } },
                    { "id": "order-notes", "type": "Attribute", "name": "notes", "typeRef": { "type": "string" }, "stereotypes": { "Text Constraints": { "SQL Data Type": "TEXT" } } },
                    { "id": "order-status-attribute", "type": "Attribute", "name": "status", "typeRef": { "element": "order-status" } },
                    { "id": "order-ship-by", "type": "Attribute", "name": "ship_by", "typeRef": { "type": "date", "nullable": true }, "stereotypes": { "Column": { "Type": "Date" } } }
                  ] },
                  { "id": "not-found", "type": "Class", "name": "NotFoundError", "children": [
                    { "id": "not-found-status", "type": "Attribute", "name": "status_code", "typeRef": { "type": "int" }, "value": "404", "static": true }
                  ] }
                ]
              }],
              "associations": [
                { "id": "customer-orders", "type": "Association",
                  "source": { "element": "customer", "multiplicity": "1", "navigable": true, "name": "customer" },
                  "target": { "element": "order", "multiplicity": "*", "navigable": true, "name": "orders" } }
              ]
            },
            "Services": {
              "packages": [{
                "id": "services", "name": "Shop", "type": "Services Package",
                "elements": [
                  { "id": "html-response", "type": "Type-Definition", "name": "HTMLResponse" },
                  { "id": "dict", "type": "Type-Definition", "name": "dict" },
                  { "id": "order-read", "type": "DTO", "name": "OrderRead", "children": [
                    { "id": "order-read-id", "type": "DTO-Field", "name": "id", "typeRef": { "type": "int" } },
                    { "id": "order-read-status", "type": "DTO-Field", "name": "status", "typeRef": { "element": "order-status" } }
                  ] },
                  { "id": "order-update", "type": "DTO", "name": "OrderUpdate", "children": [
                    { "id": "order-update-status", "type": "DTO-Field", "name": "status", "typeRef": { "element": "order-status" } }
                  ] },
                  { "id": "order-service", "type": "Service", "name": "OrderService", "children": [
                    { "id": "svc-list", "type": "Operation", "name": "list", "typeRef": { "element": "order", "collection": true }, "children": [
                      { "id": "svc-list-limit", "type": "Parameter", "name": "limit", "typeRef": { "type": "int" }, "value": "20" }
                    ] },
                    { "id": "svc-get", "type": "Operation", "name": "get", "typeRef": { "element": "order" }, "children": [
                      { "id": "svc-get-id", "type": "Parameter", "name": "order_id", "typeRef": { "type": "int" } }
                    ] },
                    { "id": "svc-update", "type": "Operation", "name": "update", "typeRef": { "element": "order" }, "children": [
                      { "id": "svc-update-id", "type": "Parameter", "name": "order_id", "typeRef": { "type": "int" } },
                      { "id": "svc-update-changes", "type": "Parameter", "name": "changes", "typeRef": { "element": "dict" } }
                    ] },
                    { "id": "svc-delete", "type": "Operation", "name": "delete", "children": [
                      { "id": "svc-delete-id", "type": "Parameter", "name": "order_id", "typeRef": { "type": "int" } }
                    ] }
                  ] },
                  { "id": "orders-api", "type": "Service", "name": "orders", "stereotypes": { "Http Service Settings": { "Route": "api/orders" } }, "children": [
                    { "id": "list-orders", "type": "Operation", "name": "list_orders", "typeRef": { "element": "order-read", "collection": true },
                      "stereotypes": { "Http Settings": { "Verb": "GET" } }, "children": [
                      { "id": "list-orders-limit", "type": "Parameter", "name": "limit", "typeRef": { "type": "int" }, "value": "20", "stereotypes": { "Parameter Settings": { "Source": "From Query" } } }
                    ] },
                    { "id": "get-order", "type": "Operation", "name": "get_order", "typeRef": { "element": "order-read" },
                      "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{order_id}" } }, "children": [
                      { "id": "get-order-id", "type": "Parameter", "name": "order_id", "typeRef": { "type": "int" }, "stereotypes": { "Parameter Settings": { "Source": "From Route" } } }
                    ] },
                    { "id": "update-order", "type": "Operation", "name": "update_order", "typeRef": { "element": "order-read" },
                      "stereotypes": { "Http Settings": { "Verb": "PATCH", "Route": "{order_id}" } }, "children": [
                      { "id": "update-order-id", "type": "Parameter", "name": "order_id", "typeRef": { "type": "int" }, "stereotypes": { "Parameter Settings": { "Source": "From Route" } } },
                      { "id": "update-order-data", "type": "Parameter", "name": "data", "typeRef": { "element": "order-update" }, "stereotypes": { "Parameter Settings": { "Source": "From Body" } } }
                    ] },
                    { "id": "delete-order", "type": "Operation", "name": "delete_order",
                      "stereotypes": { "Http Settings": { "Verb": "DELETE", "Route": "{order_id}", "Success Response Code": "204 (No Content)" } }, "children": [
                      { "id": "delete-order-id", "type": "Parameter", "name": "order_id", "typeRef": { "type": "int" }, "stereotypes": { "Parameter Settings": { "Source": "From Route" } } }
                    ] }
                  ] },
                  { "id": "web", "type": "Service", "name": "routes", "stereotypes": { "Http Service Settings": {}, "OpenAPI Settings": { "Ignore": "true" } }, "children": [
                    { "id": "index-op", "type": "Operation", "name": "index", "typeRef": { "element": "html-response" }, "stereotypes": { "Http Settings": { "Verb": "GET" } } },
                    { "id": "order-page-op", "type": "Operation", "name": "order_page", "typeRef": { "element": "html-response" },
                      "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "orders/{order_id}" } }, "children": [
                      { "id": "order-page-id", "type": "Parameter", "name": "order_id", "typeRef": { "type": "int" }, "stereotypes": { "Parameter Settings": { "Source": "From Route" } } }
                    ] },
                    { "id": "save-notes-op", "type": "Operation", "name": "save_notes",
                      "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "orders/{order_id}/notes" } }, "children": [
                      { "id": "save-notes-id", "type": "Parameter", "name": "order_id", "typeRef": { "type": "int" }, "stereotypes": { "Parameter Settings": { "Source": "From Route" } } },
                      { "id": "save-notes-notes", "type": "Parameter", "name": "notes", "typeRef": { "type": "string" }, "value": "\"\"", "stereotypes": { "Parameter Settings": { "Source": "From Form" } } }
                    ] }
                  ] }
                ]
              }],
              "associations": [
                { "id": "pi-list", "type": "Perform Invocation", "source": { "element": "list-orders" }, "target": { "element": "svc-list" } },
                { "id": "pi-get", "type": "Perform Invocation", "source": { "element": "get-order" }, "target": { "element": "svc-get" } },
                { "id": "pi-update", "type": "Perform Invocation", "source": { "element": "update-order" }, "target": { "element": "svc-update" } },
                { "id": "pi-delete", "type": "Perform Invocation", "source": { "element": "delete-order" }, "target": { "element": "svc-delete" } },
                { "id": "pi-index", "type": "Perform Invocation", "source": { "element": "index-op" }, "target": { "element": "svc-list" } },
                { "id": "pi-page", "type": "Perform Invocation", "source": { "element": "order-page-op" }, "target": { "element": "svc-get" } },
                { "id": "pi-notes", "type": "Perform Invocation", "source": { "element": "save-notes-op" }, "target": { "element": "svc-update" } },
                { "id": "q-list", "type": "Query Entity Action", "source": { "element": "svc-list" }, "target": { "element": "order", "multiplicity": "*" } },
                { "id": "q-get", "type": "Query Entity Action", "source": { "element": "svc-get" }, "target": { "element": "order" } },
                { "id": "u-update", "type": "Update Entity Action", "source": { "element": "svc-update" }, "target": { "element": "order" } }
              ]
            },
            "User Interface": {
              "packages": [{
                "id": "ui", "name": "Shop", "type": "User Interface Package",
                "elements": [
                  { "id": "base", "type": "Layout", "name": "base", "children": [
                    { "id": "title-region", "type": "Region", "name": "title" },
                    { "id": "content-region", "type": "Region", "name": "content" },
                    { "id": "scripts-region", "type": "Region", "name": "scripts" }
                  ] },
                  { "id": "index-page", "type": "Page", "name": "index", "stereotypes": { "Page": { "Route": "/", "Title": "Orders · Shop" } } },
                  { "id": "order-page", "type": "Page", "name": "order", "stereotypes": { "Page": { "Route": "/orders/{order_id}" } }, "children": [
                    { "id": "order-page-param", "type": "Property", "name": "order_id", "typeRef": { "type": "int" }, "stereotypes": { "[Route Parameter]": {} } }
                  ] },
                  { "id": "status-script", "type": "Behaviour", "name": "status" }
                ]
              }],
              "associations": [
                { "id": "content-index", "type": "Composition", "source": { "element": "content-region" }, "target": { "element": "index-page" } },
                { "id": "content-order", "type": "Composition", "source": { "element": "content-region" }, "target": { "element": "order-page" } },
                { "id": "base-index", "type": "Navigation", "source": { "element": "base" }, "target": { "element": "index-page" } },
                { "id": "index-order", "type": "Navigation", "source": { "element": "index-page" }, "target": { "element": "order-page" } },
                { "id": "order-status", "type": "Uses", "source": { "element": "order-page" }, "target": { "element": "status-script" } },
                { "id": "order-saves", "type": "Call Service Operation Action", "source": { "element": "order-page" }, "target": { "element": "save-notes-op" } },
                { "id": "status-updates", "type": "Call Service Operation Action", "source": { "element": "status-script" }, "target": { "element": "update-order" } }
              ]
            },
            "Codebase Structure": {
              "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
                "stereotypes": {
                  "Drift Scope": { "Entries": [ { "Glob": "app/**", "Scope": "In" }, { "Glob": "app/main.py\napp/dependencies.py\napp/database.py", "Scope": "Context", "Reason": "Wiring" } ] },
                  "Drift Check": { "Opt-in Rules": [
                    { "Rule": "optin/unmodelled.member", "Glob": "app/models.py" },
                    { "Rule": "optin/unmodelled.value", "Glob": "app/models.py\napp/api/**" }
                  ] }
                } }]
            }
          }
        }
        """;

    [Fact]
    public void The_app_the_model_states_is_clean_with_the_stock_stacks()
    {
        var result = Run(Code);

        Errors(result).ShouldBeEmpty(Describe(result));
        foreach (var (element, kind) in new[]
                 {
                     ("order-status-open", "enum.literal.value"), ("order-status-shipped", "enum.literal.value"),
                     ("order", "entity.persisted"), ("order", "table.name"), ("order", "key"), ("order-ship-by", "member.type"), ("order-ship-by", "member.nullable"),
                     ("order-customer-id", "member.type"), ("customer-email", "column.maxLength"), ("order-notes", "column.textType"), ("not-found-status", "member.type"),
                     ("list-orders", "operation.returns"), ("delete-order", "operation.returns"), ("svc-delete", "operation.returns"), ("index-op", "operation.returns"),
                     ("get-order", "operation.route"), ("get-order", "operation.verb"), ("orders-api", "service.route"), ("orders-api", "type.kind"),
                     ("delete-order", "operation.status"), ("index-op", "operation.openApi.ignore"), ("get-order", "operation.parameters"),
                     ("list-orders-limit", "parameter.type"), ("list-orders-limit", "parameter.source"), ("update-order-data", "parameter.source"), ("save-notes-notes", "parameter.source"),
                     ("base", "component.kind"), ("index-page", "component.kind"), ("index-page", "page.route"), ("index-page", "page.title"), ("order-page", "page.route"),
                     ("scripts-region", "region.outlet"), ("order-page-param", "property.route")
                 })
        {
            Claim(result, element, kind).Outcome.ShouldBe("verified", $"{element} {kind}\n{Describe(result)}");
        }

        foreach (var (link, kind) in new[]
                 {
                     ("pi-get", "intent.invokes"), ("pi-notes", "intent.invokes"), ("q-list", "intent.queries"), ("q-get", "intent.queries"), ("u-update", "intent.updates"),
                     ("content-order", "intent.uses"), ("order-status", "intent.uses"), ("base-index", "intent.navigates"), ("index-order", "intent.navigates"),
                     ("order-saves", "intent.invokes"), ("status-updates", "intent.invokes")
                 })
        {
            result.Claims.Where(x => x.Kind == kind && Link(x) == link).ShouldAllBe(x => x.Outcome == "verified", $"{link} {kind}\n{Describe(result)}");
            result.Claims.Count(x => x.Kind == kind && Link(x) == link).ShouldBeGreaterThan(0, $"{link} {kind}\n{Describe(result)}");
        }
    }

    [Theory]
    [InlineData("app/api/orders.py", "@router.get(\"/{order_id}\", response_model=OrderRead)", "@router.get(\"/by-id/{order_id}\", response_model=OrderRead)", "claim/operation.route")]
    [InlineData("app/api/orders.py", "@router.delete(", "@router.post(", "claim/operation.verb")]
    [InlineData("app/api/orders.py", "return service.get(order_id)", "return service.list()", "claim/intent.invokes")]
    [InlineData("app/web/routes.py", "notes: Annotated[str, Form()] = \"\"", "notes: str = \"\"", "claim/parameter.source")]
    [InlineData("app/models.py", "customer_id: Mapped[int]", "customer_id: Mapped[str]", "claim/member.type")]
    [InlineData("app/models.py", "ship_by: Mapped[date | None]", "ship_by: Mapped[date]", "claim/member.nullable")]
    [InlineData("app/models.py", "String(200)", "String(250)", "claim/column.maxLength")]
    [InlineData("app/models.py", "__tablename__ = \"orders\"", "__tablename__ = \"order\"", "claim/table.name")]
    [InlineData("app/models.py", "SHIPPED = \"shipped\"", "SHIPPED = \"sent\"", "claim/enum.literal.value")]
    [InlineData("app/services.py", "select(Order).limit(limit)", "select(Customer).limit(limit)", "claim/intent.queries")]
    [InlineData("app/templates/order.html", "url_for('save_notes', order_id=order.id)", "url_for('order_page', order_id=order.id)", "claim/intent.invokes")]
    [InlineData("app/static/status.js", "method: \"PATCH\"", "method: \"PUT\"", "claim/intent.invokes")]
    [InlineData("app/templates/index.html", "{% block title %}Orders · Shop{% endblock %}", "{% block title %}Order list · Shop{% endblock %}", "claim/page.title")]
    [InlineData("app/templates/index.html", "url_for('order_page', order_id=order.id)", "url_for('index')", "claim/intent.navigates")]
    [InlineData("app/templates/order.html", "path='status.js'", "path='other.js'", "claim/intent.uses")]
    [InlineData("app/templates/base.html", "{% block scripts %}{% endblock %}", "", "claim/region.outlet")]
    public void A_planted_break_is_one_finding_of_its_kind(string file, string from, string to, string rule)
    {
        var code = new Dictionary<string, string>(Code);
        code[file].IndexOf(from, StringComparison.Ordinal).ShouldBeGreaterThanOrEqualTo(0);
        code[file] = code[file].Replace(from, to);
        if (file == "app/services.py")
        {
            code[file] = code[file].Replace("from app.models import Order", "from app.models import Customer, Order");
        }

        var result = Run(code);

        var errors = Errors(result);
        errors.Count.ShouldBe(1, Describe(result));
        errors[0].Rule.ShouldBe(rule, Describe(result));
    }

    [Fact]
    public void A_form_sending_a_method_its_route_doesnt_answer_names_both()
    {
        var code = new Dictionary<string, string>(Code);
        code["app/web/routes.py"] = code["app/web/routes.py"].Replace("@router.post(\"/orders/{order_id}/notes\"", "@router.put(\"/orders/{order_id}/notes\"");
        var result = Run(code);

        var finding = Errors(result).Single(x => x.Rule == "claim/intent.invokes");
        finding.Message.ShouldBe("The form at `app/templates/order.html:4` sends POST to `/orders/{order_id}/notes`, but `routes.save_notes` answers PUT there: an HTML form sends only GET or POST. Give the operation a route the form can send, or link the form to the operation it reaches.");
    }

    [Fact]
    public void Python_reads_annotated_aliases_dunders_literals_and_setattr_as_a_type_checker_would()
    {
        var graph = Read();

        var api = graph.Types.Single(x => x.Name == "orders" && x.Kind == "module"); // app/api/orders.py is the module `orders`, as the model names it
        var getOrder = api.Members.Single(x => x.Name == "get_order");
        var service = getOrder.Parameters.Single(x => x.Name == "service");
        service.Type!.QualifiedName.ShouldBe("app.services.OrderService"); // OrderServiceDep, from app.dependencies
        service.Attributes.Single().Name.ShouldBe("Depends");
        var limit = api.Members.Single(x => x.Name == "list_orders").Parameters.Single(x => x.Name == "limit");
        limit.Type!.Name.ShouldBe("int");
        limit.Attributes.Single().Name.ShouldBe("Query");
        getOrder.Calls.Single(x => x.Name == "get").Target!.Type.Name.ShouldBe("OrderService");

        var customer = graph.Types.Single(x => x.Name == "Customer");
        graph.Unmodellable.Keys.ShouldContain(customer.Members.Single(x => x.Name == "__repr__"));
        graph.Unmodellable.Keys.ShouldContain(customer.Members.Single(x => x.Name == "__tablename__"));
        graph.Types.Single(x => x.Name == "NotFoundError").Members.Single(x => x.Name == "status_code").Type!.Name.ShouldBe("int");

        var update = graph.Types.Single(x => x.Name == "OrderService").Members.Single(x => x.Name == "update");
        update.Body!.Assignments.Single(x => x.Target == "order.field").Receiver!.Types.Single().Name.ShouldBe("Order");
    }

    [Fact]
    public void Only_html_the_python_renders_with_jinja_is_a_template()
    {
        var graph = Read();

        graph.Types.Where(x => x.Kind == "component").Select(x => x.Name).Order().ShouldBe(["base", "index", "order"]);

        var withoutRendering = Read(Code.Where(x => !x.Key.StartsWith("app/web/", StringComparison.Ordinal)).ToDictionary());
        withoutRendering.Types.ShouldNotContain(x => x.Kind == "component");
    }

    // ------------------------------------------------------------------ harness

    private static string? Link(Claim claim) => claim.Element;

    private static List<Finding> Errors(RunResult result) =>
        result.Findings.Where(x => x.Severity is "error" or "warning").ToList();

    private static Claim Claim(RunResult result, string element, string kind) => result.Claims.Single(x => x.Element == element && x.Kind == kind);

    private static string Describe(RunResult result) =>
        "\nClaims:\n" + string.Join("\n", result.Claims.Where(x => x.Outcome != "verified").Select(x => $"  {x.Kind} {x.Element} expected={x.Expected} -> {x.Outcome} {x.Reason} {x.Reading?.Value} {x.Reading?.At?.File}:{x.Reading?.At?.Line} {x.Reading?.Detail}"))
        + "\nFindings:\n" + string.Join("\n", result.Findings.Select(x => $"  {x.Rule} [{x.Severity}] {x.Subject.File}:{x.Subject.Line} {x.Subject.Element} {x.Message}"));

    private static CodeGraph Read(IReadOnlyDictionary<string, string>? code = null)
    {
        code ??= Code;
        var root = Write(code);
        try
        {
            var graph = new CodeGraph();
            new PythonExtension().Languages.Single().Read(root, code.Keys.Where(x => x.EndsWith(".py", StringComparison.Ordinal)).Order(StringComparer.Ordinal).ToList(), graph, TestContext.Current.CancellationToken);
            new JinjaExtension().Languages.Single().Read(root, code.Keys.Where(x => x.EndsWith(".html", StringComparison.Ordinal)).Order(StringComparer.Ordinal).ToList(), graph, TestContext.Current.CancellationToken);
            return graph;
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string Write(IReadOnlyDictionary<string, string> code)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-python-web-").FullName;
        foreach (var (path, source) in code)
        {
            var full = Path.Combine(root, path);
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            File.WriteAllText(full, source);
        }

        return root;
    }

    private static RunResult Run(IReadOnlyDictionary<string, string> code)
    {
        var root = Write(code);
        try
        {
            var designers = FixtureModel.Build(JsonNode.Parse(Model)!, out var references);
            var packs = new[] { "intent.core", "intent.domain", "intent.services", "intent.ui" }.Select(x => Pack.Stock(x)!).ToList();
            return DriftEngine.Run(new DriftInput(designers, root, packs, StockExtensions.All) { References = references });
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
