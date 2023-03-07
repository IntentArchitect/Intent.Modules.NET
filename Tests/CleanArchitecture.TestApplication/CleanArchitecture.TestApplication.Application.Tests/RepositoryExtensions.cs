using System;
using System.Threading;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using NSubstitute;

namespace CleanArchitecture.TestApplication.Application.Tests;

public static class RepositoryExtensions
{
    public static void OnAdd<TDomain, TPersistence>(
        this IRepository<TDomain, TPersistence> repository, 
        Action<TDomain> addAction)
    {
        repository.When(x => x.Add(Arg.Any<TDomain>())).Do(ci => addAction(ci.Arg<TDomain>()));
        repository.UnitOfWork.When(async x => await x.SaveChangesAsync(CancellationToken.None)).Do(_ => addedAggregateRoot.Id = expectedAggregateRoot.Id);
    }
}