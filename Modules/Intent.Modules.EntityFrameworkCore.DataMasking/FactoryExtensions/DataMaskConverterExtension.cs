using System;
using System.Linq;
using System.Transactions;
using Intent.Engine;
using Intent.EntityFrameworkCore.DataMasking.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.DataMasking.Templates.DataMaskConverter;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.DataMasking.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DataMaskConverterExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.DataMasking.DataMaskConverterExtension";
        private readonly IMetadataManager _metadataManager;

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        public DataMaskConverterExtension(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var entityConfig = application
              .FindTemplateInstances<IIntentTemplate<ClassModel>>(
                  TemplateDependency.OnTemplate("Infrastructure.Data.EntityTypeConfiguration"))
              .Where(p => p.Model.Attributes.Any(a => a.HasDataMasking()))
              .Cast<ICSharpFileBuilderTemplate>()
              .ToArray();

            foreach (var entity in entityConfig)
            {
                entity.CSharpFile.AfterBuild(file =>
                {
                    var entityClass = file.Classes.First();

                    foreach (var ctor in entityClass.Constructors)
                    {
                        if (!ctor.Parameters.Any(p => p.Type == entity.GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface")))
                        {
                            ctor.AddParameter(entity.GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService", cfg =>
                            {
                                cfg.IntroduceReadonlyField();
                            });
                        }
                    }

                    if (!entityClass.Constructors.Any())
                    {
                        entityClass.AddConstructor(ctor =>
                        {
                            ctor.AddParameter(entity.GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService", cfg =>
                            {
                                cfg.IntroduceReadonlyField();
                            });
                        });
                    }

                    var configMethod = entityClass.FindMethod("Configure");

                    // this really should always return true
                    if (entity.TryGetModel(out ClassModel model))
                    {
                        var maskedAttributes = model.Attributes.Where(a => a.HasDataMasking()).ToList();
                        foreach (var attribute in maskedAttributes)
                        {
                            EfCoreFieldConfigStatement statement = (EfCoreFieldConfigStatement)configMethod.FindStatement(s => s.Text == $"builder.Property(x => x.{attribute.Name})");
                            statement?.AddStatement(GetConversionStatement(attribute, entity));
                        }
                    }
                });
            }
        }

        private static CSharpStatement GetConversionStatement(AttributeModel attribute, ICSharpFileBuilderTemplate template)
        {
            var maskedChar = string.IsNullOrWhiteSpace(attribute.GetDataMasking().MaskCharacter()) ? "*" : attribute.GetDataMasking().MaskCharacter();
            var maskLength = attribute.GetDataMasking().SetLength() <= 0 ? 5 : attribute.GetDataMasking().SetLength();
            var roles = string.Join("\",\"", GetRoles(attribute));
            var policies = string.Join("\",\"", GetPolicies(attribute));

            if (attribute.HasDataMasking() && attribute.GetDataMasking().DataMaskType().IsVariableLength())
            {
                var variableStatement = new CSharpInvocationStatement(".HasConversion").WithoutSemicolon()
                    .AddArgument(new CSharpInvocationStatement($"new {template.GetTypeName(DataMaskConverterTemplate.TemplateId)}").WithoutSemicolon()
                        .AddArgument("_currentUserService")
                        .AddArgument("MaskDataType.VariableLength")
                        .AddArgument($"\"{maskedChar}\"")
                        .AddArgument($"roles: [{roles}]")
                        .AddArgument($"policies: [{policies}]")
                        );

                return variableStatement;
            }

            if (attribute.HasDataMasking() && attribute.GetDataMasking().DataMaskType().IsSetLength())
            {
                var fixedStatement = new CSharpInvocationStatement(".HasConversion").WithoutSemicolon()
                     .AddArgument(new CSharpInvocationStatement($"new {template.GetTypeName(DataMaskConverterTemplate.TemplateId)}").WithoutSemicolon()
                         .AddArgument("_currentUserService")
                         .AddArgument("MaskDataType.SetLength")
                         .AddArgument($"\"{maskedChar}\"")
                         .AddArgument(maskLength.ToString())
                         .AddArgument($"roles: [{roles}]")
                         .AddArgument($"policies: [{policies}]"));

                return fixedStatement;
            }

            var prefixLength = attribute.GetDataMasking().UnmaskedPrefixLength() <= 0 ? 3 : attribute.GetDataMasking().UnmaskedPrefixLength();
            var suffixLength = attribute.GetDataMasking().UnmaskedSuffixLength() <= 0 ? 3 : attribute.GetDataMasking().UnmaskedSuffixLength();

            var partialStatement = new CSharpInvocationStatement(".HasConversion").WithoutSemicolon()
                     .AddArgument(new CSharpInvocationStatement("new DataMaskConverter").WithoutSemicolon()
                         .AddArgument("_currentUserService")
                         .AddArgument("MaskDataType.PartialMask")
                         .AddArgument($"\"{maskedChar}\"")
                         .AddArgument("0")
                         .AddArgument(prefixLength.ToString())
                         .AddArgument(suffixLength.ToString())
                         .AddArgument($"roles: [{roles}]")
                         .AddArgument($"policies: [{policies}]"));

            return partialStatement;
        }

        public static string[] GetRoles(AttributeModel attribute)
        {
            var commaSeparatedRoles = attribute.GetDataMasking().Roles();
            var roles = attribute.GetDataMasking().SecurityRoles();

            if (roles?.Length > 0)
            {
                return roles!.Select(x => $"\"{x.Name}\"").ToArray();
            }

            if (commaSeparatedRoles?.Length > 0)
            {
                return commaSeparatedRoles
                   .Split(',')
                   .Select(x => $"\"{x.Trim()}\"")
                   .Where(x => !string.IsNullOrWhiteSpace(x))
                   .ToArray();
            }

            return [];
        }

        public static string[] GetPolicies(AttributeModel attribute)
        {
            var commaSeparatedPolicies = attribute.GetDataMasking().Policy();
            var policies = attribute.GetDataMasking().SecurityPolicies();

            if (policies?.Length > 0)
            {
                return policies!.Select(x => $"\"{x.Name}\"").ToArray();
            }

            if (commaSeparatedPolicies?.Length > 0)
            {
                return commaSeparatedPolicies
                   .Split(',')
                   .Select(x => $"\"{x.Trim()}\"")
                   .Where(x => !string.IsNullOrWhiteSpace(x))
                   .ToArray();
            }

            return [];
        }

    }
}