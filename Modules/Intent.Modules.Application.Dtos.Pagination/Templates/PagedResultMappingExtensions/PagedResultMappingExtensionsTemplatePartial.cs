using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Pagination.Templates.PagedResultMappingExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PagedResultMappingExtensionsTemplate : CSharpTemplateBase<IList<DTOModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Dtos.Pagination.PagedResultMappingExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PagedResultMappingExtensionsTemplate(IOutputTarget outputTarget, IList<DTOModel> model) : base(TemplateId, outputTarget, model)
        {
            this.AddUsing("System");
            this.AddUsing("System.Linq");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("PagedResultMappingExtensions", @class =>
                {
                    @class.Static();
                    @class.AddMethod($"{this.GetPagedResultName()}<TDto>", "MapToPagedResult", mthd =>
                    {
                        mthd.Static();

                        mthd.AddGenericParameter("TDomain");
                        mthd.AddGenericParameter("TDto");

                        mthd.AddParameter($"{this.GetPagedListInterfaceName()}<TDomain>", "pagedList", prm =>
                        {
                            prm.WithThisModifier();
                        });

                        mthd.AddParameter("Func<TDomain, TDto>", "mapFunc");

                        mthd.AddObjectInitStatement("var data", "pagedList.Select(mapFunc).ToList();");
                        mthd.AddReturn($@"{this.GetPagedResultName()}<TDto>.Create(
                totalCount: pagedList.TotalCount,
                pageCount: pagedList.PageCount,
                pageSize: pagedList.PageSize,
                pageNumber: pagedList.PageNo,
                data: data)");

                        mthd.WithComments($@"/// <summary>
/// For mapping a paged-list of Domain elements into a page of DTO elements. See <see cref=""{this.GetPagedListInterfaceName()}{{T}}""/>.
/// </summary>
/// <param name=""pagedList"">A single page retrieved from a persistence store.</param>
/// <param name=""mapFunc"">
/// Provide a mapping function where a single Domain element is supplied to the function
/// that returns a single DTO element. There are some convenient mapping extension methods
/// available, or alternatively you can instantiate a new DTO element.
/// <example>results.MapToPagedResult(x => x.MapToItemDTO(_mapper));</example>
/// <example>results.MapToPagedResult(x => ItemDTO.Create(x.ItemName));</example>
/// </param>
/// <typeparam name=""TDomain"">Domain element type.</typeparam>
/// <typeparam name=""TDto"">DTO element type.</typeparam>
/// <returns>A single page of DTO elements.</returns>");
                    });

                    @class.AddMethod($"{this.GetPagedResultName()}<TDto>", "MapToPagedResult", mthd =>
                    {
                        mthd.Static();
                        mthd.AddGenericParameter("TDto");

                        mthd.AddParameter($"{this.GetPagedListInterfaceName()}<TDto>", "pagedList", prm =>
                        {
                            prm.WithThisModifier();
                        });

                        mthd.AddReturn($@"{this.GetPagedResultName()}<TDto>.Create(
                totalCount: pagedList.TotalCount,
                pageCount: pagedList.PageCount,
                pageSize: pagedList.PageSize,
                pageNumber: pagedList.PageNo,
                data: pagedList)");

                        mthd.WithComments($@"/// <summary>
/// For mapping a paged-list of Domain elements into a page of DTO elements. See <see cref=""{this.GetPagedListInterfaceName()}{{T}}""/>.
/// </summary>
/// <param name=""pagedList"">A single page retrieved from a persistence store.</param>
/// <typeparam name=""TDto"">DTO element type.</typeparam>
/// <returns>A single page of DTO elements.</returns>");

                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"PagedResultMappingExtensions",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private string GetDtoModelName(DTOModel dto)
        {
            return GetTypeName(Roles.Application.ContractDto, dto);
        }

        private string GetEntityName(DTOModel dto)
        {
            return GetTypeName(Roles.Domain.EntityInterface, dto.Mapping.ElementId);
        }

        private string GetPagedListInterfaceName()
        {
            return TryGetTypeName(TemplateRoles.Repository.Interface.PagedList, out var name) ? name
                : TryGetTypeName(TemplateRoles.Repository.Interface.PagedResult, out name) ? name // for backward compatibility
                : TryGetTypeName(TemplateRoles.Application.Common.PagedList, out name) ? name
                : TryGetTypeName(TemplateRoles.Infrastructure.Data.PagedList, out name) ? name // for backward compatibility
                : null;
        }

        public override bool CanRunTemplate()
        {
            return !string.IsNullOrWhiteSpace(GetPagedListInterfaceName());
        }
    }
}