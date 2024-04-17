using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Common.Interfaces;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Entities.Associations;
using MongoDb.TestApplication.Domain.Entities.Collections;
using MongoDb.TestApplication.Domain.Entities.Collections.FolderCollection;
using MongoDb.TestApplication.Domain.Entities.IdTypes;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Entities.Mappings;
using MongoDb.TestApplication.Domain.Entities.NestedAssociations;
using MongoDb.TestApplication.Domain.Entities.ToManyIds;
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
        public MongoDbSet<BaseType> BaseTypes { get; set; }
        public MongoDbSet<C_RequireComposite> C_RequireComposites { get; set; }
        public MongoDbSet<CompoundIndexEntity> CompoundIndexEntities { get; set; }
        public MongoDbSet<CompoundIndexEntityMultiParent> CompoundIndexEntityMultiParents { get; set; }
        public MongoDbSet<CompoundIndexEntitySingleParent> CompoundIndexEntitySingleParents { get; set; }
        public MongoDbSet<CustomCollectionEntityA> CustomCollectionEntityAs { get; set; }
        public MongoDbSet<CustomCollectionEntityB> CustomCollectionEntityBs { get; set; }
        public MongoDbSet<D_MultipleDependent> D_MultipleDependents { get; set; }
        public MongoDbSet<D_OptionalAggregate> D_OptionalAggregates { get; set; }
        public MongoDbSet<Derived> Deriveds { get; set; }
        public MongoDbSet<DerivedOfT> DerivedOfTs { get; set; }
        public MongoDbSet<E_RequiredCompositeNav> E_RequiredCompositeNavs { get; set; }
        public MongoDbSet<F_OptionalAggregateNav> F_OptionalAggregateNavs { get; set; }
        public MongoDbSet<F_OptionalDependent> F_OptionalDependents { get; set; }
        public MongoDbSet<FolderCollectionEntityA> FolderCollectionEntityAs { get; set; }
        public MongoDbSet<FolderCollectionEntityB> FolderCollectionEntityBs { get; set; }
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
        public MongoDbSet<MapAggChild> MapAggChildren { get; set; }
        public MongoDbSet<MapAggPeer> MapAggPeers { get; set; }
        public MongoDbSet<MapAggPeerAgg> MapAggPeerAggs { get; set; }
        public MongoDbSet<MapAggPeerAggMore> MapAggPeerAggMores { get; set; }
        public MongoDbSet<MapCompChildAgg> MapCompChildAggs { get; set; }
        public MongoDbSet<MapImplyOptional> MapImplyOptionals { get; set; }
        public MongoDbSet<MapMapMe> MapMapMes { get; set; }
        public MongoDbSet<MapPeerCompChildAgg> MapPeerCompChildAggs { get; set; }
        public MongoDbSet<MapperM2M> MapperM2Ms { get; set; }
        public MongoDbSet<MapperRoot> MapperRoots { get; set; }
        public MongoDbSet<MultikeyIndexEntity> MultikeyIndexEntities { get; set; }
        public MongoDbSet<MultikeyIndexEntityMultiParent> MultikeyIndexEntityMultiParents { get; set; }
        public MongoDbSet<MultikeyIndexEntitySingleParent> MultikeyIndexEntitySingleParents { get; set; }
        public MongoDbSet<SingleIndexEntity> SingleIndexEntities { get; set; }
        public MongoDbSet<SingleIndexEntityMultiParent> SingleIndexEntityMultiParents { get; set; }
        public MongoDbSet<SingleIndexEntitySingleParent> SingleIndexEntitySingleParents { get; set; }
        public MongoDbSet<TextIndexEntity> TextIndexEntities { get; set; }
        public MongoDbSet<TextIndexEntityMultiParent> TextIndexEntityMultiParents { get; set; }
        public MongoDbSet<TextIndexEntitySingleParent> TextIndexEntitySingleParents { get; set; }
        public MongoDbSet<ToManySource> ToManySources { get; set; }

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
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<AggregateA>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<AggregateB>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<B_OptionalAggregate>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<B_OptionalDependent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<BaseType>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<C_RequireComposite>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<CompoundIndexEntity>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => new
                {
                    entity.CompoundOne,
                    entity.CompoundTwo
                }, build => build
                    .HasName("CompoundIndexEntities_CompoundOne_CompoundTwo")
                    .HasType(IndexType.Standard));

            mappingBuilder.Entity<CompoundIndexEntityMultiParent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => new
                {
                    entity.CompoundIndexEntityMultiChild.First().CompoundOne,
                    entity.CompoundIndexEntityMultiChild.First().CompoundTwo
                }, build => build
                    .HasName("CompoundIndexEntityMultiParents_CompoundOne_CompoundTwo")
                    .HasType(IndexType.Standard));

            mappingBuilder.Entity<CompoundIndexEntitySingleParent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => new
                {
                    entity.CompoundIndexEntitySingleChild.CompoundOne,
                    entity.CompoundIndexEntitySingleChild.CompoundTwo
                }, build => build
                    .HasName("CompoundIndexEntitySingleParents_CompoundOne_CompoundTwo")
                    .HasType(IndexType.Standard));

            mappingBuilder.Entity<CustomCollectionEntityA>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .ToCollection("CustomCollection");

            mappingBuilder.Entity<CustomCollectionEntityB>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .ToCollection("CustomCollection");

            mappingBuilder.Entity<D_MultipleDependent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<D_OptionalAggregate>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<Derived>();

            mappingBuilder.Entity<DerivedOfT>();

            mappingBuilder.Entity<E_RequiredCompositeNav>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<F_OptionalAggregateNav>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<F_OptionalDependent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<FolderCollectionEntityA>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .ToCollection("FolderCollection");

            mappingBuilder.Entity<FolderCollectionEntityB>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .ToCollection("FolderCollection");

            mappingBuilder.Entity<G_RequiredCompositeNav>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<H_MultipleDependent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<H_OptionalAggregateNav>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<I_MultipleAggregate>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<I_RequiredDependent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<IdTypeGuid>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.GuidKeyGenerator));

            mappingBuilder.Entity<IdTypeOjectIdStr>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<J_MultipleAggregate>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<J_MultipleDependent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<K_MultipleAggregateNav>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<K_MultipleDependent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<MapAggChild>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<MapAggPeer>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<MapAggPeerAgg>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<MapAggPeerAggMore>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<MapCompChildAgg>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<MapImplyOptional>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<MapMapMe>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<MapPeerCompChildAgg>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<MapperM2M>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<MapperRoot>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<MultikeyIndexEntity>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.MultiKey, build => build
                    .HasName("MultikeyIndexEntities_MultiKey")
                    .HasType(IndexType.Standard));

            mappingBuilder.Entity<MultikeyIndexEntityMultiParent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.MultikeyIndexEntityMultiChild.First().MultiKey, build => build
                    .HasName("MultikeyIndexEntityMultiParents_MultiKey")
                    .HasType(IndexType.Standard));

            mappingBuilder.Entity<MultikeyIndexEntitySingleParent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.MultikeyIndexEntitySingleChild.MultiKey, build => build
                    .HasName("MultikeyIndexEntitySingleParents_MultiKey")
                    .HasType(IndexType.Standard));

            mappingBuilder.Entity<SingleIndexEntity>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.SingleIndex, build => build
                    .HasName("SingleIndexEntities_SingleIndex")
                    .HasType(IndexType.Standard));

            mappingBuilder.Entity<SingleIndexEntityMultiParent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.SingleIndexEntityMultiChild.First().SingleIndex, build => build
                    .HasName("SingleIndexEntityMultiParents_SingleIndex")
                    .HasType(IndexType.Standard));

            mappingBuilder.Entity<SingleIndexEntitySingleParent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.SingleIndexEntitySingleChild.SingleIndex, build => build
                    .HasName("SingleIndexEntitySingleParents_SingleIndex")
                    .HasType(IndexType.Standard));

            mappingBuilder.Entity<TextIndexEntity>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.FullText, build => build
                    .HasName("TextIndexEntities_FullText")
                    .HasType(IndexType.Standard));

            mappingBuilder.Entity<TextIndexEntityMultiParent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.TextIndexEntityMultiChild.First().FullText, build => build
                    .HasName("TextIndexEntityMultiParents_FullText")
                    .HasType(IndexType.Standard));

            mappingBuilder.Entity<TextIndexEntitySingleParent>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .HasIndex(entity => entity.TextIndexEntitySingleChild.FullText, build => build
                    .HasName("TextIndexEntitySingleParents_FullText")
                    .HasType(IndexType.Standard));

            mappingBuilder.Entity<ToManySource>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));
            base.OnConfigureMapping(mappingBuilder);
        }
    }
}