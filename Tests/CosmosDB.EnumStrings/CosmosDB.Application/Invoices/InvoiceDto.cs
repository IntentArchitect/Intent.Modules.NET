using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.Application.Common.Mappings;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.Application.Invoices
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
            Id = null!;
            ClientId = null!;
            Number = null!;
            LineItems = null!;
            Client = null!;
        }

        public string Id { get; set; }
        public string ClientId { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public List<InvoiceLineItemDto> LineItems { get; set; }
        public InvoiceClientDto Client { get; set; }

        public static InvoiceDto Create(
            string id,
            string clientId,
            DateTime date,
            string number,
            List<InvoiceLineItemDto> lineItems,
            InvoiceClientDto client)
        {
            return new InvoiceDto
            {
                Id = id,
                ClientId = clientId,
                Date = date,
                Number = number,
                LineItems = lineItems,
                Client = client
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>()
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ClientIdentifier))
                .ForMember(d => d.LineItems, opt => opt.MapFrom(src => src.LineItems))
                .AfterMap<MappingAction>();
        }

        internal class MappingAction : IMappingAction<Invoice, InvoiceDto>
        {
            private readonly IClientRepository _clientRepository;
            private readonly IMapper _mapper;

            public MappingAction(IClientRepository clientRepository, IMapper mapper)
            {
                _clientRepository = clientRepository;
                _mapper = mapper;
            }

            public void Process(Invoice source, InvoiceDto destination, ResolutionContext context)
            {
                var client = _clientRepository.FindByIdAsync(source.ClientIdentifier).Result;

                if (client == null)
                {
                    throw new NotFoundException($"Unable to load required relationship for Id({source.ClientIdentifier}). (Invoice)->(Client)");
                }
                destination.Client = client.MapToInvoiceClientDto(_mapper);
            }
        }
    }
}