namespace Intent.Modules.Constants
{
    public static class TemplateFulfillingRoles
    {
        public static class Domain
        {
            public const string UnitOfWork = "Domain.UnitOfWork";
            public static class Entity
            {
                public const string Primary = "Domain.Entity.Primary";
                public const string Interface = "Domain.Entity.Interface";
                public const string State = "Domain.Entity.State";
            }

        }

        public static class Application
        {
            public static class Common
            {
                public const string DbContextInterface = "Application.Common.DbContextInterface";
            }
        }

        public static class Infrastructure
        {
            public static class Data
            {
                public const string DbContext = "Infrastructure.Data.DbContext";
            }
        }
    }
}
