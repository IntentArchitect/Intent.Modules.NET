using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Interfaces;
using CosmosDB.Domain.Repositories;
using CosmosDB.Infrastructure.Persistence;
using CosmosDB.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepositoryBase", Version = "1.0")]

namespace CosmosDB.Infrastructure.Repositories
{
    internal abstract class CosmosDBRepositoryBase<TDomain, TPersistence, TDocument> : ICosmosDBRepository<TDomain, TPersistence>
        where TPersistence : TDomain
        where TDocument : TDomain, ICosmosDBDocument<TDocument, TDomain>, new()
    {
        private readonly CosmosDBUnitOfWork _unitOfWork;
        private readonly Microsoft.Azure.CosmosRepository.IRepository<TDocument> _cosmosRepository;
        private readonly string _idFieldName;

        protected CosmosDBRepositoryBase(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<TDocument> cosmosRepository,
            string idFieldName)
        {
            _unitOfWork = unitOfWork;
            _cosmosRepository = cosmosRepository;
            _idFieldName = idFieldName;
        }

        public ICosmosDBUnitOfWork UnitOfWork => _unitOfWork;

        public void Add(TDomain entity)
        {
            _unitOfWork.Track(entity);
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity);
                await _cosmosRepository.CreateAsync(document, cancellationToken: cancellationToken);
            });
        }

        public void Update(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity);
                await _cosmosRepository.UpdateAsync(document, cancellationToken: cancellationToken);
            });
        }

        public void Remove(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity);
                await _cosmosRepository.DeleteAsync(document, cancellationToken: cancellationToken);
            });
        }

        public async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var documents = await _cosmosRepository.GetAsync(_ => true, cancellationToken);
            var results = documents.Cast<TDomain>().ToList();

            foreach (var result in results)
            {
                _unitOfWork.Track(result);
            }

            return results;
        }

        public async Task<TDomain?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var document = await _cosmosRepository.GetAsync(id, cancellationToken: cancellationToken);

            return document;
        }

        public async Task<List<TDomain>> FindByIdsAsync(
            IEnumerable<string> ids,
            CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($"SELECT * from c WHERE ARRAY_CONTAINS(@ids, c.{_idFieldName})")
                .WithParameter("@ids", ids);
            var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);
            var results = documents.Cast<TDomain>().ToList();

            foreach (var result in results)
            {
                _unitOfWork.Track(result);
            }

            return results;
        }
    }
}