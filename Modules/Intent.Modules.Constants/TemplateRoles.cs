using System;
using static Intent.Modules.Constants.TemplateRoles.Application;

namespace Intent.Modules.Constants
{
    public static class TemplateRoles
    {
        public static class Distribution
        {

            public static class Custom
            {
                public const string Dispatcher = "Distribution.Custom.Dispatcher";
            }

            public static class WebApi
            {
                public const string Controller = "Distribution.WebApi.Controller";
                public const string Startup = "App.Startup";
                public const string Program = "App.Program";
                public const string MultiTenancyConfiguration = "Configuration.MultiTenancy";
            }
        }
        public static class Domain
        {
            public const string Enum = "Domain.Enum";
            public const string UnitOfWork = "Domain.UnitOfWork";
            public const string MongoDbUnitOfWork = "Domain.UnitOfWork.MongoDb";
            public const string ValueObject = "Domain.ValueObject";
            public const string DataContract = "Domain.DataContract";
            public const string Events = "Domain.Events";
            public const string Specification = "Domain.Specification";

            public static class Entity
            {
                public const string Primary = "Domain.Entity";
                public const string Interface = "Domain.Entity.Interface";
                public const string EntityImplementation = "Domain.Entity.Primary";
                public const string State = "Domain.Entity.State";
                public const string Behaviour = "Domain.Entity.Behaviour";
            }

            public static class DomainServices
            {
                public const string Interface = "Domain.DomainServices.Interface";
                public const string Implementation = "Domain.DomainServices.Implementation";
            }
        }

        public static class Repository
        {
            [Obsolete("This template has been moved Infrastructure.Data.PagedList")]
            public const string PagedList = "Repository.Implementation.PagedList";

            public static class Interface
            {
                public const string Entity = "Repository.Interface.Entity";
                [Obsolete("This template has been moved Repository.Interface.PagedList")]
                public const string PagedResult = "Repository.Interface.PagedResult";
                public const string PagedList = "Repository.Interface.PagedList";
            }
        }

        public static class Application
        {
            public const string DependencyInjection = "Application.DependencyInjection";
            public const string Query = "Application.Query";
            public const string Command = "Application.Command";

            public static class Handler
            {
                public const string Command = "Application.Command.Handler";
                public const string Query = "Application.Query.Handler";
            }

            public static class Common
            {
                public const string DbContextInterface = "Application.Common.DbContextInterface";
                public const string ConnectionStringDbContextInterface = "Application.Common.ConnectionStringDbContextInterface";
                public const string ValidationServiceInterface = "Application.Common.ValidatonServiceInterface";
                public const string PagedList = "Application.Common.PagedList";
            }

            public static class Contracts
            {
                public const string Dto = "Application.Contract.Dto";
                public const string Enum = "Application.Contract.Enum";

                public static class Clients
                {
                    public const string Dto = "Application.Contracts.Clients.Dto";
                    public const string Enum = "Application.Contracts.Clients.Enum";
                }
            }

            public static class Services
            {
                [Obsolete("Use Distribution.WebApi.Controller")]
                public const string Controllers = "Intent.AspNetCore.Controllers.Controller";
                public const string Interface = "Application.Contracts";
                public const string Implementation = "Application.Implementation";
                public const string ClientInterface = "Application.Contracts.Clients";
            }

            public static class Eventing
            {
                public const string EventBusInterface = "Application.Eventing.EventBusInterface";
                public const string EventHandler = "Application.Eventing.EventHandler";
            }

            public static class Validation
            {
                public const string Dto = "Application.Validation.Dto";
                public const string Command = "Application.Validation.Command";
                public const string Query = "Application.Validation.Query";
            }

            public const string Mappings = "Application.Mappings";

            public static class DomainEventHandler
            {
                [Obsolete("Remove this in favor of Default")]
                public const string OldDefault = "Application.DomainEventHandler";
                public const string Default = "Application.DomainEventHandler.Default";
                public const string Explicit = "Application.DomainEventHandler.Explicit";
            }
        }

        public static class Infrastructure
        {
            public const string DependencyInjection = "Infrastructure.DependencyInjection";
            public static class Data
            {
                public const string DbContext = "Infrastructure.Data.DbContext";
                public const string ConnectionStringDbContext = "Infrastructure.Data.ConnectionStringDbContext";
                [Obsolete("This template has been moved Application.Common.PagedList")]
                public const string PagedList = "Infrastructure.Data.PagedList";
            }
        }

        public static class Blazor
        {
            [Obsolete("No longer will be used. Use Blazor.Client instead")]
            public static class WebAssembly
            {
                public const string Program = "Blazor.WebAssembly.Program";
            }
            public static class Client
            {
                public const string Program = "Blazor.Client.Program";
                public const string DependencyInjection = "Blazor.Client.DependencyInjection";

                public static class Model
                {
                    public const string Definition = "Blazor.Client.Model.Definition";
                    public const string Validator = "Blazor.Client.Model.Validator";
                }
            }

            public static class HttpClient
            {
                public static class Contracts
                {
                    public const string Dto = "Blazor.HttpClient.Contracts.Dto";
                }
            }
        }
    }
}
