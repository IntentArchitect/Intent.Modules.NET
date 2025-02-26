using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Output;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins;
using Intent.Utils;

namespace Intent.Modules.AI.Prompts.Tasks;

public class GetSettingsTask : IModuleTask
{
    private readonly IMetadataManager _metadataManager;
    private readonly ISolutionConfig _solution;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GetSettingsTask(IMetadataManager metadataManager, ISolutionConfig solution)
    {
        _metadataManager = metadataManager;
        _solution = solution;
    }

    public string TaskTypeId => "Intent.Modules.AI.Prompts.CreateMediatRHandlerPrompt";
    public string TaskTypeName => "Create Prompt for Handler";
    public int Order => 0;

    public string Execute(params string[] args)
    {
        var applicationConfig = _solution.GetApplicationConfig(args[0]);
        var basePath = Path.Combine(applicationConfig.DirectoryPath, applicationConfig.OutputLocation);
        var correlations = MetadataOutputFileCorrelationsPersistable.TryLoad(applicationConfig.FilePath);
        var queryModel = _metadataManager.Services(args[0]).GetQueryModels().Single(x => x.Id == args[1]);

        var fileMap = correlations.Files
            .SelectMany(x => x.Models, (file, model) => (File: file, Model: model))
            .GroupBy(x => x.Model.Id)
            .ToDictionary(x => x.Key, x => x.Select(i => Path.Combine(basePath, i.File.RelativePath)).ToList());

        var correlatedFiles = CorrelatedFiles(fileMap, queryModel.Id);

        var queryHandlerFile = correlatedFiles.First(x => x.FileName.EndsWith("Handler"));
        var otherCorrelatedFiles = correlatedFiles.Where(x => !x.FileName.EndsWith("Handler") && Path.GetExtension(x.Path) == ".cs");
        var returnTypeFile = CorrelatedFiles(fileMap, queryModel.TypeReference.ElementId);
        return @$"
I'm going to provide you with a C# class called {queryHandlerFile.FileName} that is a handler for a MediatR query. 
I want you to implement the Handle method to fetch out the {queryHandlerFile.FileName.RemovePrefix("Get").RemoveSuffix("Query", "Handler").ToSentenceCase()} appropriately using best practices.

I want you to return the updated handler class, {queryHandlerFile.FileName}, and only that class, unless you made an update to a repository in which case you must return that too. Do not give me anything but just the code. No explanation.

If there is no repository method that would be able to fetch out the required results in a performant way, then you may update the entity repository and its interface with an appropriate method. You may need to introduce a Domain Object as the return type of these methods.
In this case, if you added any methods, then you must add the [IntentIgnore] attribute to that method.
You can inject in any entity repository that you may require.
You must keep all attributes unchanged on the class or its methods.

The architecture is a clean architecture using EF Core and a Repository pattern. The repository interfaces are in the Domain / Core layer and so cannot reference types in the other layers.

The current implementation of the {queryHandlerFile.FileName} class is as follows:
{queryHandlerFile.FileText}

The other files related to this handler are as follows:
{string.Join(Environment.NewLine + Environment.NewLine, otherCorrelatedFiles.Select(x => x.FileText))}

{(returnTypeFile.Count > 0 ? string.Join(Environment.NewLine + Environment.NewLine, returnTypeFile.Select(x => x.FileText)) : "")}

{string.Join(Environment.NewLine + Environment.NewLine, GetRelatedEntities(queryModel).Select(x => $@"
The files for the {x.Name} entity is as follows:
{string.Join(Environment.NewLine + Environment.NewLine, CorrelatedFiles(fileMap, x.Id).Select(x => x.FileText))}"))}

Your base repository interface is as follows:
public interface IEFRepository<TDomain, TPersistence> : IRepository<TDomain>
{{
    IUnitOfWork UnitOfWork {{ get; }}
    Task<TDomain?> FindAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<TDomain?> FindAsync(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<TDomain?> FindAsync(Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
    Task<List<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<List<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
    Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default);
    Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<TDomain>> FindAllAsync(Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int pageSize, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Func<IQueryable<TPersistence>, IQueryable<TPersistence>>? queryOptions = default, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Func<IQueryable<TPersistence>, IQueryable<TPersistence>>? queryOptions = default, CancellationToken cancellationToken = default);
    Task<List<TProjection>> FindAllProjectToAsync<TProjection>(CancellationToken cancellationToken = default);
    Task<List<TProjection>> FindAllProjectToAsync<TProjection>(Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<TProjection>> FindAllProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<List<TProjection>> FindAllProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<IPagedList<TProjection>> FindAllProjectToAsync<TProjection>(int pageNo, int pageSize, CancellationToken cancellationToken = default);
    Task<IPagedList<TProjection>> FindAllProjectToAsync<TProjection>(int pageNo, int pageSize, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<IPagedList<TProjection>> FindAllProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default);
    Task<IPagedList<TProjection>> FindAllProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<TProjection?> FindProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<TProjection?> FindProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<TProjection?> FindProjectToAsync<TProjection>(Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
}}";
        //try
        //{
        //    var settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Intent Architect", "chatdrivendomain-settings.json");
        //    if (!Path.Exists(settingsFilePath))
        //    {
        //        return JsonSerializer.Serialize(new SettingsResult { SettingsFileExists = false }, SerializerOptions);
        //    }

        //    var settingsData = JsonSerializer.Deserialize<SettingsData>(File.ReadAllText(settingsFilePath), SerializerOptions);
        //    var result = JsonSerializer.Serialize(new SettingsResult { SettingsFileExists = true, Data = settingsData }, SerializerOptions);
        //    return result;
        //}
        //catch (Exception e)
        //{
        //    Logging.Log.Failure(e);
        //    return Fail(e.GetBaseException().Message);
        //}
    }

    private static List<(string Path, string FileName, string FileText)> CorrelatedFiles(Dictionary<string, List<string>> fileMap, string modelId)
    {
        return fileMap[modelId].Select(x => (Path: x, FileName: Path.GetFileNameWithoutExtension(x), FileText: File.ReadAllText(x))).ToList();
    }

    private IEnumerable<ClassModel> GetRelatedEntities(QueryModel queryModel)
    {
        var queriedEntity = queryModel.QueryEntityActions().FirstOrDefault()?.TypeReference.Element.AsClassModel();
        if (queriedEntity == null)
        {
            return [];
        }
        var sb = new StringBuilder();
        var relatedClasses = new[] { queriedEntity }.Concat(queriedEntity.AssociatedClasses.Where(x => x.Class != null).Select(x => x.Class));
        return relatedClasses;
    }
}