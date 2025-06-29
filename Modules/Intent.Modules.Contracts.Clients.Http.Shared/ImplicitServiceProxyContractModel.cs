using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;

namespace Intent.Modules.Contracts.Clients.Http.Shared;

public class ImplicitServiceProxyContractModel : IServiceContractModel
{
    private const string CommandTypeId = "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9";
    private const string QueryTypeId = "e71b0662-e29d-4db2-868b-8a12464b25d0";
    private const string OperationTypeId = Intent.Modelers.Services.Api.OperationModel.SpecializationTypeId;

    public ImplicitServiceProxyContractModel(IReadOnlyList<IElement> elements)
    {
        var first = elements[0];
        Id = first.ParentId;
        InternalElement = first.ParentElement;
        Name = first.ParentElement != null
            ? first.ParentElement.Name.EnsureSuffixedWith("Service")
            : $"{first.Package.Name.RemoveSuffix(".Services").ToPascalCase().RemoveSuffix("Service")}DefaultService";
        Operations = elements
            .Select(x => new OperationModel(x))
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToArray();
    }

    public string Id { get; }
    public string Name { get; }
    public IElement InternalElement { get; }
    public IReadOnlyList<IServiceContractOperationModel?> Operations { get; }

    private class OperationModel : IServiceContractOperationModel
    {
        public OperationModel(IElement element)
        {
            Id = element.Id;
            TypeReference = element.TypeReference;
            InternalElement = element;

            switch (element.SpecializationTypeId)
            {
                case OperationTypeId:
                    Name = element.Name;
                    Parameters = Intent.Modelers.Services.Api.OperationModelExtensions.AsOperationModel(element)
                        .Parameters
                        .Select(x => new ParameterModel(
                            id: x.Id,
                            name: x.Name,
                            typeReference: x.TypeReference))
                        .ToArray();
                    break;
                case CommandTypeId:
                    Name = element.Name.RemoveSuffix("Command");
                    Parameters = element.ChildElements.Any()
                        ? [new ParameterModel(
                            id: element.Id,
                            name: "command",
                            typeReference: element.AsTypeReference())]
                        : [];
                    break;
                case QueryTypeId:
                    Name = element.Name.RemoveSuffix("Query");
                    Parameters = element.ChildElements.Any()
                        ? [new ParameterModel(
                            id: element.Id,
                            name: "query",
                            typeReference: element.AsTypeReference())]
                        : [];
                    break;
                default:
                    throw new InvalidOperationException($"Unknown type: {element.SpecializationType} ({element.SpecializationTypeId})");
            }
        }

        public string Id { get; }
        public string Name { get; }
        public ITypeReference TypeReference { get; }
        public IElement InternalElement { get; }
        public IReadOnlyList<IServiceContractOperationParameterModel> Parameters { get; }
    }

    private class ParameterModel : IServiceContractOperationParameterModel
    {
        public ParameterModel(string id, string name, ITypeReference typeReference)
        {
            Id = id;
            Name = name;
            TypeReference = typeReference;
        }

        public string Id { get; }
        public string Name { get; }
        public ITypeReference TypeReference { get; }
    }
}