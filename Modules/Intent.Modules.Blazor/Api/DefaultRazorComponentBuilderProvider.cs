using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Api;

public static class DefaultRazorComponentBuilderProvider
{
    private static Dictionary<string, Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentBuilder>> _factoryRegister = new();
    private static List<Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentInterceptor>> _interceptors = [];
    public static void Register(string elementSpecializationId, Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentBuilder> createBuilderFunc)
    {
        _factoryRegister[elementSpecializationId] = createBuilderFunc;
    }

    public static void AddInterceptor(Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentInterceptor> interceptor)
    {
        _interceptors.Add(interceptor);
    }

    public static IRazorComponentBuilderProvider Create(IRazorComponentTemplate template)
    {
        var provider = new RazorComponentBuilderProvider(template);
        foreach (var interceptor in _interceptors)
        {
            provider.AddInterceptor(interceptor(provider, template));
        }
        foreach (var item in _factoryRegister)
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
