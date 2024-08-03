using EntityFrameworkCore.SqlServer.EF7.Domain.Common.Interfaces;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.Accounts;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.Accounts.NotSchema;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.Associations;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.BasicAudit;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.ExplicitKeys;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.Geometry;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.Indexes;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.NestedAssociations;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.NotSchema;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.SoftDelete;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.TimeConcepts;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.TPC.Polymorphic;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.TPH.Polymorphic;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.TPT.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.TPT.Polymorphic;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.ValueObjects;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.Accounts;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.Accounts.NotSchema;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.Associations;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.BasicAudit;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.ExplicitKeys;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.Geometry;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.Indexes;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.NestedAssociations;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.NotSchema;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.SoftDelete;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.TimeConcepts;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.TPC.Polymorphic;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.TPH.Polymorphic;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.TPT.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.TPT.Polymorphic;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.ValueObjects;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b =>
                    {
                        b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        b.UseDateOnlyTimeOnly();
                        b.UseNetTopologySuite();
                    });
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<ISchemaParentRepository, SchemaParentRepository>();
            services.AddTransient<ITableRepository, TableRepository>();
            services.AddTransient<ITableExplicitSchemaRepository, TableExplicitSchemaRepository>();
            services.AddTransient<ITableOverrideRepository, TableOverrideRepository>();
            services.AddTransient<ITablePlainRepository, TablePlainRepository>();
            services.AddTransient<IViewRepository, ViewRepository>();
            services.AddTransient<IViewExplicitSchemaRepository, ViewExplicitSchemaRepository>();
            services.AddTransient<IViewOverrideRepository, ViewOverrideRepository>();
            services.AddTransient<IAccTableRepository, AccTableRepository>();
            services.AddTransient<IAccTableOverrideRepository, AccTableOverrideRepository>();
            services.AddTransient<IAccViewRepository, AccViewRepository>();
            services.AddTransient<IAccViewOverrideRepository, AccViewOverrideRepository>();
            services.AddTransient<IAccTableFolderRepository, AccTableFolderRepository>();
            services.AddTransient<IAccViewFolderRepository, AccViewFolderRepository>();
            services.AddTransient<IA_RequiredCompositeRepository, A_RequiredCompositeRepository>();
            services.AddTransient<IAuthorRepository, AuthorRepository>();
            services.AddTransient<IB_OptionalAggregateRepository, B_OptionalAggregateRepository>();
            services.AddTransient<IB_OptionalDependentRepository, B_OptionalDependentRepository>();
            services.AddTransient<IBookRepository, BookRepository>();
            services.AddTransient<IC_RequiredCompositeRepository, C_RequiredCompositeRepository>();
            services.AddTransient<ID_MultipleDependentRepository, D_MultipleDependentRepository>();
            services.AddTransient<ID_OptionalAggregateRepository, D_OptionalAggregateRepository>();
            services.AddTransient<IE_RequiredCompositeNavRepository, E_RequiredCompositeNavRepository>();
            services.AddTransient<IF_OptionalAggregateNavRepository, F_OptionalAggregateNavRepository>();
            services.AddTransient<IF_OptionalDependentRepository, F_OptionalDependentRepository>();
            services.AddTransient<IG_RequiredCompositeNavRepository, G_RequiredCompositeNavRepository>();
            services.AddTransient<IH_MultipleDependentRepository, H_MultipleDependentRepository>();
            services.AddTransient<IH_OptionalAggregateNavRepository, H_OptionalAggregateNavRepository>();
            services.AddTransient<IJ_MultipleAggregateRepository, J_MultipleAggregateRepository>();
            services.AddTransient<IJ_RequiredDependentRepository, J_RequiredDependentRepository>();
            services.AddTransient<IK_SelfReferenceRepository, K_SelfReferenceRepository>();
            services.AddTransient<IL_SelfReferenceMultipleRepository, L_SelfReferenceMultipleRepository>();
            services.AddTransient<IM_SelfReferenceBiNavRepository, M_SelfReferenceBiNavRepository>();
            services.AddTransient<IN_ComplexRootRepository, N_ComplexRootRepository>();
            services.AddTransient<IO_DestNameDiffRepository, O_DestNameDiffRepository>();
            services.AddTransient<IP_SourceNameDiffRepository, P_SourceNameDiffRepository>();
            services.AddTransient<IQ_DestNameDiffRepository, Q_DestNameDiffRepository>();
            services.AddTransient<IR_SourceNameDiffRepository, R_SourceNameDiffRepository>();
            services.AddTransient<IAudit_DerivedClassRepository, Audit_DerivedClassRepository>();
            services.AddTransient<IAudit_SoloClassRepository, Audit_SoloClassRepository>();
            services.AddTransient<IFK_A_CompositeForeignKeyRepository, FK_A_CompositeForeignKeyRepository>();
            services.AddTransient<IFK_B_CompositeForeignKeyRepository, FK_B_CompositeForeignKeyRepository>();
            services.AddTransient<IParentNonStdIdRepository, ParentNonStdIdRepository>();
            services.AddTransient<IPK_A_CompositeKeyRepository, PK_A_CompositeKeyRepository>();
            services.AddTransient<IPK_B_CompositeKeyRepository, PK_B_CompositeKeyRepository>();
            services.AddTransient<IPK_PrimaryKeyIntRepository, PK_PrimaryKeyIntRepository>();
            services.AddTransient<IPK_PrimaryKeyLongRepository, PK_PrimaryKeyLongRepository>();
            services.AddTransient<IGeometryTypeRepository, GeometryTypeRepository>();
            services.AddTransient<IComplexDefaultIndexRepository, ComplexDefaultIndexRepository>();
            services.AddTransient<ICustomIndexRepository, CustomIndexRepository>();
            services.AddTransient<IDefaultIndexRepository, DefaultIndexRepository>();
            services.AddTransient<IDeviationIndexRepository, DeviationIndexRepository>();
            services.AddTransient<IParentIndexRepository, ParentIndexRepository>();
            services.AddTransient<ISortDirectionIndexRepository, SortDirectionIndexRepository>();
            services.AddTransient<ISortDirectionStereotypeRepository, SortDirectionStereotypeRepository>();
            services.AddTransient<IStereotypeIndexRepository, StereotypeIndexRepository>();
            services.AddTransient<IWithBaseIndexRepository, WithBaseIndexRepository>();
            services.AddTransient<IWithBaseIndexBaseRepository, WithBaseIndexBaseRepository>();
            services.AddTransient<IInhabitantRepository, InhabitantRepository>();
            services.AddTransient<ISunRepository, SunRepository>();
            services.AddTransient<ITextureRepository, TextureRepository>();
            services.AddTransient<ITreeRepository, TreeRepository>();
            services.AddTransient<IWormRepository, WormRepository>();
            services.AddTransient<ITableFolderRepository, TableFolderRepository>();
            services.AddTransient<IViewFolderRepository, ViewFolderRepository>();
            services.AddTransient<IClassWithSoftDeleteRepository, ClassWithSoftDeleteRepository>();
            services.AddTransient<ITimeEntityRepository, TimeEntityRepository>();
            services.AddTransient<ITPC_ConcreteBaseClassRepository, TPC_ConcreteBaseClassRepository>();
            services.AddTransient<ITPC_ConcreteBaseClassAssociatedRepository, TPC_ConcreteBaseClassAssociatedRepository>();
            services.AddTransient<ITPC_DerivedClassForAbstractRepository, TPC_DerivedClassForAbstractRepository>();
            services.AddTransient<ITPC_DerivedClassForAbstractAssociatedRepository, TPC_DerivedClassForAbstractAssociatedRepository>();
            services.AddTransient<ITPC_DerivedClassForConcreteRepository, TPC_DerivedClassForConcreteRepository>();
            services.AddTransient<ITPC_DerivedClassForConcreteAssociatedRepository, TPC_DerivedClassForConcreteAssociatedRepository>();
            services.AddTransient<ITPC_FkAssociatedClassRepository, TPC_FkAssociatedClassRepository>();
            services.AddTransient<ITPC_FkBaseClassRepository, TPC_FkBaseClassRepository>();
            services.AddTransient<ITPC_FkBaseClassAssociatedRepository, TPC_FkBaseClassAssociatedRepository>();
            services.AddTransient<ITPC_FkDerivedClassRepository, TPC_FkDerivedClassRepository>();
            services.AddTransient<ITPC_Poly_BaseClassNonAbstractRepository, TPC_Poly_BaseClassNonAbstractRepository>();
            services.AddTransient<ITPC_Poly_ConcreteARepository, TPC_Poly_ConcreteARepository>();
            services.AddTransient<ITPC_Poly_ConcreteBRepository, TPC_Poly_ConcreteBRepository>();
            services.AddTransient<ITPC_Poly_RootAbstract_AggrRepository, TPC_Poly_RootAbstract_AggrRepository>();
            services.AddTransient<ITPC_Poly_SecondLevelRepository, TPC_Poly_SecondLevelRepository>();
            services.AddTransient<ITPC_Poly_TopLevelRepository, TPC_Poly_TopLevelRepository>();
            services.AddTransient<ITPH_AbstractBaseClassRepository, TPH_AbstractBaseClassRepository>();
            services.AddTransient<ITPH_AbstractBaseClassAssociatedRepository, TPH_AbstractBaseClassAssociatedRepository>();
            services.AddTransient<ITPH_ConcreteBaseClassRepository, TPH_ConcreteBaseClassRepository>();
            services.AddTransient<ITPH_ConcreteBaseClassAssociatedRepository, TPH_ConcreteBaseClassAssociatedRepository>();
            services.AddTransient<ITPH_DerivedClassForAbstractRepository, TPH_DerivedClassForAbstractRepository>();
            services.AddTransient<ITPH_DerivedClassForAbstractAssociatedRepository, TPH_DerivedClassForAbstractAssociatedRepository>();
            services.AddTransient<ITPH_DerivedClassForConcreteRepository, TPH_DerivedClassForConcreteRepository>();
            services.AddTransient<ITPH_DerivedClassForConcreteAssociatedRepository, TPH_DerivedClassForConcreteAssociatedRepository>();
            services.AddTransient<ITPH_FkAssociatedClassRepository, TPH_FkAssociatedClassRepository>();
            services.AddTransient<ITPH_FkBaseClassRepository, TPH_FkBaseClassRepository>();
            services.AddTransient<ITPH_FkBaseClassAssociatedRepository, TPH_FkBaseClassAssociatedRepository>();
            services.AddTransient<ITPH_FkDerivedClassRepository, TPH_FkDerivedClassRepository>();
            services.AddTransient<ITPH_MiddleAbstract_LeafRepository, TPH_MiddleAbstract_LeafRepository>();
            services.AddTransient<ITPH_MiddleAbstract_RootRepository, TPH_MiddleAbstract_RootRepository>();
            services.AddTransient<ITPH_Poly_BaseClassNonAbstractRepository, TPH_Poly_BaseClassNonAbstractRepository>();
            services.AddTransient<ITPH_Poly_ConcreteARepository, TPH_Poly_ConcreteARepository>();
            services.AddTransient<ITPH_Poly_ConcreteBRepository, TPH_Poly_ConcreteBRepository>();
            services.AddTransient<ITPH_Poly_RootAbstractRepository, TPH_Poly_RootAbstractRepository>();
            services.AddTransient<ITPH_Poly_RootAbstract_AggrRepository, TPH_Poly_RootAbstract_AggrRepository>();
            services.AddTransient<ITPH_Poly_SecondLevelRepository, TPH_Poly_SecondLevelRepository>();
            services.AddTransient<ITPH_Poly_TopLevelRepository, TPH_Poly_TopLevelRepository>();
            services.AddTransient<ITPT_AbstractBaseClassRepository, TPT_AbstractBaseClassRepository>();
            services.AddTransient<ITPT_AbstractBaseClassAssociatedRepository, TPT_AbstractBaseClassAssociatedRepository>();
            services.AddTransient<ITPT_ConcreteBaseClassRepository, TPT_ConcreteBaseClassRepository>();
            services.AddTransient<ITPT_ConcreteBaseClassAssociatedRepository, TPT_ConcreteBaseClassAssociatedRepository>();
            services.AddTransient<ITPT_DerivedClassForAbstractRepository, TPT_DerivedClassForAbstractRepository>();
            services.AddTransient<ITPT_DerivedClassForAbstractAssociatedRepository, TPT_DerivedClassForAbstractAssociatedRepository>();
            services.AddTransient<ITPT_DerivedClassForConcreteRepository, TPT_DerivedClassForConcreteRepository>();
            services.AddTransient<ITPT_DerivedClassForConcreteAssociatedRepository, TPT_DerivedClassForConcreteAssociatedRepository>();
            services.AddTransient<ITPT_FkAssociatedClassRepository, TPT_FkAssociatedClassRepository>();
            services.AddTransient<ITPT_FkBaseClassRepository, TPT_FkBaseClassRepository>();
            services.AddTransient<ITPT_FkBaseClassAssociatedRepository, TPT_FkBaseClassAssociatedRepository>();
            services.AddTransient<ITPT_FkDerivedClassRepository, TPT_FkDerivedClassRepository>();
            services.AddTransient<ITPT_Poly_BaseClassNonAbstractRepository, TPT_Poly_BaseClassNonAbstractRepository>();
            services.AddTransient<ITPT_Poly_ConcreteARepository, TPT_Poly_ConcreteARepository>();
            services.AddTransient<ITPT_Poly_ConcreteBRepository, TPT_Poly_ConcreteBRepository>();
            services.AddTransient<ITPT_Poly_RootAbstractRepository, TPT_Poly_RootAbstractRepository>();
            services.AddTransient<ITPT_Poly_RootAbstract_AggrRepository, TPT_Poly_RootAbstract_AggrRepository>();
            services.AddTransient<ITPT_Poly_SecondLevelRepository, TPT_Poly_SecondLevelRepository>();
            services.AddTransient<ITPT_Poly_TopLevelRepository, TPT_Poly_TopLevelRepository>();
            services.AddTransient<IDictionaryWithKvPNormalRepository, DictionaryWithKvPNormalRepository>();
            services.AddTransient<IDictionaryWithKvPSerializedRepository, DictionaryWithKvPSerializedRepository>();
            services.AddTransient<IPersonWithAddressNormalRepository, PersonWithAddressNormalRepository>();
            services.AddTransient<IPersonWithAddressSerializedRepository, PersonWithAddressSerializedRepository>();
            return services;
        }
    }
}