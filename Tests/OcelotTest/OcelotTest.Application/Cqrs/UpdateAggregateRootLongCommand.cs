using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OcelotTest.Application.Cqrs
{
    // public class UpdateAggregateRootLongCommand
    // {
    //     public UpdateAggregateRootLongCommand()
    //     {
    //         Attribute = null!;
    //         CompositeOfAggrLong = null!;
    //     }
    //
    //     public string Attribute { get; set; }
    //     public UpdateAggregateRootLongCompositeOfAggrLongDto CompositeOfAggrLong { get; set; }
    //
    //     public static UpdateAggregateRootLongCommand Create(
    //         string attribute,
    //         UpdateAggregateRootLongCompositeOfAggrLongDto compositeOfAggrLong)
    //     {
    //         return new UpdateAggregateRootLongCommand
    //         {
    //             Attribute = attribute,
    //             CompositeOfAggrLong = compositeOfAggrLong
    //         };
    //     }
    // }
}