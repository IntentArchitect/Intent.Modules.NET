using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Pagination.Templates.CursorPagedResultMappingExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CursorPagedResultMappingExtensionsTemplate : CSharpTemplateBase<IList<DTOModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Dtos.Pagination.CursorPagedResultMappingExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CursorPagedResultMappingExtensionsTemplate(IOutputTarget outputTarget, IList<DTOModel> model) : base(TemplateId, outputTarget, model)
        {
            this.AddUsing("System");
            this.AddUsing("System.Linq");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"CursorPagedResultMappingExtensions", @class =>
                {
                    @class.Static();
                    @class.AddMethod($"{this.GetCursorPagedResultName()}<TDto>", "MapToCursorPagedResult", mthd =>
                    {
                        mthd.Static();

                        mthd.AddGenericParameter("TDomain");
                        mthd.AddGenericParameter("TDto");

                        mthd.AddParameter($"{this.GetCursorPagedListInterfaceName()}<TDomain>", "pagedList", prm =>
                        {
                            prm.WithThisModifier();
                        });

                        mthd.AddParameter("Func<TDomain, TDto>", "mapFunc");

                        mthd.AddObjectInitStatement("var data", "pagedList.Select(mapFunc).ToList();");
                        mthd.AddReturn($@"{this.GetCursorPagedResultName()}<TDto>.Create(
                pageSize: pagedList.PageSize,
                cursorToken: pagedList.CursorToken,
                data: data)");

                        mthd.WithComments($@"/// <summary>
/// For mapping a cursor based paged-list of Domain elements into a page of DTO elements. See <see cref=""{this.GetCursorPagedListInterfaceName()}{{T}}""/>.
/// </summary>
/// <param name=""pagedList"">A single page retrieved from a persistence store.</param>
/// <param name=""mapFunc"">
/// Provide a mapping function where a single Domain element is supplied to the function
/// that returns a single DTO element. There are some convenient mapping extension methods
/// available, or alternatively you can instantiate a new DTO element.
/// <example>results.MapToCursorPagedResult(x => x.MapToItemDTO(_mapper));</example>
/// <example>results.MapToCursorPagedResult(x => ItemDTO.Create(x.ItemName));</example>
/// </param>
/// <typeparam name=""TDomain"">Domain element type.</typeparam>
/// <typeparam name=""TDto"">DTO element type.</typeparam>
/// <returns>A single page of DTO elements.</returns>");
                    });

                    @class.AddMethod($"{this.GetCursorPagedResultName()}<TDto>", "MapToCursorPagedResult", mthd =>
                    {
                        mthd.Static();
                        mthd.AddGenericParameter("TDto");

                        mthd.AddParameter($"{this.GetCursorPagedListInterfaceName()}<TDto>", "pagedList", prm =>
                        {
                            prm.WithThisModifier();
                        });

                        mthd.AddReturn($@"{this.GetCursorPagedResultName()}<TDto>.Create(
                pageSize: pagedList.PageSize,
                cursorToken: pagedList.CursorToken,
                data: pagedList)");

                        mthd.WithComments($@"/// <summary>
/// For mapping a paged-list of Domain elements into a page of DTO elements. See <see cref=""{this.GetCursorPagedListInterfaceName()}{{T}}""/>.
/// </summary>
/// <param name=""pagedList"">A single page retrieved from a persistence store.</param>
/// <typeparam name=""TDto"">DTO element type.</typeparam>
/// <returns>A single page of DTO elements.</returns>");

                    });

                    AddUsing("System.Linq.Expressions");
                    @class.AddMethod($"Expression<{UseType("System.Func<T, bool>")}>", "Combine", method =>
                    {
                        method.Static();
                        method.AddGenericParameter("T");

                        method.AddParameter("Expression<Func<T, bool>>", "first", prm =>
                        {
                            prm.WithThisModifier();
                        });
                        method.AddParameter("Expression<Func<T, bool>>", "second");

                        method.AddObjectInitStatement("var param", "Expression.Parameter(typeof(T));");
                        method.AddObjectInitStatement("var body", "Expression.AndAlso(Expression.Invoke(first, param), Expression.Invoke(second, param));");

                        method.AddReturn("Expression.Lambda<Func<T, bool>>(body, param)");
                    });

                });
        }

        public override bool CanRunTemplate()
        {
            var cursorResult = "CursorPagedResult";
            var isPagingUsed = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id)
                        .GetQueryModels().Any(x => x.TypeReference?.Element?.Name == cursorResult) ||
                    ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id)
                        .GetServiceModels().Any(x => x.Operations.Any(o => o.TypeReference?.Element?.Name == cursorResult));

            return TryGetTypeName(TemplateRoles.Repository.Interface.CursorPagedList, out var interfaceName) 
                && !string.IsNullOrWhiteSpace(interfaceName)
                && isPagingUsed;
        }

        private string GetCursorPagedListInterfaceName()
        {
            return TryGetTypeName(TemplateRoles.Repository.Interface.CursorPagedList, out var interfaceName) ? interfaceName : "";
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}