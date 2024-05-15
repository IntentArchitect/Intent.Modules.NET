using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Dapr.Application.Common.Mappings;
using CleanArchitecture.Dapr.Domain.Common.Exceptions;
using CleanArchitecture.Dapr.Domain.Entities;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
            Id = null!;
            Client = null!;
            InvoiceLines = null!;
        }

        public string Id { get; set; }
        public int Number { get; set; }
        public InvoiceClientDto Client { get; set; }
        public List<InvoiceInvoiceLineDto> InvoiceLines { get; set; }

        public static InvoiceDto Create(
            string id,
            int number,
            InvoiceClientDto client,
            List<InvoiceInvoiceLineDto> invoiceLines)
        {
            return new InvoiceDto
            {
                Id = id,
                Number = number,
                Client = client,
                InvoiceLines = invoiceLines
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>()
                .ForMember(d => d.InvoiceLines, opt => opt.MapFrom(src => src.InvoiceLines))
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
                var client = _clientRepository.FindByIdAsync(source.ClientId).Result;

                if (client == null)
                {
                    throw new NotFoundException($"Unable to load required relationship for Id({source.ClientId}). (Invoice)->(Client)");
                }
                destination.Client = client.MapToInvoiceClientDto(_mapper);
            }
        }
    }
}