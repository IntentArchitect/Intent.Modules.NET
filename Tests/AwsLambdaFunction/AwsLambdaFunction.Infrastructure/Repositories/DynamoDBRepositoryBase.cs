using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AwsLambdaFunction.Domain.Common.Interfaces;
using AwsLambdaFunction.Domain.Repositories;
using AwsLambdaFunction.Infrastructure.Persistence;
using AwsLambdaFunction.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBRepositoryBase", Version = "1.0")]

namespace AwsLambdaFunction.Infrastructure.Repositories
{
    internal abstract class DynamoDBRepositoryBase<TDomain, TDocument, TPartitionKey, TSortKey> : IDynamoDBRepository<TDomain, TPartitionKey>, IDynamoDBRepository<TDomain, TPartitionKey, TSortKey>
        where TDomain : class
        where TDocument : IDynamoDBDocument<TDomain, TDocument>, new()
    {
        private readonly Dictionary<object, int?> _versions = new Dictionary<object, int?>();
        private readonly IDynamoDBContext _context;
        private readonly DynamoDBUnitOfWork _unitOfWork;

        protected DynamoDBRepositoryBase(IDynamoDBContext context, DynamoDBUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public IDynamoDBUnitOfWork UnitOfWork => _unitOfWork;

        public void Add(TDomain entity)
        {
            _unitOfWork.Track(entity);
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity, _ => null);
                await _context.SaveAsync(document, cancellationToken);
            });
        }

        public void Update(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity, _versions.GetValueOrDefault);
                await _context.SaveAsync(document, cancellationToken);
            });
        }

        public void Remove(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity, _versions.GetValueOrDefault);
                await _context.DeleteAsync<TDocument>(document.GetKey(), cancellationToken);
            });
        }

        public async Task<TDomain?> FindByKeyAsync(
            TPartitionKey partitionKey,
            CancellationToken cancellationToken = default)
        {
            var document = await _context.LoadAsync<TDocument>(partitionKey, cancellationToken);
            if (document == null)
            {
                return null;
            }

            var entity = LoadAndTrackDocument(document);

            return entity;
        }

        public async Task<TDomain?> FindByKeyAsync(
            TPartitionKey partitionKey,
            TSortKey sortKey,
            CancellationToken cancellationToken = default)
        {
            var document = await _context.LoadAsync<TDocument>(partitionKey, sortKey, cancellationToken);
            if (document == null)
            {
                return null;
            }

            var entity = LoadAndTrackDocument(document);

            return entity;
        }

        public async Task<List<TDomain>> FindByKeysAsync(
            IEnumerable<TPartitionKey> partitionKeys,
            CancellationToken cancellationToken = default)
        {
            var batch = _context.CreateBatchGet<TDocument>();

            foreach (var partitionKey in partitionKeys)
            {
                batch.AddKey(partitionKey);
            }

            await batch.ExecuteAsync(cancellationToken);

            return LoadAndTrackDocuments(batch.Results).ToList();
        }

        public async Task<List<TDomain>> FindByKeysAsync(
            IEnumerable<(TPartitionKey Partition, TSortKey Sort)> keys,
            CancellationToken cancellationToken = default)
        {
            var batch = _context.CreateBatchGet<TDocument>();

            foreach (var key in keys)
            {
                batch.AddKey(key.Partition, key.Sort);
            }

            await batch.ExecuteAsync(cancellationToken);

            return LoadAndTrackDocuments(batch.Results).ToList();
        }

        public async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var documents = await _context.ScanAsync<TDocument>(Enumerable.Empty<ScanCondition>()).GetRemainingAsync(cancellationToken);

            return LoadAndTrackDocuments(documents).ToList();
        }

        public TDomain LoadAndTrackDocument(TDocument document)
        {
            var entity = document.ToEntity();

            _unitOfWork.Track(entity);
            _versions[document.GetKey()] = document.GetVersion();

            return entity;
        }

        public IEnumerable<TDomain> LoadAndTrackDocuments(IEnumerable<TDocument> documents)
        {
            return documents.Select(LoadAndTrackDocument);
        }
    }
}