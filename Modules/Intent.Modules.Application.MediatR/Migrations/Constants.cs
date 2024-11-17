namespace Intent.Modules.Application.MediatR.Migrations;

internal static class Constants
{
    public static class Elements
    {
        public const string Command = "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9";
        public const string Query = "e71b0662-e29d-4db2-868b-8a12464b25d0";
    }

    public static class Stereotypes
    {
        public static class Authorize
        {
            public const string DefinitionId = "b06358cd-aed3-4c39-96cf-abb131e4ecde";
            public const string PackageId = "e92495db-1b26-4389-bf06-cc1b375520b1";
            public const string PackageName = "Intent.Application.MediatR";

            public static class Property
            {
                public const string Roles = "98f96218-c5a7-4656-96b6-f173947028f7";
                public const string Policy = "ced1460d-1bd9-4ec2-92b3-51bdbf173315";
                public const string SecurityRoles = "9c5cd2b9-52d8-47a0-96a5-f0026c3b9e48";
                public const string SecurityPolicies = "2d95351e-d2e5-46da-8361-6c29bc690f98";
            }
        }

        public static class Security
        {
            public const string DefinitionId = "a9eade71-1d56-4be7-a80c-81046c0c978b";
            public const string PackageId = "a6fa1088-0064-43e3-a7fc-36c97b2b9285";
            public const string PackageName = "Intent.Metadata.Security";

            public static class Property
            {
                public const string Roles = "2b39acef-6079-48c9-b85e-2b0981f9de70";
                public const string Policy = "ae5251ff-40a1-4e46-be66-6b176f329f98";
                public const string SecurityRoles = "28bbe8bb-8d31-44c7-b642-ff0e279ab85f";
                public const string SecurityPolicies = "68cbcd05-cd5c-49f3-a982-8ee9caf554bb";
            }
        }
    }
}