using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Customers;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.SubscriptionType", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.Subscriptions
{
    [ExtendObjectType(OperationType.Subscription)]
    public class Subscription
    {
        [Subscribe]
        public CustomerDto CustomerCreated([EventMessage] CustomerDto customer)
        {
            return customer;
        }
    }
}