using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks.AspNetCore;
using Intent.Modules.Common.DriftEngine.Stacks.BeanValidation;
using Intent.Modules.Common.DriftEngine.Stacks.Blazor;
using Intent.Modules.Common.DriftEngine.Stacks.Constraints;
using Intent.Modules.Common.DriftEngine.Stacks.Cqrs;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Intent.Modules.Common.DriftEngine.Stacks.DataAnnotations;
using Intent.Modules.Common.DriftEngine.Stacks.EfCore;
using Intent.Modules.Common.DriftEngine.Stacks.FastApi;
using Intent.Modules.Common.DriftEngine.Stacks.Http;
using Intent.Modules.Common.DriftEngine.Stacks.Interactions;
using Intent.Modules.Common.DriftEngine.Stacks.Java;
using Intent.Modules.Common.DriftEngine.Stacks.Jinja;
using Intent.Modules.Common.DriftEngine.Stacks.Jpa;
using Intent.Modules.Common.DriftEngine.Stacks.Maui;
using Intent.Modules.Common.DriftEngine.Stacks.Mediator;
using Intent.Modules.Common.DriftEngine.Stacks.MediatR;
using Intent.Modules.Common.DriftEngine.Stacks.Messaging;
using Intent.Modules.Common.DriftEngine.Stacks.Mvc;
using Intent.Modules.Common.DriftEngine.Stacks.MvvmToolkit;
using Intent.Modules.Common.DriftEngine.Stacks.Protobuf;
using Intent.Modules.Common.DriftEngine.Stacks.Python;
using Intent.Modules.Common.DriftEngine.Stacks.React;
using Intent.Modules.Common.DriftEngine.Stacks.Proxies;
using Intent.Modules.Common.DriftEngine.Stacks.Relational;
using Intent.Modules.Common.DriftEngine.Stacks.Serialization;
using Intent.Modules.Common.DriftEngine.Stacks.SqlAlchemy;
using Intent.Modules.Common.DriftEngine.Stacks.SpringMvc;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScript;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScriptUi;
using Intent.Modules.Common.DriftEngine.Stacks.Vue;
using Intent.Modules.Common.DriftEngine.Stacks.Wolverine;
using Intent.Modules.Common.DriftEngine.Stacks.Xaml;

namespace Intent.Modules.Common.DriftEngine.Stacks;

/// <summary>
/// The extensions shipped with the engine. A language is one (CSharp, XAML, Java, Python, TypeScript): the engine reads no code itself. What the model
/// states is technology-neutral (Relational, Http, Constraints, Cqrs); how one framework's code states it is a separate extension
/// (EfCore, Jpa and SqlAlchemy, AspNetCore, SpringMvc and FastApi, DataAnnotations and BeanValidation, MediatR, Mediator and Wolverine; Blazor,
/// MVC views, Jinja templates, .NET MAUI, the MVVM Toolkit, Vue and React for the User Interface designer), each reading the same claims with no
/// change to the others. A framework's extension answers only when its framework is in the code; otherwise it leaves the claim to another,
/// or can't verify.
/// </summary>
public static class StockExtensions
{
    /// <summary>The stock extensions, with TypeScript's parser run by the machine's <c>node</c>.</summary>
    public static IReadOnlyList<IDriftExtension> All { get; } = With(MachineNodeRunner.Instance);

    /// <summary>The stock extensions, with TypeScript's parser run by the given Node.js: Intent Architect's own in a Software Factory run.</summary>
    public static IReadOnlyList<IDriftExtension> With(INodeRunner node) =>
    [
        new CSharpExtension(),
        new XamlExtension(),
        new JavaExtension(),
        new PythonExtension(),
        new TypeScriptExtension(node),
        new ProtobufExtension(),
        new GrpcExtension(),
        new RelationalExtension(),
        new HttpExtension(),
        new ConstraintsExtension(),
        new EfCoreExtension(),
        new AspNetCoreExtension(),
        new JpaExtension(),
        new SpringMvcExtension(),
        new DataAnnotationsExtension(),
        new BeanValidationExtension(),
        new CqrsExtension(),
        new MediatRExtension(),
        new MediatorExtension(),
        new WolverineExtension(),
        new MessagingExtension(),
        new InteractionsExtension(),
        new SqlAlchemyExtension(),
        new FastApiExtension(),
        new SerializationExtension(),
        new BlazorExtension(),
        new MvcExtension(),
        new JinjaExtension(),
        new MvvmToolkitExtension(),
        new MauiExtension(),
        new TypeScriptUiExtension(),
        new VueExtension(),
        new ReactExtension(),
        new ProxiesExtension()
    ];

    /// <summary>A stock extension by its id, or null.</summary>
    public static IDriftExtension? Find(string id) => All.FirstOrDefault(x => x.Id == id);

    internal static Pack LoadPack(string id)
    {
        using var stream = typeof(StockExtensions).Assembly.GetManifestResourceStream($"{id}.pack.json")
                           ?? throw new InvalidOperationException($"The {id} pack is missing from the stock extensions' resources.");
        return Pack.Parse(JsonNode.Parse(stream)!);
    }
}
