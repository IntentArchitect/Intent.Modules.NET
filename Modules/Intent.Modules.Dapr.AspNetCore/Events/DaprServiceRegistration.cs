using System;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Dapr.AspNetCore.Events
{
    public class DaprServiceRegistration
    {
        public DaprServiceRegistration(
            Func<CSharpTemplateBase<object>, string> serviceTypeResolver,
            Func<CSharpTemplateBase<object>, string> implementationTypeResolver,
            Func<CSharpTemplateBase<object>, string> implementationFactoryResolver)
        {
            ServiceTypeResolver = serviceTypeResolver;
            ImplementationTypeResolver = implementationTypeResolver;
            ImplementationFactoryResolver = implementationFactoryResolver;
        }

        public Func<CSharpTemplateBase<object>, string> ServiceTypeResolver { get; }
        public Func<CSharpTemplateBase<object>, string> ImplementationTypeResolver { get; }
        public Func<CSharpTemplateBase<object>, string> ImplementationFactoryResolver { get; }
    }
}
