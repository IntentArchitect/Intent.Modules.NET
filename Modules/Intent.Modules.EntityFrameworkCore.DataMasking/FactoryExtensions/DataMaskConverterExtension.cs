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
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.DataMasking.Templates.DataMaskConverter;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.Modules.Security.Shared;
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
            var maskedChar = string.IsNullOrWhiteSpace(attribute.GetDataMasking().MaskCharacter()) ? '*' : attribute.GetDataMasking().MaskCharacter().First();
            var maskLength = attribute.GetDataMasking().FixedLength() <= 0 ? 5 : attribute.GetDataMasking().FixedLength();

            var permissionOptions = new PermissionConversionOptions
            {
                ConvertedCollectionFormat = "{0}",
                UnconvertedCollectionFormat = "{0}",
                ConvertedNameFormat = "{0}",
                UnconvertedNameFormat = "\"{0}\""
            };

            var roles = SecurityHelper.RolesToPermissionConstants(GetRoles(attribute), template, permissionOptions);
            var policies = SecurityHelper.PoliciesToPermissionConstants(GetPolicies(attribute), template, permissionOptions);

            if (attribute.HasDataMasking() && attribute.GetDataMasking().DataMaskType().IsVariableLength())
            {
                var variableInvoc = new CSharpInvocationStatement($"{template.GetTypeName(DataMaskConverterTemplate.TemplateId)}.VariableLength").WithoutSemicolon()
                        .AddArgument("_currentUserService")
                        .AddArgument($"'{maskedChar}'");

                if (!string.IsNullOrWhiteSpace(roles))
                {
                    variableInvoc.AddArgument("roles", $"[{roles}]");
                }

                if (!string.IsNullOrWhiteSpace(policies))
                {
                    variableInvoc.AddArgument("policies", $"[{policies}]");
                }

                var variableStatement = new CSharpInvocationStatement(".HasConversion").WithoutSemicolon()
                    .AddArgument(variableInvoc);

                return variableStatement;
            }

            if (attribute.HasDataMasking() && attribute.GetDataMasking().DataMaskType().IsFixedLength())
            {
                var fixedInvoc = new CSharpInvocationStatement($"{template.GetTypeName(DataMaskConverterTemplate.TemplateId)}.FixedLength").WithoutSemicolon()
                         .AddArgument("_currentUserService")
                         .AddArgument($"'{maskedChar}'")
                         .AddArgument(maskLength.ToString());

                if (!string.IsNullOrWhiteSpace(roles))
                {
                    fixedInvoc.AddArgument("roles", $"[{roles}]");
                }

                if (!string.IsNullOrWhiteSpace(policies))
                {
                    fixedInvoc.AddArgument("policies", $"[{policies}]");
                }

                var fixedStatement = new CSharpInvocationStatement(".HasConversion").WithoutSemicolon()
                     .AddArgument(fixedInvoc);

                return fixedStatement;
            }

            var prefixLength = attribute.GetDataMasking().UnmaskedPrefixLength() <= 0 ? 3 : attribute.GetDataMasking().UnmaskedPrefixLength();
            var suffixLength = attribute.GetDataMasking().UnmaskedSuffixLength() <= 0 ? 3 : attribute.GetDataMasking().UnmaskedSuffixLength();

            var partialInvoc = new CSharpInvocationStatement($"{template.GetTypeName(DataMaskConverterTemplate.TemplateId)}.Partial").WithoutSemicolon()
                .AddArgument("_currentUserService")
                .AddArgument($"'{maskedChar}'")
                .AddArgument(prefixLength.ToString())
                .AddArgument(suffixLength.ToString());

            if (!string.IsNullOrWhiteSpace(roles))
            {
                partialInvoc.AddArgument("roles", $"[{roles}]");
            }

            if (!string.IsNullOrWhiteSpace(policies))
            {
                partialInvoc.AddArgument("policies", $"[{policies}]");
            }

            var partialStatement = new CSharpInvocationStatement(".HasConversion")
                .WithoutSemicolon()
                .AddArgument(partialInvoc);

            return partialStatement;
        }

        public static string[] GetRoles(AttributeModel attribute)
        {
            var commaSeparatedRoles = attribute.GetDataMasking().Roles();
            var roles = attribute.GetDataMasking().SecurityRoles();

            if (roles?.Length > 0)
            {
                return roles!.Select(x => $"{x.Name}").ToArray();
            }

            if (commaSeparatedRoles?.Length > 0)
            {
                return commaSeparatedRoles
                   .Split(',')
                   .Select(x => $"{x.Trim()}")
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
                return policies!.Select(x => $"{x.Name}").ToArray();
            }

            if (commaSeparatedPolicies?.Length > 0)
            {
                return commaSeparatedPolicies
                   .Split(',')
                   .Select(x => $"{x.Trim()}")
                   .Where(x => !string.IsNullOrWhiteSpace(x))
                   .ToArray();
            }

            return [];
        }

    }
}