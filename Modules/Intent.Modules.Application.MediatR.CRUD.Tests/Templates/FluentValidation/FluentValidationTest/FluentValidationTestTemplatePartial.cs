using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Application.FluentValidation.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.FluentValidation.Templates;
using Intent.Modules.Application.MediatR.FluentValidation.Templates.CommandValidator;
using Intent.Modules.Application.MediatR.FluentValidation.Templates.ValidationBehaviour;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.FluentValidation.FluentValidationTest;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class FluentValidationTestTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public FluentValidationTestTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.AutoFixture);
        AddNugetDependency(NugetPackages.FluentAssertions);
        AddNugetDependency(NugetPackages.MicrosoftNetTestSdk);
        AddNugetDependency(NugetPackages.NSubstitute);
        AddNugetDependency(NugetPackages.Xunit);
        AddNugetDependency(NugetPackages.XunitRunnerVisualstudio);

        AddTypeSource(TemplateRoles.Domain.Entity.Primary);
        AddTypeSource(TemplateRoles.Domain.Enum);
        AddTypeSource(CommandModelsTemplate.TemplateId);
        AddTypeSource(TemplateRoles.Application.Contracts.Dto);

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddClass($"{Model.Name}ValidatorTests")
            .OnBuild(file =>
            {
                file.AddUsing("System");
                file.AddUsing("System.Collections.Generic");
                file.AddUsing("System.Linq");
                file.AddUsing("System.Threading");
                file.AddUsing("System.Threading.Tasks");
                file.AddUsing("AutoFixture");
                file.AddUsing("FluentAssertions");
                file.AddUsing("NSubstitute");
                file.AddUsing("Xunit");
                file.AddUsing("MediatR");
                file.AddUsing("FluentValidation");

                var domainElement = Model.Mapping.Element.AsClassModel();
                var domainIdAttr = domainElement.GetEntityPkAttribute(ExecutionContext);
                var isCommandWithReturnId = Model.Name.Contains("create", StringComparison.OrdinalIgnoreCase)
                                            && Model.TypeReference.Element != null;

                var priClass = file.Classes.First();
                priClass.AddMethod("IEnumerable<object[]>", "GetSuccessfulResultTestData", method =>
                {
                    method.Static();
                    method.AddStatements($@"
        var fixture = new Fixture();
        var testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();");
                    AddSuccessTestDataFields(method);
                    method.AddStatement($@"yield return new object[] {{ testCommand }};");
                });

                priClass.AddMethod("Task", "Validate_WithValidCommand_PassesValidation", method =>
                {
                    method.Async();
                    method.AddAttribute("Theory");
                    method.AddAttribute("MemberData(nameof(GetSuccessfulResultTestData))");
                    method.AddParameter(GetTypeName(Model.InternalElement), "testCommand");
                    method.AddStatements($@"
        // Arrange
        var validator = GetValidationBehaviour();");
                    if (isCommandWithReturnId)
                    {
                        method.AddStatement($@"var expectedId = new Fixture().Create<{domainIdAttr.Type}>();");
                    }

                    method.AddStatements($@"
        // Act
        var result = await validator.Handle(testCommand, () => Task.FromResult({(isCommandWithReturnId ? "expectedId" : "Unit.Value")}), CancellationToken.None);

        // Assert
        result.Should().Be({(isCommandWithReturnId ? "expectedId" : "Unit.Value")});");
                });

                priClass.AddMethod("IEnumerable<object[]>", "GetFailedResultTestData", method =>
                {
                    method.Static();
                    AddFailingTestData(method);
                });

                priClass.AddMethod("Task", "Validate_WithInvalidCommand_FailsValidation", method =>
                {
                    method.Async();
                    method.AddAttribute("Theory");
                    method.AddAttribute("MemberData(nameof(GetFailedResultTestData))");
                    method.AddParameter(GetTypeName(Model.InternalElement), "testCommand");
                    method.AddParameter("string", "expectedPropertyName");
                    method.AddParameter("string", "expectedPhrase");
                    method.AddStatements($@"
        // Arrange
        var validator = GetValidationBehaviour();");
                    if (isCommandWithReturnId)
                    {
                        method.AddStatement($@"var expectedId = new Fixture().Create<{domainIdAttr.Type}>();");
                    }

                    method.AddStatements($@"
        // Act
        var act = async () => await validator.Handle(testCommand, () => Task.FromResult({(isCommandWithReturnId ? "expectedId" : "Unit.Value")}), CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<ValidationException>().Result
            .Which.Errors.Should().Contain(x => x.PropertyName == expectedPropertyName && x.ErrorMessage.Contains(expectedPhrase));");
                });

                // No valid test data added? Then remove those test cases as it will cause some compilation errors.
                if (!priClass.FindMethod("GetFailedResultTestData").Statements.Any())
                {
                    priClass.Methods.Remove(priClass.FindMethod("GetFailedResultTestData"));
                    priClass.Methods.Remove(priClass.FindMethod("Validate_WithInvalidCommand_FailsValidation"));
                }
            })
            .AfterBuild(file =>
            {
                var priClass = file.Classes.First();
                var domainElement = Model.Mapping.Element.AsClassModel();
                var domainIdAttr = domainElement.GetEntityPkAttribute(ExecutionContext);
                var isCommandWithReturnId = Model.Name.Contains("create", StringComparison.OrdinalIgnoreCase)
                                            && Model.TypeReference.Element != null;

                priClass.AddMethod($"{this.GetValidationBehaviourName()}<{GetTypeName(Model.InternalElement)}, {(isCommandWithReturnId ? domainIdAttr.Type : "Unit")}>",
                    "GetValidationBehaviour", method =>
                    {
                        method.Private();

                        var mainValidatorInstantiation = new CSharpInvocationStatement($"new {this.GetCommandValidatorName(Model)}")
                            .WithoutSemicolon();

                        if (MainValidatorHasValidatorProviderRequirement())
                        {
                            var dtoValidators = GetValidators(Model.Properties);
                            var validatorProviderInter = GetTypeName("Application.Common.ValidatorProviderInterface");

                            method.AddStatement($"var validatorProvider = Substitute.For<{validatorProviderInter}>();");
                            foreach (var validator in dtoValidators)
                            {
                                var nestedValidatorInstantiation = new CSharpInvocationStatement($"new {validator.ValidatorName}")
                                    .WithoutSemicolon();
                                if (validator.NeedsServiceProvider)
                                {
                                    nestedValidatorInstantiation.AddArgument("validatorProvider");
                                }
                                method.AddStatement($"validatorProvider.GetValidator<{validator.DtoName}>().Returns(c => {nestedValidatorInstantiation});");
                            }

                            mainValidatorInstantiation.AddArgument("validatorProvider");
                        }

                        if (this.TryGetCommandValidatorTemplate(Model, out var validatorTemplate))
                        {
                            var validatorCtorParams = validatorTemplate.CSharpFile.Classes
                                .FirstOrDefault(x => x.HasMetadata("validator"))
                                ?.Constructors
                                .FirstOrDefault()
                                ?.Parameters
                                .Where(p => p.HasMetadata("repository"))
                                .ToArray() ?? ArraySegment<CSharpConstructorParameter>.Empty;
                            foreach (var parameter in validatorCtorParams)
                            {
                                method.AddStatement($"var {parameter.Name.ToCamelCase()} = Substitute.For<{UseType(parameter.GetMetadata<string>("repository"))}>();");

                                mainValidatorInstantiation.AddArgument(parameter.Name.ToCamelCase());
                            }
                        }

                        method.AddStatement(
                            $@"return new {this.GetValidationBehaviourName()}<{GetTypeName(Model.InternalElement)}, {(isCommandWithReturnId ? domainIdAttr.Type : "Unit")}>(new[] {{ {mainValidatorInstantiation} }});");
                    });
            });
    }

    private bool MainValidatorHasValidatorProviderRequirement()
    {
        if (!this.TryGetCommandValidatorTemplate(Model, out var template))
        {
            return false;
        }

        var @class = template.CSharpFile.Classes.First(x => x.HasMetadata("validator"));
        var constructor = @class.Constructors.First();

        return constructor.Parameters.Any(p => p.Type.Contains("IValidatorProvider"));
    }

    private IReadOnlyCollection<(string DtoName, string ValidatorName, bool NeedsServiceProvider)> GetValidators(IEnumerable<DTOFieldModel> properties)
    {
        var list = new List<(string DtoName, string ValidatorName, bool NeedsServiceProvider)>();

        foreach (var property in properties)
        {
            var dtoModel = property.TypeReference?.Element?.AsDTOModel();
            if (dtoModel is null ||
                !TryGetTypeName(TemplateRoles.Application.Contracts.Dto, dtoModel, out var dtoTemplate) ||
                !TryGetTypeName(TemplateRoles.Application.Validation.Dto, dtoModel, out var dtoValidatorTemplate))
            {
                continue;
            }

            var more = GetValidators(dtoModel.Fields);
            list.Add((dtoTemplate, dtoValidatorTemplate, more.Any()));
            list.AddRange(more);
        }

        return list;
    }

    private void AddSuccessTestDataFields(CSharpClassMethod method)
    {
        foreach (var property in Model.Properties)
        {
            var attribute = property.InternalElement?.MappedElement?.Element?.AsAttributeModel();
            if (attribute != null && attribute.HasStereotype("Text Constraints") &&
                attribute.GetStereotypeProperty<int?>("Text Constraints", "MaxLength") != null &&
                attribute.GetStereotypeProperty<int?>("Text Constraints", "MaxLength") > 0)
            {
                var maxLen = attribute.GetStereotypeProperty<int>("Text Constraints", "MaxLength");
                method.AddStatement($@"testCommand.{property.Name} = $""{GetStringWithLen(maxLen)}"";");
            }
            else if (property.GetValidations().MinLength() != null && property.GetValidations().MaxLength() != null)
            {
                method.AddStatement($@"testCommand.{property.Name} = $""{GetStringWithLen(property.GetValidations().MinLength().Value)}"";");
            }
            else if (property.GetValidations()?.MaxLength() != null)
            {
                method.AddStatement($@"testCommand.{property.Name} = $""{GetStringWithLen(property.GetValidations().MaxLength().Value)}"";");
            }
            else if (property.GetValidations()?.MinLength() != null)
            {
                method.AddStatement($@"testCommand.{property.Name} = $""{GetStringWithLen(property.GetValidations().MinLength().Value)}"";");
            }
        }
    }

    private void AddFailingTestData(CSharpClassMethod method)
    {
        bool first = true;
        foreach (var property in Model.Properties)
        {
            if (!Types.Get(property.TypeReference).IsPrimitive &&
                !IsEnum(property.TypeReference) &&
                !property.TypeReference.IsNullable)
            {
                if (!first)
                {
                    method.AddStatement(string.Empty);
                }

                AddNegativeTestCaseStatements(method, property, first, "default", "not be empty");
                first = false;
            }

            if (property.GetValidations()?.NotEmpty() == true &&
                !IsEnum(property.TypeReference))
            {
                if (!first)
                {
                    method.AddStatement(string.Empty);
                }

                AddNegativeTestCaseStatements(method, property, first, "default", "not be empty");
                first = false;
            }

            if (property.GetValidations().MinLength() != null && property.GetValidations().MaxLength() != null)
            {
                if (!first)
                {
                    method.AddStatement(string.Empty);
                }

                var minLen = property.GetValidations().MinLength().Value;
                var maxLen = property.GetValidations().MaxLength().Value;
                AddNegativeTestCaseStatements(method, property, first, $@"$""{GetStringWithLen(maxLen + 1)}""", $@"must be between {minLen} and {maxLen} characters");

                first = false;

                method.AddStatement(string.Empty);
                AddNegativeTestCaseStatements(method, property, first, $@"$""{GetStringWithLen(minLen - 1)}""", $@"must be between {minLen} and {maxLen} characters");
            }
            else if (property.GetValidations()?.MaxLength() != null)
            {
                if (!first)
                {
                    method.AddStatement(string.Empty);
                }

                var maxLen = property.GetValidations().MaxLength().Value;
                AddNegativeTestCaseStatements(method, property, first, $@"$""{GetStringWithLen(maxLen + 1)}""", $@"must be {maxLen} characters or fewer");
                first = false;
            }
            else if (property.GetValidations()?.MinLength() != null)
            {
                if (!first)
                {
                    method.AddStatement(string.Empty);
                }

                var minLen = property.GetValidations().MinLength().Value;
                AddNegativeTestCaseStatements(method, property, first, $@"$""{GetStringWithLen(minLen - 1)}""", $@"must be at least {minLen} characters");
                first = false;
            }

            if (property.GetValidations()?.MaxLength() == null && property.InternalElement.IsMapped)
            {
                var attribute = property.InternalElement?.MappedElement?.Element?.AsAttributeModel();
                if (attribute != null && attribute.HasStereotype("Text Constraints") &&
                    attribute.GetStereotypeProperty<int?>("Text Constraints", "MaxLength") > 0)
                {
                    if (!first)
                    {
                        method.AddStatement(string.Empty);
                    }

                    var maxLen = attribute.GetStereotypeProperty<int>("Text Constraints", "MaxLength");
                    AddNegativeTestCaseStatements(method, property, first, $@"$""{GetStringWithLen(maxLen + 1)}""", $@"must be {maxLen} characters or fewer");
                    first = false;
                }
            }

            HandleEnumCases(method, property, ref first);
        }
    }

    private void HandleEnumCases(CSharpClassMethod method, DTOFieldModel property, ref bool first)
    {
        if (!IsEnum(property.TypeReference))
        {
            return;
        }

        var enumModel = property.TypeReference.Element.AsEnumModel();
        if (enumModel.Literals.Count < 1)
        {
            return;
        }

        var enumLiteralsWithOrdinals = GetConceptualEnumWithOrdinalValues(enumModel);
        if (!string.IsNullOrWhiteSpace(enumModel.Literals.First().Value) && enumLiteralsWithOrdinals.First() != 0)
        {
            if (!first)
            {
                method.AddStatement(string.Empty);
            }

            AddNegativeTestCaseStatements(method, property, first, GetInvalidEnumExpression(property, "0"), "has a range of values which does not include");
            first = false;
        }

        if (!first)
        {
            method.AddStatement(string.Empty);
        }

        var lastOrdinalValue = enumLiteralsWithOrdinals.Last();
        var invalidOrdinalValueForTest = lastOrdinalValue + 1;
        AddNegativeTestCaseStatements(method, property, first, GetInvalidEnumExpression(property, invalidOrdinalValueForTest.ToString()),
            "has a range of values which does not include");
        first = false;
    }

    private string GetInvalidEnumExpression(DTOFieldModel property, string invalidOrdinalValueForTest)
    {
        if (property.TypeReference.IsCollection)
        {
            return
                $"new {UseType($"System.Collections.Generic.List<{GetTypeName(TemplateRoles.Domain.Enum, property.TypeReference.Element)}>")} {{ ({GetTypeName(TemplateRoles.Domain.Enum, property.TypeReference.Element)}){invalidOrdinalValueForTest} }}";
        }

        return $"({GetTypeName(property.TypeReference)}){invalidOrdinalValueForTest}";
    }

    private void AddNegativeTestCaseStatements(CSharpClassMethod method, DTOFieldModel property, bool first,
        string testValue, string expectedPhrase)
    {
        method.AddStatements($@"
{(first ? "var " : "")}fixture = new Fixture();");
        var customLambdaBody = new CSharpLambdaBlock("comp");
        method.AddInvocationStatement($"fixture.Customize<{GetTypeName(Model.InternalElement)}>", inv => inv
            .AddArgument(customLambdaBody));
        var otherWithStatements = new List<CSharpStatement>();

        foreach (var otherProperty in Model.Properties.Where(p => p.Id != property.Id))
        {
            if (otherProperty.GetValidations()?.MaxLength() != null)
            {
                var maxLen = otherProperty.GetValidations().MaxLength().Value;
                otherWithStatements.Add($@"With(x => x.{otherProperty.Name}, () => $""{GetStringWithLen(maxLen - 1)}"")");
            }
            else if (otherProperty.GetValidations()?.MaxLength() == null && otherProperty.InternalElement.IsMapped)
            {
                var attribute = otherProperty.InternalElement?.MappedElement?.Element?.AsAttributeModel();
                if (attribute != null && attribute.HasStereotype("Text Constraints") &&
                    attribute.GetStereotypeProperty<int?>("Text Constraints", "MaxLength") > 0)
                {
                    var maxLen = attribute.GetStereotypeProperty<int>("Text Constraints", "MaxLength");
                    otherWithStatements.Add($@"With(x => x.{otherProperty.Name}, () => $""{GetStringWithLen(maxLen - 1)}"")");
                }
            }
        }

        if (otherWithStatements.Any())
        {
            var chain = new CSharpMethodChainStatement("comp")
                .WithoutSemicolon()
                .AddChainStatement($@"With(x => x.{property.Name}, () => {testValue})");
            foreach (var statement in otherWithStatements)
            {
                chain.AddChainStatement(statement);
            }

            customLambdaBody.WithExpressionBody(chain);
        }
        else
        {
            customLambdaBody.WithExpressionBody($@"comp.With(x => x.{property.Name}, () => {testValue})");
        }

        method.AddStatements($@"
{(first ? "var " : "")}testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
yield return new object[] {{ testCommand, ""{GetPropertyNameWithArray(property)}"", ""{expectedPhrase}"" }};");
    }

    private static string GetPropertyNameWithArray(DTOFieldModel property)
    {
        return property.TypeReference.Element.IsEnumModel() && property.TypeReference.IsCollection
            ? $"{property.Name}[0]"
            : property.Name;
    }

    private static int?[] GetConceptualEnumWithOrdinalValues(EnumModel enumModel)
    {
        var enumLiteralsWithOrdinals = enumModel.Literals.Select(elem => string.IsNullOrWhiteSpace(elem.Value)
                ? null
                : int.TryParse(elem.Value, out var val)
                    ? val
                    : (int?)null)
            .ToArray();
        var lastValue = -1;
        for (var index = 0; index < enumLiteralsWithOrdinals.Length; index++)
        {
            var value = enumLiteralsWithOrdinals[index];
            if (value is null)
            {
                lastValue++;
                enumLiteralsWithOrdinals[index] = lastValue;
            }
            else
            {
                lastValue = (int)value;
            }
        }

        return enumLiteralsWithOrdinals;
    }

    private static bool IsEnum(ITypeReference typeReference)
    {
        return typeReference?.Element?.SpecializationType.EndsWith("Enum", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static string GetStringWithLen(int length)
    {
        return $@"{{string.Join(string.Empty, fixture.CreateMany<char>({length}))}}";
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