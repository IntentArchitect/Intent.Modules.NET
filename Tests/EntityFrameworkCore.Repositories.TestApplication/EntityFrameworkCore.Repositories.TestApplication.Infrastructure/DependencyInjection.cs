using System.Reflection;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Application;
using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Interfaces;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Common.Interfaces;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.MappableStoredProcs;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.PrimaryKeyTypes;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.MappableStoredProcs;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.PrimaryKeyTypes;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IAsyncRepositoryTestRepository, AsyncRepositoryTestRepository>();
            services.AddTransient<ICustomRepository, CustomRepository>();
            services.AddTransient<ISqlOutParameterRepository, SqlOutParameterRepository>();
            services.AddTransient<ISqlParameterRepositoryReturningScalarRepository, SqlParameterRepositoryReturningScalarRepository>();
            services.AddTransient<ISqlParameterRepositoryWithNameRepository, SqlParameterRepositoryWithNameRepository>();
            services.AddTransient<ISqlParameterRepositoryWithoutNameRepository, SqlParameterRepositoryWithoutNameRepository>();
            services.AddTransient<IStoredProcOperationsRepository, StoredProcOperationsRepository>();
            services.AddTransient<IMappableStoredProcRepository, MappableStoredProcRepository>();
            services.AddTransient<IAggregateRoot1Repository, AggregateRoot1Repository>();
            services.AddTransient<IAggregateRoot2CompositionRepository, AggregateRoot2CompositionRepository>();
            services.AddTransient<IAggregateRoot3AggCollectionRepository, AggregateRoot3AggCollectionRepository>();
            services.AddTransient<IAggregateRoot3CollectionRepository, AggregateRoot3CollectionRepository>();
            services.AddTransient<IAggregateRoot3NullableRepository, AggregateRoot3NullableRepository>();
            services.AddTransient<IAggregateRoot3SingleRepository, AggregateRoot3SingleRepository>();
            services.AddTransient<IAggregateRoot4AggNullableRepository, AggregateRoot4AggNullableRepository>();
            services.AddTransient<IAggregateRoot4CollectionRepository, AggregateRoot4CollectionRepository>();
            services.AddTransient<IAggregateRoot4NullableRepository, AggregateRoot4NullableRepository>();
            services.AddTransient<IAggregateRoot4SingleRepository, AggregateRoot4SingleRepository>();
            services.AddTransient<IAggregateRoot5Repository, AggregateRoot5Repository>();
            services.AddTransient<IAggregateRoot5EntityWithRepoRepository, AggregateRoot5EntityWithRepoRepository>();
            services.AddTransient<IMockEntityRepository, MockEntityRepository>();
            services.AddTransient<INewClassByteRepository, NewClassByteRepository>();
            services.AddTransient<INewClassDecimalRepository, NewClassDecimalRepository>();
            services.AddTransient<INewClassDoubleRepository, NewClassDoubleRepository>();
            services.AddTransient<INewClassFloatRepository, NewClassFloatRepository>();
            services.AddTransient<INewClassGuidRepository, NewClassGuidRepository>();
            services.AddTransient<INewClassIntRepository, NewClassIntRepository>();
            services.AddTransient<INewClassLongRepository, NewClassLongRepository>();
            services.AddTransient<INewClassShortRepository, NewClassShortRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}