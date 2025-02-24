namespace Intent.Modules.Constants;
public static class Security
{
    // Role and Policy stereotype defined in Intent.Modules.Metadata.Security
    public const string Role = "d6ce4e4c-fdd1-4fa1-82d8-f3405e114ec5";
    public const string Policy = "9f37c60e-ad88-4484-98ca-6c9aa7ca3134";

    public static class Secured
    {
        public const string Id = "a9eade71-1d56-4be7-a80c-81046c0c978b";

        public static class Properties
        {
            public const string CommaSeparatedRoles = "2b39acef-6079-48c9-b85e-2b0981f9de70";
            public const string CommaSeparatedPolicies = "ae5251ff-40a1-4e46-be66-6b176f329f98";
            public const string Roles = "28bbe8bb-8d31-44c7-b642-ff0e279ab85f";
            public const string Policies = "68cbcd05-cd5c-49f3-a982-8ee9caf554bb";
        }
    }

    public static class Unsecured
    {
        public const string Id = "8b65c29e-1448-43ac-a92a-0e0f86efd6c6";
    }

    public static class DataMasking
    {
        public const string Id = "2c878051-d640-47d6-98bf-243fda0e60fb";

        public static class Properties
        {
            public const string CommaSeparatedRoles = "d525f5b5-10a3-43dc-85f5-6cbc527e4c76";
            public const string CommaSeparatedPolicies = "1f4d27c8-8f73-4967-a289-cad5110944fd";
            public const string Roles = "94865e3a-d9a7-4788-82f1-338c87f462b9";
            public const string Policies = "6a39a279-177c-480f-88fd-61d8e837fd68";
        }
    }
}
