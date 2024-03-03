using System.Reflection;
using AutoMapper;
using EntityFrameworkCore.CosmosDb.TestApplication.Application;
using EntityFrameworkCore.CosmosDb.TestApplication.Application.Common.Interfaces;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common.Interfaces;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.Associations;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.BasicAudit;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.FolderContainer;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.Inheritance;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.InheritanceAssociations;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.NestedComposition;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.Polymorphic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.SoftDelete;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.ValueObjects;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Repositories;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Repositories.Associations;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Repositories.BasicAudit;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Repositories.FolderContainer;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Repositories.Inheritance;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Repositories.InheritanceAssociations;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Repositories.NestedComposition;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Repositories.Polymorphic;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Repositories.SoftDelete;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Repositories.ValueObjects;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseCosmos(
                    configuration["Cosmos:AccountEndpoint"],
                    configuration["Cosmos:AccountKey"],
                    configuration["Cosmos:DatabaseName"]);
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IExplicitKeyClassRepository, ExplicitKeyClassRepository>();
            services.AddTransient<INormalEntityRepository, NormalEntityRepository>();
            services.AddTransient<ISelfContainedEntityRepository, SelfContainedEntityRepository>();
            services.AddTransient<IA_RequiredCompositeRepository, A_RequiredCompositeRepository>();
            services.AddTransient<IB_OptionalAggregateRepository, B_OptionalAggregateRepository>();
            services.AddTransient<IB_OptionalDependentRepository, B_OptionalDependentRepository>();
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
            services.AddTransient<IM_SelfReferenceBiNavRepository, M_SelfReferenceBiNavRepository>();
            services.AddTransient<IN_ComplexRootRepository, N_ComplexRootRepository>();
            services.AddTransient<IO_DestNameDiffRepository, O_DestNameDiffRepository>();
            services.AddTransient<IP_SourceNameDiffRepository, P_SourceNameDiffRepository>();
            services.AddTransient<IQ_DestNameDiffRepository, Q_DestNameDiffRepository>();
            services.AddTransient<IR_SourceNameDiffRepository, R_SourceNameDiffRepository>();
            services.AddTransient<IS_NoPkInCompositeRepository, S_NoPkInCompositeRepository>();
            services.AddTransient<IT_NoPkInCompositeRepository, T_NoPkInCompositeRepository>();
            services.AddTransient<IAudit_DerivedClassRepository, Audit_DerivedClassRepository>();
            services.AddTransient<IAudit_SoloClassRepository, Audit_SoloClassRepository>();
            services.AddTransient<IFolderEntityRepository, FolderEntityRepository>();
            services.AddTransient<IAssociatedRepository, AssociatedRepository>();
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<IBaseAssociatedRepository, BaseAssociatedRepository>();
            services.AddTransient<IDerivedRepository, DerivedRepository>();
            services.AddTransient<IWeirdClassRepository, WeirdClassRepository>();
            services.AddTransient<IAbstractBaseClassAssociatedRepository, AbstractBaseClassAssociatedRepository>();
            services.AddTransient<IConcreteBaseClassRepository, ConcreteBaseClassRepository>();
            services.AddTransient<IConcreteBaseClassAssociatedRepository, ConcreteBaseClassAssociatedRepository>();
            services.AddTransient<IDerivedClassForAbstractRepository, DerivedClassForAbstractRepository>();
            services.AddTransient<IDerivedClassForAbstractAssociatedRepository, DerivedClassForAbstractAssociatedRepository>();
            services.AddTransient<IDerivedClassForConcreteRepository, DerivedClassForConcreteRepository>();
            services.AddTransient<IDerivedClassForConcreteAssociatedRepository, DerivedClassForConcreteAssociatedRepository>();
            services.AddTransient<IMiddleAbstract_LeafRepository, MiddleAbstract_LeafRepository>();
            services.AddTransient<IMiddleAbstract_RootRepository, MiddleAbstract_RootRepository>();
            services.AddTransient<IStandaloneDerivedRepository, StandaloneDerivedRepository>();
            services.AddTransient<IClassARepository, ClassARepository>();
            services.AddTransient<IPoly_ConcreteARepository, Poly_ConcreteARepository>();
            services.AddTransient<IPoly_ConcreteBRepository, Poly_ConcreteBRepository>();
            services.AddTransient<IPoly_SecondLevelRepository, Poly_SecondLevelRepository>();
            services.AddTransient<IClassWithSoftDeleteRepository, ClassWithSoftDeleteRepository>();
            services.AddTransient<IDictionaryWithKvPNormalRepository, DictionaryWithKvPNormalRepository>();
            services.AddTransient<IDictionaryWithKvPSerializedRepository, DictionaryWithKvPSerializedRepository>();
            services.AddTransient<IPersonWithAddressNormalRepository, PersonWithAddressNormalRepository>();
            services.AddTransient<IPersonWithAddressSerializedRepository, PersonWithAddressSerializedRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}