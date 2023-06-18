using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Common.Interfaces;
using CosmosDB.Domain.Repositories;
using CosmosDB.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBRepositoryBase", Version = "1.0")]

namespace CosmosDB.Infrastructure.Repositories
{
    internal abstract class CosmosDBRepositoryBase<TDomain, TPersistence, TDocument> : ICosmosDBRepository<TDomain, TPersistence>
        where TPersistence : TDomain
        where TDocument : IItem
    {
        private readonly CosmosDBUnitOfWork _unitOfWork;
        private readonly Microsoft.Azure.CosmosRepository.IRepository<TDocument> _cosmosRepository;
        private readonly IMapper _mapper;
        private readonly string _idFieldName;

        protected CosmosDBRepositoryBase(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<TDocument> cosmosRepository,
            IMapper mapper,
            string idFieldName)
        {
            _unitOfWork = unitOfWork;
            _cosmosRepository = cosmosRepository;
            _mapper = mapper;
            _idFieldName = idFieldName;
        }

        public ICosmosDBUnitOfWork UnitOfWork => _unitOfWork;

        public void Add(TDomain entity)
        {
            EnsureHasId(entity);

            _unitOfWork.Track(entity);
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = _mapper.Map<TDocument>(entity);
                await _cosmosRepository.CreateAsync(document, cancellationToken: cancellationToken);
            });
        }

        public void Update(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = _mapper.Map<TDocument>(entity);
                await _cosmosRepository.UpdateAsync(document, cancellationToken: cancellationToken);
            });
        }

        public void Remove(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = _mapper.Map<TDocument>(entity);
                await _cosmosRepository.DeleteAsync(document, cancellationToken: cancellationToken);
            });
        }

        public async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var documents = await _cosmosRepository.GetAsync(_ => true, cancellationToken);
            var results = _mapper.Map<List<TDomain>>(documents);

            foreach (var result in results)
            {
                _unitOfWork.Track(result);
            }

            return results;
        }

        public async Task<TDomain?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var document = await _cosmosRepository.GetAsync(id, cancellationToken: cancellationToken);

            return _mapper.Map<TDomain>(document);
        }

        public async Task<List<TDomain>> FindByIdsAsync(
            IEnumerable<string> ids,
            CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($"SELECT * from c WHERE ARRAY_CONTAINS(@ids, c.{_idFieldName})")
                .WithParameter("@ids", ids);
            var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);
            var results = _mapper.Map<List<TDomain>>(documents);

            foreach (var result in results)
            {
                _unitOfWork.Track(result);
            }

            return results;
        }

        protected abstract void EnsureHasId(TDomain entity);
    }
}