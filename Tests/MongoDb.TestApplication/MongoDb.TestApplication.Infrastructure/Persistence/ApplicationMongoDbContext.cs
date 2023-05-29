using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Common.Interfaces;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Entities.Associations;
using MongoDb.TestApplication.Domain.Entities.IdTypes;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Entities.NestedAssociations;
using MongoFramework;
using MongoFramework.Infrastructure.Mapping;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.ApplicationMongoDbContext", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence
{
    public class ApplicationMongoDbContext : MongoDbContext, IMongoDbUnitOfWork
    {
        public ApplicationMongoDbContext(IMongoDbConnection connection) : base(connection)
        {
        }

        public MongoDbSet<A_RequiredComposite> A_RequiredComposites { get; set; }
        public MongoDbSet<AggregateA> AggregateAs { get; set; }
        public MongoDbSet<AggregateB> AggregateBs { get; set; }
        public MongoDbSet<B_OptionalAggregate> B_OptionalAggregates { get; set; }
        public MongoDbSet<B_OptionalDependent> B_OptionalDependents { get; set; }
        public MongoDbSet<C_RequireComposite> C_RequireComposites { get; set; }
        public MongoDbSet<CompoundIndexEntity> CompoundIndexEntities { get; set; }
        public MongoDbSet<CompoundIndexEntityMultiParent> CompoundIndexEntityMultiParents { get; set; }
        public MongoDbSet<CompoundIndexEntitySingleParent> CompoundIndexEntitySingleParents { get; set; }
        public MongoDbSet<D_MultipleDependent> D_MultipleDependents { get; set; }
        public MongoDbSet<D_OptionalAggregate> D_OptionalAggregates { get; set; }
        public MongoDbSet<E_RequiredCompositeNav> E_RequiredCompositeNavs { get; set; }
        public MongoDbSet<F_OptionalAggregateNav> F_OptionalAggregateNavs { get; set; }
        public MongoDbSet<F_OptionalDependent> F_OptionalDependents { get; set; }
        public MongoDbSet<G_RequiredCompositeNav> G_RequiredCompositeNavs { get; set; }
        public MongoDbSet<H_MultipleDependent> H_MultipleDependents { get; set; }
        public MongoDbSet<H_OptionalAggregateNav> H_OptionalAggregateNavs { get; set; }
        public MongoDbSet<I_MultipleAggregate> I_MultipleAggregates { get; set; }
        public MongoDbSet<I_RequiredDependent> I_RequiredDependents { get; set; }
        public MongoDbSet<IdTypeGuid> IdTypeGuids { get; set; }
        public MongoDbSet<IdTypeOjectIdStr> IdTypeOjectIdStrs { get; set; }
        public MongoDbSet<J_MultipleAggregate> J_MultipleAggregates { get; set; }
        public MongoDbSet<J_MultipleDependent> J_MultipleDependents { get; set; }
        public MongoDbSet<K_MultipleAggregateNav> K_MultipleAggregateNavs { get; set; }
        public MongoDbSet<K_MultipleDependent> K_MultipleDependents { get; set; }
        public MongoDbSet<MultikeyIndexEntity> MultikeyIndexEntities { get; set; }
        public MongoDbSet<MultikeyIndexEntityMultiParent> MultikeyIndexEntityMultiParents { get; set; }
        public MongoDbSet<MultikeyIndexEntitySingleParent> MultikeyIndexEntitySingleParents { get; set; }
        public MongoDbSet<SingleIndexEntity> SingleIndexEntities { get; set; }
        public MongoDbSet<SingleIndexEntityMultiParent> SingleIndexEntityMultiParents { get; set; }
        public MongoDbSet<SingleIndexEntitySingleParent> SingleIndexEntitySingleParents { get; set; }
        public MongoDbSet<TextIndexEntity> TextIndexEntities { get; set; }
        public MongoDbSet<TextIndexEntityMultiParent> TextIndexEntityMultiParents { get; set; }
        public MongoDbSet<TextIndexEntitySingleParent> TextIndexEntitySingleParents { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await base.SaveChangesAsync(cancellationToken);
            return default;
        }

        protected override void Dispose(bool disposing)
        {
            // Don't call the base's dispose as it disposes the connection which is not recommended as per https://www.mongodb.com/docs/manual/administration/connection-pool-overview/
        }

        protected override void OnConfigureMapping(MappingBuilder mappingBuilder)
        {
            mappingBuilder.Entity<A_RequiredComposite>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<AggregateA>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<AggregateB>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<B_OptionalAggregate>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<B_OptionalDependent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<C_RequireComposite>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            // IntentIgnore
            mappingBuilder.Entity<CompoundIndexEntityMultiParent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => new
                {
                    entity.CompoundIndexEntityMultiChild.First().CompoundOne,
                    entity.CompoundIndexEntityMultiChild.First().CompoundTwo
                }, build => build.HasName("MultiCompoundIndex").IsDescending());

            // IntentIgnore
            mappingBuilder.Entity<CompoundIndexEntitySingleParent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => new
                {
                    entity.CompoundIndexEntitySingleChild.CompoundOne,
                    entity.CompoundIndexEntitySingleChild.CompoundTwo
                }, build => build.HasName("SingleCompoundIndex").IsDescending());

            // IntentIgnore
            mappingBuilder.Entity<CompoundIndexEntity>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => new { entity.CompoundOne, entity.CompoundTwo }, build => build.HasName("CompoundIndex"));

            mappingBuilder.Entity<D_MultipleDependent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<D_OptionalAggregate>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<E_RequiredCompositeNav>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<F_OptionalAggregateNav>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<F_OptionalDependent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<G_RequiredCompositeNav>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<H_MultipleDependent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<H_OptionalAggregateNav>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<I_MultipleAggregate>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<I_RequiredDependent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<IdTypeGuid>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.GuidKeyGenerator));

            mappingBuilder.Entity<IdTypeOjectIdStr>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<J_MultipleAggregate>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<J_MultipleDependent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<K_MultipleAggregateNav>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<K_MultipleDependent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            // IntentIgnore
            mappingBuilder.Entity<MultikeyIndexEntityMultiParent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.MultikeyIndexEntityMultiChild.First().MultiKey, build => build.HasName("MultiMultiKey"));

            // IntentIgnore
            mappingBuilder.Entity<MultikeyIndexEntitySingleParent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.MultikeyIndexEntitySingleChild.MultiKey, build => build.HasName("SingleMultiKey"));

            // IntentIgnore
            mappingBuilder.Entity<SingleIndexEntityMultiParent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.SingleIndexEntityMultiChild.First().SingleIndex, build => build.HasName("MultiSingleIndex"));

            // IntentIgnore
            mappingBuilder.Entity<SingleIndexEntitySingleParent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.SingleIndexEntitySingleChild.SingleIndex, build => build.HasName("SingleSingleIndex"));

            // IntentIgnore
            mappingBuilder.Entity<TextIndexEntity>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.FullText, build => build.HasName("TextIndex").HasType(IndexType.Text));

            // IntentIgnore
            mappingBuilder.Entity<TextIndexEntityMultiParent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.TextIndexEntityMultiChild.First().FullText, build => build.HasType(IndexType.Text).HasName("MultiTextIndex"));

            // IntentIgnore
            mappingBuilder.Entity<TextIndexEntitySingleParent>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.TextIndexEntitySingleChild.FullText, build => build.HasType(IndexType.Text).HasName("SingleTextIndex"));

            // IntentIgnore
            mappingBuilder.Entity<MultikeyIndexEntity>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.MultiKey, build => build.HasName("MultiKey").IsDescending());

            // IntentIgnore
            mappingBuilder.Entity<SingleIndexEntity>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.SingleIndex, build => build.HasName("SingleIndex").HasType(IndexType.Standard).IsDescending());

            base.OnConfigureMapping(mappingBuilder);
        }
    }
}