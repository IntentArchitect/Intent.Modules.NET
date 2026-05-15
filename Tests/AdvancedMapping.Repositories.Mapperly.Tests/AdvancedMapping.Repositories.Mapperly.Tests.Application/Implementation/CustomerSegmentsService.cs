using AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.Interfaces;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.CustomerSegments;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Common.Exceptions;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class CustomerSegmentsService : ICustomerSegmentsService
    {
        private readonly ICustomerSegmentsRepository _customerSegmentsRepository;
        private readonly CustomerSegmentsDtoMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public CustomerSegmentsService(ICustomerSegmentsRepository customerSegmentsRepository,
            CustomerSegmentsDtoMapper mapper)
        {
            _customerSegmentsRepository = customerSegmentsRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateCustomerSegments(
            CreateCustomerSegmentsDto dto,
            CancellationToken cancellationToken = default)
        {
            var customerSegments = new Domain.Entities.CustomerSegments
            {
                SegmentId = dto.SegmentId,
                CustomerId = dto.CustomerId,
                Confidence = dto.Confidence
            };

            _customerSegmentsRepository.Add(customerSegments);
            await _customerSegmentsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return customerSegments.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateCustomerSegments(
            Guid id,
            UpdateCustomerSegmentsDto dto,
            CancellationToken cancellationToken = default)
        {
            var customerSegments = await _customerSegmentsRepository.FindByIdAsync(id, cancellationToken);
            if (customerSegments is null)
            {
                throw new NotFoundException($"Could not find CustomerSegments '{id}'");
            }

            customerSegments.SegmentId = dto.SegmentId;
            customerSegments.CustomerId = dto.CustomerId;
            customerSegments.Confidence = dto.Confidence;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerSegmentsDto> FindCustomerSegmentsById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var customerSegments = await _customerSegmentsRepository.FindByIdAsync(id, cancellationToken);
            if (customerSegments is null)
            {
                throw new NotFoundException($"Could not find CustomerSegments '{id}'");
            }
            return _mapper.CustomerSegmentsToCustomerSegmentsDto(customerSegments);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerSegmentsDto>> FindCustomerSegments(CancellationToken cancellationToken = default)
        {
            var customerSegments = await _customerSegmentsRepository.FindAllAsync(cancellationToken);
            return _mapper.CustomerSegmentsToCustomerSegmentsDtoList(customerSegments);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteCustomerSegments(Guid id, CancellationToken cancellationToken = default)
        {
            var customerSegments = await _customerSegmentsRepository.FindByIdAsync(id, cancellationToken);
            if (customerSegments is null)
            {
                throw new NotFoundException($"Could not find CustomerSegments '{id}'");
            }


            _customerSegmentsRepository.Remove(customerSegments);
        }
    }
}