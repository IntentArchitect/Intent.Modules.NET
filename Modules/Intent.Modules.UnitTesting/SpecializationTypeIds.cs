using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.UnitTesting
{
    /// <summary>
    /// Specialization Type IDs for unit test template filtering.
    /// These constants represent the specialization types from various Intent Architect modelers.
    /// </summary>
    internal static class SpecializationTypeIds
    {
        // Services.CQRS modeler specializations
        public const string Command = "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9";
        public const string Query = "e71b0662-e29d-4db2-868b-8a12464b25d0";
        
        // Services modeler specializations  
        public const string Service = "b16578a5-27b1-4047-a8df-f0b783d706bd";
        public const string ServiceOperation = "e030c97a-e066-40a7-8188-808c275df3cb";
        public const string Parameter = "00208d20-469d-41cb-8501-768fd5eb796b";
        
        // Domain.Events modeler specializations
        public const string DomainEventHandler = "d80e61c5-7e4c-4175-9df1-0413f664824c";
        
        // EventInteractions modeler specializations
        public const string IntegrationEventHandler = "c0582230-22f5-4f74-8eb0-ec6fc9364900";

        public const string DomainService = "07f936ea-3756-48c8-babd-24ac7271daac";
        public const string DomainServiceOperation = "e042bb67-a1df-480c-9935-b26210f78591";
        
        // Common specializations (properties on Query/Command)
        public const string Property = "7baed1fd-469b-4980-8fd9-4cefb8331eb2";
        
        // Stereotypes
        public const string UnitTestStereotype = "4965bed2-6320-49d1-beba-0fbc6fd4dfe6";
    }
}
