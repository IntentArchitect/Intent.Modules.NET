using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Drift;
using Intent.Modules.Services.Guardrails.Contracts.Templates;

namespace Intent.Modules.Services.Guardrails.Contracts.Drift
{
    /// <summary>
    /// The Services designer's HTTP and security facts, read from the model's own stereotypes
    /// (Intent.Metadata.WebApi and Intent.Metadata.Security, by name, so neither is a dependency), and its intent facts,
    /// read from the interaction associations by type id (so none of the modelers that define them is a dependency).
    /// Only this application's packages count.
    /// </summary>
    internal static class DriftModel
    {
        /// <summary>Each interaction association type and the kind of fact it states.</summary>
        private static readonly IReadOnlyDictionary<string, FactKind> Intents = new Dictionary<string, FactKind>(StringComparer.OrdinalIgnoreCase)
        {
            [OptionalModelerMetadata.CreateEntityActionTypeId] = ServicesDrift.IntentCreates,
            [OptionalModelerMetadata.UpdateEntityActionTypeId] = ServicesDrift.IntentUpdates,
            [OptionalModelerMetadata.DeleteEntityActionTypeId] = ServicesDrift.IntentDeletes,
            [OptionalModelerMetadata.QueryEntityActionTypeId] = ServicesDrift.IntentQueries,
            [OptionalModelerMetadata.PerformInvocationTypeId] = ServicesDrift.IntentInvokes,
            [OptionalModelerMetadata.PublishIntegrationEventTypeId] = ServicesDrift.IntentPublishes,
            [OptionalModelerMetadata.SendIntegrationCommandTypeId] = ServicesDrift.IntentSends
        };

        /// <summary>The subscriptions an event handler's interactions are drawn from.</summary>
        private static readonly HashSet<string> Subscriptions = new(StringComparer.OrdinalIgnoreCase)
        {
            OptionalModelerMetadata.SubscribeIntegrationEventTypeId,
            OptionalModelerMetadata.SubscribeIntegrationCommandTypeId,
            OptionalModelerMetadata.DomainEventHandlerAssociationTypeId
        };

        public static IReadOnlyList<ModelFact> Read(IDesigner designer, string applicationId)
        {
            var packages = designer.Packages.Where(x => x.ApplicationId == applicationId).ToList();
            var elements = packages.SelectMany(x => Descendants(x.ChildElements)).ToList();
            var usesSecurity = packages.Cast<IHasStereotypes>().Concat(elements).Any(x => IsSecured(x) || IsUnsecured(x));
            var facts = new List<ModelFact>();

            foreach (var element in elements)
            {
                switch (element.SpecializationType)
                {
                    case "Service":
                        if (Stereotype(element, "Http Service Settings") is { } serviceSettings)
                        {
                            facts.Add(Fact(ServicesDrift.ServiceRoute, element.Name, Property(serviceSettings, "Route"), new Dictionary<string, string> { ["Service"] = element.Name }, null, element.Id));
                        }

                        foreach (var operation in element.ChildElements.Where(x => x.SpecializationType == "Operation"))
                        {
                            AddEndpoint(operation, element, "Parameter", usesSecurity, facts);
                            AddIntents(operation, element, operation.Name, $"{element.Name}.{operation.Name}",
                                operation.ChildElements.Where(x => x.SpecializationType == "Parameter").Select(x => x.Name).ToList(), facts, element.Id, operation.Id);
                        }

                        break;
                    case "Command":
                    case "Query":
                        AddEndpoint(element, null, "DTO-Field", usesSecurity, facts);
                        AddIntents(element, null, element.Name, element.Name, null, facts, element.Id);
                        break;
                }

                // An event handler's interactions are drawn from each subscription, the association's end under the handler.
                foreach (var subscription in Ends(element, Subscriptions))
                {
                    var message = subscription.TypeReference?.Element?.Name ?? string.Empty;
                    AddIntents(subscription, element, message, $"{element.Name}({message})", null, facts, element.Id);
                }
            }

            return facts;
        }

        /// <summary>
        /// One intent fact per interaction modelled on the handler (a Command, Query, Service operation or subscription):
        /// the handler's placeholders, as for an endpoint, then the target's. The handler's element ids, so the report opens it.
        /// </summary>
        private static void AddIntents(IElement handler, IElement owner, string operation, string display, IReadOnlyList<string> parameterNames, List<ModelFact> facts, params string[] elementIds)
        {
            var folderOf = owner ?? handler;
            foreach (var end in Ends(handler, Intents.Keys).OrderBy(x => x.Association?.Id ?? x.Id, StringComparer.Ordinal))
            {
                var kind = Intents[end.Association.SpecializationTypeId];
                var target = end.TypeReference?.Element as IElement;
                if (target == null)
                {
                    continue;
                }

                var placeholders = new Dictionary<string, string>
                {
                    ["Service"] = owner?.Name ?? string.Empty,
                    ["Operation"] = operation,
                    ["Folder"] = folderOf.ParentElement is { SpecializationType: "Folder" } folder ? folder.Name : string.Empty
                };
                string value;
                if (kind == ServicesDrift.IntentInvokes)
                {
                    // An operation belongs to its Service, Domain Service, Repository or Service Proxy; a Command, Query or
                    // stored procedure to nothing a call names.
                    value = target.Name;
                    placeholders["Invoked"] = target.Name;
                    placeholders["InvokedOwner"] = target.SpecializationType == "Operation" && target.ParentElement is { } parent && parent.SpecializationType != "Folder" ? parent.Name : string.Empty;
                }
                else if (kind == ServicesDrift.IntentPublishes || kind == ServicesDrift.IntentSends)
                {
                    value = target.Name;
                    placeholders["Message"] = target.Name;
                }
                else
                {
                    // An entity action targets the class, or one of its constructors or operations.
                    var entity = target.SpecializationType == "Class" ? target : target.ParentElement;
                    value = entity?.Name ?? string.Empty;
                    placeholders["Entity"] = value;
                    if (kind.Placeholders.Contains("Member"))
                    {
                        placeholders["Member"] = target.SpecializationType == "Class" ? string.Empty : target.SpecializationType == "Class Constructor" ? value : target.Name;
                    }
                }

                facts.Add(Fact(kind, display, value, placeholders, parameterNames, elementIds));
            }
        }

        /// <summary>The far ends of the element's associations of these types that it is the source of.</summary>
        private static IEnumerable<IAssociationEnd> Ends(IElement element, IEnumerable<string> associationTypeIds)
        {
            var types = associationTypeIds as ICollection<string> ?? associationTypeIds.ToList();
            return (element.AssociatedElements ?? Enumerable.Empty<IAssociationEnd>())
                .Where(x => x.Association?.SpecializationTypeId is { } type && types.Contains(type) && x.IsTargetEnd());
        }

        private static void AddEndpoint(IElement element, IElement service, string parameterType, bool usesSecurity, List<ModelFact> facts)
        {
            var http = Stereotype(element, "Http Settings");
            if (http == null)
            {
                return;
            }

            var display = service == null ? element.Name : $"{service.Name}.{element.Name}";
            var placeholders = new Dictionary<string, string>
            {
                ["Service"] = service?.Name ?? string.Empty,
                ["Operation"] = element.Name,
                ["Folder"] = (service ?? element).ParentElement is { SpecializationType: "Folder" } folder ? folder.Name : string.Empty
            };
            var parameters = element.ChildElements.Where(x => x.SpecializationType == parameterType).ToList();
            var parameterNames = service == null ? null : parameters.Select(x => x.Name).ToList();

            facts.Add(Fact(ServicesDrift.OperationVerb, display, Property(http, "Verb"), placeholders, parameterNames, service?.Id, element.Id));
            facts.Add(Fact(ServicesDrift.OperationRoute, display, Property(http, "Route"), placeholders, parameterNames, service?.Id, element.Id));
            if (usesSecurity)
            {
                var (security, secured) = EffectiveSecurity(element);
                facts.Add(Fact(ServicesDrift.Security, display, security, placeholders, parameterNames, service?.Id, element.Id));
                if (secured != null)
                {
                    var roles = Names(secured, "Roles", "Security Roles");
                    if (roles.Length > 0)
                    {
                        facts.Add(Fact(ServicesDrift.SecurityRoles, display, roles, placeholders, parameterNames, service?.Id, element.Id));
                    }

                    var policies = Names(secured, "Policy", "Security Policies");
                    if (policies.Length > 0)
                    {
                        facts.Add(Fact(ServicesDrift.SecurityPolicy, display, policies, placeholders, parameterNames, service?.Id, element.Id));
                    }
                }
            }

            foreach (var parameter in parameters)
            {
                if (Stereotype(parameter, "Parameter Settings") is { } settings)
                {
                    var parameterPlaceholders = new Dictionary<string, string>(placeholders) { ["Parameter"] = parameter.Name };
                    facts.Add(Fact(ServicesDrift.ParameterSource, $"{display}({parameter.Name})", Property(settings, "Source") is { Length: > 0 } source ? source : "Default", parameterPlaceholders, parameterNames, service?.Id, element.Id, parameter.Id));
                }
            }
        }

        /// <summary>
        /// <c>Unsecured</c> or <c>Secured</c> on the element, else on its Service, its folders or its package, else
        /// <c>None</c>; with the <c>Secured</c> stereotype that applies, for its roles and policies.
        /// </summary>
        private static (string Security, IStereotype Secured) EffectiveSecurity(IElement element)
        {
            for (var current = element; current != null; current = current.ParentElement)
            {
                var result = SecurityOf(current);
                if (result.Security != null)
                {
                    return result;
                }
            }

            var package = SecurityOf(element.Package);
            return package.Security != null ? package : ("None", null);
        }

        private static (string Security, IStereotype Secured) SecurityOf(IHasStereotypes element)
        {
            if (element == null)
            {
                return (null, null);
            }

            if (IsUnsecured(element))
            {
                return ("Unsecured", null);
            }

            var secured = element.Stereotypes.FirstOrDefault(x => x.Name == "Secured");
            return secured != null ? ("Secured", secured) : (null, null);
        }

        private static bool IsSecured(IHasStereotypes element) => element.Stereotypes.Any(x => x.Name == "Secured");

        private static bool IsUnsecured(IHasStereotypes element) => element.Stereotypes.Any(x => x.Name == "Unsecured");

        /// <summary>A text property's comma-separated names, then an element list property's names, without spaces.</summary>
        private static string Names(IStereotype stereotype, string textProperty, string elementsProperty)
        {
            var names = Property(stereotype, textProperty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            if (stereotype.TryGetProperty(elementsProperty, out var property))
            {
                switch (property)
                {
                    case IStereotypeProperty<IElement[]> elements when elements.Value != null:
                        names.AddRange(elements.Value.Where(x => x != null).Select(x => x.Name));
                        break;
                    case IStereotypeProperty<IElement> single when single.Value != null:
                        names.Add(single.Value.Name);
                        break;
                }
            }

            return string.Join(",", names.Where(x => x.Length > 0).Distinct(StringComparer.Ordinal));
        }

        private static ModelFact Fact(FactKind kind, string element, string value, IReadOnlyDictionary<string, string> placeholders, IReadOnlyList<string> parameterNames, params string[] elementIds)
        {
            return new ModelFact { Kind = kind, Element = element, ElementIds = elementIds.Where(x => x != null).ToList(), Value = value?.Trim() ?? string.Empty, Placeholders = placeholders, ParameterNames = parameterNames };
        }

        /// <summary>The application's elements in this designer, by id, for the errors that open them.</summary>
        public static IReadOnlyDictionary<string, IElement> ElementsById(IDesigner designer, string applicationId)
        {
            var result = new Dictionary<string, IElement>(StringComparer.Ordinal);
            foreach (var element in designer.Packages.Where(x => x.ApplicationId == applicationId).SelectMany(x => Descendants(x.ChildElements)))
            {
                result.TryAdd(element.Id, element);
            }

            return result;
        }

        private static IEnumerable<IElement> Descendants(IEnumerable<IElement> elements)
        {
            foreach (var element in elements ?? Enumerable.Empty<IElement>())
            {
                yield return element;
                foreach (var descendant in Descendants(element.ChildElements))
                {
                    yield return descendant;
                }
            }
        }

        private static IStereotype Stereotype(IHasStereotypes element, string name)
        {
            return element?.Stereotypes.FirstOrDefault(x => x.Name == name);
        }

        private static string Property(IStereotype stereotype, string name)
        {
            return stereotype != null && stereotype.TryGetProperty(name, out var property) ? property.Value?.Trim() ?? string.Empty : string.Empty;
        }
    }
}
