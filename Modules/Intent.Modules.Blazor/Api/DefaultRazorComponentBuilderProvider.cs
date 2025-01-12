using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Api;

public static class DefaultRazorComponentBuilderProvider
{
    private static readonly Dictionary<string, ComponentBuilderRegistration> FactoryRegister = new();
    private static readonly List<InterceptorRegistration> Interceptors = [];

    public static void Register(string elementSpecializationId, Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentBuilder> createBuilderFunc)
    {
        FactoryRegister[elementSpecializationId] = new ComponentBuilderRegistration(
            factory: createBuilderFunc,
            configureRazor: null);
    }

    public static void Register<TRazorComponentBuilder>(string elementSpecializationId, Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, TRazorComponentBuilder> createBuilderFunc)
        where TRazorComponentBuilder : IConfigurableRazorComponentBuilder
    {
        FactoryRegister[elementSpecializationId] = new ComponentBuilderRegistration(
            factory: (provider, template) => createBuilderFunc(provider, template),
            configureRazor: TRazorComponentBuilder.ConfigureRazor);
    }

    public static void AddInterceptor(Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentInterceptor> interceptor)
    {
        Interceptors.Add(new InterceptorRegistration(
            factory: interceptor,
            configureRazor: null));
    }

    public static void AddInterceptor<TRazorComponentInterceptor>(Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentInterceptor> interceptor)
        where TRazorComponentInterceptor : IConfigurableRazorComponentInterceptor
    {
        Interceptors.Add(new InterceptorRegistration(
            factory: interceptor,
            configureRazor: TRazorComponentInterceptor.ConfigureRazor));
    }

    public static IRazorComponentBuilderProvider Create(IRazorComponentTemplate template)
    {
        var provider = new RazorComponentBuilderProvider(template);
        foreach (var interceptor in Interceptors)
        {
            provider.AddInterceptor(interceptor.Build(provider, template));
        }
        foreach (var item in FactoryRegister)
        {
            provider.Register(item.Key, item.Value.Build(provider, template));
        }
        return provider;
    }

    private class ComponentBuilderRegistration(
        Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentBuilder> factory,
        Action<IRazorConfigurator>? configureRazor)
    {
        private bool _configureRazorCalled;

        public IRazorComponentBuilder Build(IRazorComponentBuilderProvider provider, IRazorComponentTemplate template)
        {
            if (!_configureRazorCalled)
            {
                template.ExecutionContext.ConfigureRazor(c => configureRazor?.Invoke(c));
                _configureRazorCalled = true;
            }

            return factory(provider, template);
        }
    }

    private class InterceptorRegistration(
        Func<IRazorComponentBuilderProvider, IRazorComponentTemplate, IRazorComponentInterceptor> factory,
        Action<IRazorConfigurator>? configureRazor)
    {
        private bool _configureRazorCalled;

        public IRazorComponentInterceptor Build(IRazorComponentBuilderProvider provider, IRazorComponentTemplate template)
        {
            if (!_configureRazorCalled)
            {
                template.ExecutionContext.ConfigureRazor(c => configureRazor?.Invoke(c));
                _configureRazorCalled = true;
            }

            return factory(provider, template);
        }
    }
}

public interface IRazorComponentInterceptor
{
    void Handle(IElement component, IEnumerable<IRazorFileNode> razorNodes, IRazorFileNode node);
}

public interface IConfigurableRazorComponentInterceptor : IRazorComponentInterceptor
{
    static abstract void ConfigureRazor(IRazorConfigurator configurator);
}
