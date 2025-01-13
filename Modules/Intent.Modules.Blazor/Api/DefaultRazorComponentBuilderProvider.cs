using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Api;

public static class DefaultRazorComponentBuilderProvider
{
    private static readonly Dictionary<string, Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentBuilder>> FactoryRegister = new();
    private static readonly List<Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentInterceptor>> Interceptors = [];
    public static void Register(string elementSpecializationId, Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentBuilder> createBuilderFunc)
    {
        FactoryRegister[elementSpecializationId] = createBuilderFunc;
    }

    public static void AddInterceptor(Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentInterceptor> interceptor)
    {
        Interceptors.Add(interceptor);
    }

    public static IRazorComponentBuilderProvider Create(IRazorComponentTemplate template)
    {
        var provider = new RazorComponentBuilderProvider(template);
        foreach (var interceptor in Interceptors)
        {
            provider.AddInterceptor(interceptor(provider, template));
        }
        foreach (var item in FactoryRegister)
        {
            provider.Register(item.Key, item.Value(provider, template));
        }
        return provider;
    }
}

public interface IRazorComponentInterceptor
{
    void Handle(IElement component, IEnumerable<IRazorFileNode> razorNodes, IRazorFileNode node);
}
