using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Application.FluentValidation.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.FluentValidation.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
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

        AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
        AddTypeSource(CommandModelsTemplate.TemplateId);
        AddTypeSource(TemplateFulfillingRoles.Application.Contracts.Dto);

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
                var domainIdAttr = domainElement.GetEntityIdAttribute(ExecutionContext);
                var isCommand = Model.Name.Contains("create", StringComparison.OrdinalIgnoreCase);

                var priClass = file.Classes.First();
                priClass.AddMethod("IEnumerable<object[]>", "GetSuccessfulResultTestData", method =>
                {
                    method.Static();
                    method.AddStatements($@"
        var fixture = new Fixture();
        var testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        testCommand.AggregateAttr = testCommand.AggregateAttr.Substring(0, 20);
        yield return new object[] {{ testCommand }};");
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
                    if (isCommand)
                    {
                        method.AddStatement($@"var expectedId = new Fixture().Create<{domainIdAttr.Type}>();");
                    }
                    method.AddStatements($@"
        // Act
        var result = await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(expectedId));

        // Assert
        result.Should().Be({(isCommand ? "expectedId" : "Unit.Value")});");
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
                    if (isCommand)
                    {
                        method.AddStatement($@"var expectedId = new Fixture().Create<{domainIdAttr.Type}>();");
                    }
                    method.AddStatements($@"
        // Act
        var act = async () => await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult({(isCommand ? "expectedId" : "Unit.Value")}));

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

                priClass.AddMethod($"{this.GetValidationBehaviourName()}<{GetTypeName(Model.InternalElement)}, {(isCommand ? domainIdAttr.Type : "Unit")}>",
                    "GetValidationBehaviour", method =>
                    {
                        method.Private();
                        method.AddStatement($@"return new {this.GetValidationBehaviourName()}<{GetTypeName(Model.InternalElement)}, {(isCommand ? domainIdAttr.Type : "Unit")}>(new[] {{ new {this.GetCommandValidatorName(Model)}() }});");
                    });
            });
    }

    private void AddFailingTestData(CSharpClassMethod method)
    {
        bool first = true;
        foreach (var property in Model.Properties)
        {
            if (Types.Get(property.TypeReference).IsPrimitive && !property.TypeReference.IsNullable)
            {
                method.AddStatements($@"
        {(first ? "var " : "")}fixture = new Fixture();
        fixture.Customize<{GetTypeName(Model.InternalElement)}>(comp => comp.With(x => x.{property.Name}, () => default));
        {(first ? "var " : "")}testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        yield return new object[] {{ testCommand, ""{property.Name}"", ""not be empty"" }};");
                first = false;
            }

            if (property.GetValidations()?.NotEmpty() == true)
            {
                method.AddStatements($@"
        {(first ? "var " : "")}fixture = new Fixture();
        fixture.Customize<{GetTypeName(Model.InternalElement)}>(comp => comp.With(x => x.{property.Name}, () => default));
        {(first ? "var " : "")}testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        yield return new object[] {{ testCommand, ""{property.Name}"", ""not be empty"" }};");
                first = false;
            }

            if (property.GetValidations()?.MaxLength() != null)
            {
                method.AddStatements($@"
        {(first ? "var " : "")}fixture = new Fixture();
        fixture.Customize<{GetTypeName(Model.InternalElement)}>(comp => comp.With(x => x.{property.Name}, () => ""{(Enumerable.Range(0, property.GetValidations().MaxLength().Value + 1).Aggregate(new StringBuilder(), (s, i) => s.Append(i % 10)))}""));
        {(first ? "var " : "")}testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        yield return new object[] {{ testCommand, ""{property.Name}"", ""must be {property.GetValidations().MaxLength().Value} characters or fewer"" }};");
                first = false;
            }
        }
    }

    [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }

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