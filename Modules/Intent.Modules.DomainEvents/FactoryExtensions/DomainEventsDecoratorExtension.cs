using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.DomainEvents.Templates.DomainEvent;
using Intent.Modules.DomainEvents.Templates.DomainEventBase;
using Intent.Modules.DomainEvents.Templates.DomainEventServiceInterface;
using Intent.Modules.DomainEvents.Templates.HasDomainEventInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.DomainEvents.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DomainEventsDecoratorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.DomainEvents.DomainEventsDecoratorExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => -10;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var entityStateTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Primary));
            foreach (var template in entityStateTemplates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.FirstOrDefault();
                    if (@class?.TryGetMetadata<ClassModel>("model", out var model) == true &&
                        model.IsAggregateRoot() && model.ParentClass == null)
                    {
                        @class.ImplementsInterface(template.GetTypeName(HasDomainEventInterfaceTemplate.TemplateId));
                        @class.AddProperty($"{template.UseType("System.Collections.Generic.List")}<{template.GetTypeName(DomainEventBaseTemplate.TemplateId)}>", "DomainEvents", property =>
                        {
                            property.WithInitialValue($"new {property.Type}()");
                            property.AddMetadata("non-persistent", true);
                        });

                        foreach (var ctorModel in model.Constructors.Where(x => x.PublishedDomainEvents().Any()))
                        {
                            var ctor = @class.Constructors.FirstOrDefault(x => x.TryGetMetadata("model", out var m) && Equals(m, ctorModel));
                            if (ctor != null)
                            {
                                foreach (var publishedDomainEvent in ctorModel.PublishedDomainEvents())
                                {
                                    ctor.Statements.FirstOrDefault(x => x.ToString().Contains("NotImplementedException"))?.Remove();
                                    var mapping = publishedDomainEvent.InternalAssociation.Mapping;
                                    if (mapping != null)
                                    {
                                        ctor.AddStatement($"DomainEvents.Add({ConstructDomainEvent(template, mapping, publishedDomainEvent.Element.AsDomainEventModel())});");
                                    }
                                    else
                                    {
                                        ctor.AddStatement($"DomainEvents.Add({ConstructDomainEvent(template, ctor.Parameters.Select(x => x.Name).ToList(), publishedDomainEvent.Element.AsDomainEventModel())});");
                                    }
                                }
                            }
                        }

                        foreach (var operationModel in model.Operations.Where(x => x.PublishedDomainEvents().Any()))
                        {
                            var method = @class.Methods.FirstOrDefault(x => x.TryGetMetadata("model", out var m) && Equals(m, operationModel));
                            if (method != null)
                            {
                                foreach (var publishedDomainEvent in operationModel.PublishedDomainEvents().Select(x => x.Element.AsDomainEventModel()))
                                {
                                    method.Statements.FirstOrDefault(x => x.ToString().Contains("NotImplementedException"))?.Remove();
                                    method.AddStatement($"DomainEvents.Add({ConstructDomainEvent(template, method.Parameters.Select(x => x.Name).ToList(), publishedDomainEvent)});");
                                }
                            }
                        }
                    }
                });
            }

            var entityInterfaceTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Interface));
            foreach (var template in entityInterfaceTemplates.Where(x => x.CSharpFile.Interfaces.Any()))
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var @interface = file.Interfaces.First();
                    if (@interface.TryGetMetadata<ClassModel>("model", out var model) &&
                        model.IsAggregateRoot() && model.ParentClass == null)
                    {
                        @interface.ExtendsInterface(template.GetTypeName(HasDomainEventInterfaceTemplate.TemplateId));
                    }
                });
            }
        }

        private string ConstructDomainEvent(ICSharpFileBuilderTemplate template,
            IElementToElementMapping mapping,
            DomainEventModel model)
        {
            var classModel = template is ITemplateWithModel templateWithModel ? templateWithModel.Model as ClassModel : null;
            var invocation = new CSharpInvocationStatement(template.GetTypeName(DomainEventTemplate.TemplateId, model)).WithoutSemicolon();
            if (classModel == null)
            {
                throw new Exception("Constructing a domain event cannot be done from a template that doesn't have a ClassModel");
            }

            foreach (var property in model.Properties)
            {
                var connection = mapping.Connections.SingleOrDefault(x => x.ToPath.Last().Element.Id == property.Id);
                if (connection == null)
                {
                    invocation.AddArgument("null");
                    continue;
                }



                invocation.AddArgument(GetPath(connection.FromPath, classModel, mapping.FromElement.AsClassConstructorModel()));
            }

            return $"new {invocation}";
        }

        private static string GetPath(IList<IElementMappingPathTarget> path, params IMetadataModel[] rootModels)
        {
            if (path.Count == 1 && rootModels.Any(x => x.Id == path[0].Id))
            {
                return "this";
            }

            var mappedPath = new List<string>();
            foreach (var pathItem in path)
            {
                if (rootModels.Any(x => x.Id == pathItem.Id))
                {
                    continue;
                }
                
                switch (pathItem.Specialization)
                {
                    case OperationModel.SpecializationType:
                        mappedPath.Add($"{pathItem.Element.Name.ToPascalCase()}()");
                        continue;
                    case ParameterModel.SpecializationType:
                        mappedPath.Add($"{pathItem.Element.Name.ToParameterName()}");
                        continue;
                    default:
                        mappedPath.Add($"{pathItem.Element.Name.ToPascalCase()}");
                        continue;
                }
            }

            return string.Join(".", mappedPath);
        }

        private string ConstructDomainEvent(ICSharpFileBuilderTemplate template, 
            IList<string> availableParameters, 
            DomainEventModel model)
        {
            var classModel = template is ITemplateWithModel templateWithModel ? templateWithModel.Model as ClassModel : null;
            var invocation = new CSharpInvocationStatement(template.GetTypeName(DomainEventTemplate.TemplateId, model)).WithoutSemicolon();
            if (classModel == null)
            {
                throw new Exception("Constructing a domain event cannot be done from a template that doesn't have a ClassModel");
            }

            foreach (var property in model.Properties)
            {
                if (property.TypeReference.Element.Id == classModel.Id)
                {
                    invocation.AddArgument("this");
                }
                else if (availableParameters.Any(x => x.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    invocation.AddArgument(property.Name.ToCamelCase());
                }
                else if (classModel.Attributes.Any(x => x.Name.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    invocation.AddArgument(property.Name.ToPascalCase());
                }
                else if (classModel.AssociatedClasses.Any(x => x.Name.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    invocation.AddArgument(property.Name.ToPascalCase());
                }
                else if (classModel.Attributes.Any(x => ($@"{classModel.Name}{x.Name}").Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var attribute = classModel.Attributes.First(x => ($@"{classModel.Name}{x.Name}").Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
                    invocation.AddArgument(attribute.Name.ToPascalCase());
                }
                else
                {
                    invocation.AddArgument("null");
                }
            }

            return $"new {invocation}";
        }
    }
}