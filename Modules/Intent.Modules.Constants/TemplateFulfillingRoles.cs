namespace Intent.Modules.Constants
{
    public static class TemplateFulfillingRoles
    {
        public static class Domain
        {
            public const string Enum = "Domain.Enum";
            public const string UnitOfWork = "Domain.UnitOfWork";
            public const string MongoDbUnitOfWork = "Domain.UnitOfWork.MongoDb";
            public const string ValueObject = "Domain.ValueObject";
            public const string DataContract = "Domain.DataContract";
            public static class Entity
            {
                public const string Primary = "Domain.Entity";
                public const string Interface = "Domain.Entity.Interface";
                public const string State = "Domain.Entity.State";
            }

            public static class DomainServices
            {
                public const string Interface = "Domain.DomainServices.Interface";
                public const string Implementation = "Domain.DomainServices.Implementation";
            }
        }

        public static class Repository
        {
            public const string PagedList = "Repository.Implementation.PagedList";
        }

        public static class Application
        {
            public const string DependencyInjection = "Application.DependencyInjection";

            public static class Common
            {
                public const string DbContextInterface = "Application.Common.DbContextInterface";
                public const string ValidationServiceInterface = "Application.Common.ValidatonServiceInterface";
            }

            public static class Contracts
            {
                public const string Dto = "Application.Contract.Dto";
            }
            
            public static class Services
            {
                public const string Controllers = "Intent.AspNetCore.Controllers.Controller";
            }
            
            public static class Eventing
            {
                public const string EventBusInterface = "Application.Eventing.EventBusInterface";
            }

            public static class Validation
            {
                public const string Dto = "Application.Validation.Dto";
            }
        }

        public static class Infrastructure
        {
            public const string DependencyInjection = "Infrastructure.DependencyInjection";
            public static class Data
            {
                public const string DbContext = "Infrastructure.Data.DbContext";
            }
        }
    }
}
