using System;
using System.Collections.Generic;

namespace Intent.Modules.Blazor.Api;

public static class DefaultRazorComponentBuilderProvider
{
    private static Dictionary<string, Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentBuilder>> _factoryRegister = new();
    public static void Register(string elementSpecializationId, Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentBuilder> createBuilderFunc)
    {
        _factoryRegister[elementSpecializationId] = createBuilderFunc;
    }

    public static IRazorComponentBuilderProvider Create(IRazorComponentTemplate template)
    {
        var provider = new RazorComponentBuilderProvider(template);
        foreach (var item in _factoryRegister)
        {
            provider.Register(item.Key, item.Value(provider, template));
        }
        return provider;
    }
}
