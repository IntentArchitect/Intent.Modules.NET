using System;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates;

public static class StatementExtensions
{
    [Obsolete("TODO REMOVE")]
    public static void RegisterDomainEventBaseFixture(this ICSharpFileBuilderTemplate template, CSharpClassMethod method)
    {
        RegisterDomainEventBaseFixture(template, method, null);
    }

    [Obsolete("TODO REMOVE")]
    public static void RegisterDomainEventBaseFixture(this ICSharpFileBuilderTemplate template, CSharpClassMethod method, ClassModel domainModel)
    {
        if (!template.TryGetTypeName("Intent.DomainEvents.DomainEventBase", out var domainEventBaseName))
        {
            return;
        }

        template.AddTypeSource("Intent.DomainEvents.DomainEventBase");
        method.AddStatement($@"fixture.Register<{domainEventBaseName}>(() => null);");

        if (domainModel == null)
        {
            return;
        }

        method.AddStatement($@"fixture.Customize<{template.GetTypeName(domainModel.InternalElement)}>(comp => comp.Without(x => x.DomainEvents));");
    }
}