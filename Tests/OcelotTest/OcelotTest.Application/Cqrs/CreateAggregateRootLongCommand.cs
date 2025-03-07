using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OcelotTest.Application.Cqrs
{
    // public class CreateAggregateRootLongCommand
    // {
    //     public CreateAggregateRootLongCommand()
    //     {
    //         Attribute = null!;
    //         CompositeOfAggrLong = null!;
    //     }
    //
    //     public string Attribute { get; set; }
    //     public CreateAggregateRootLongCompositeOfAggrLongDto CompositeOfAggrLong { get; set; }
    //
    //     public static CreateAggregateRootLongCommand Create(
    //         string attribute,
    //         CreateAggregateRootLongCompositeOfAggrLongDto compositeOfAggrLong)
    //     {
    //         return new CreateAggregateRootLongCommand
    //         {
    //             Attribute = attribute,
    //             CompositeOfAggrLong = compositeOfAggrLong
    //         };
    //     }
    // }
}