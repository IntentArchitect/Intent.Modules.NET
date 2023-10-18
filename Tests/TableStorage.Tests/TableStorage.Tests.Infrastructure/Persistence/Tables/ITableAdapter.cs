using System;
using Azure.Data.Tables;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.TableStorage.TableStorageTableIAdapterInterface", Version = "1.0")]

namespace TableStorage.Tests.Infrastructure.Persistence.Tables
{
    internal interface ITableAdapter<TDomain, out TTable> : ITableAdapter
        where TDomain : class
        where TTable : ITableAdapter<TDomain, TTable>
    {
        TTable PopulateFromEntity(TDomain entity);
        TDomain ToEntity(TDomain? entity = null);
    }

    internal interface ITableAdapter : ITableEntity
    {
    }
}