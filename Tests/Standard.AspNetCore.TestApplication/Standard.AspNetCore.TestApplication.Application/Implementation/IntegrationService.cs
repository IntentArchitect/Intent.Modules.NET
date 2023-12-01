using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Integration;
using Standard.AspNetCore.TestApplication.Application.Interfaces;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class IntegrationService : IIntegrationService
    {
        public const string ReferenceNumber = "refnumber_1234";
        public const string DefaultString = "string value";
        public const int DefaultInt = 55;
        public const string ExceptionMessage = "Some exception message";
        public static readonly Guid DefaultGuid = Guid.Parse("b7698947-5237-4686-9571-442335426771");

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntegrationService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<CustomDTO> QueryParamOp(string param1, int param2, CancellationToken cancellationToken = default)
        {
            Assert.Equal(DefaultString, param1);
            Assert.Equal(DefaultInt, param2);
            return CustomDTO.Create(ReferenceNumber);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HeaderParamOp(string param1, CancellationToken cancellationToken = default)
        {
            Assert.Equal(DefaultString, param1);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task FormParamOp(string param1, int param2, CancellationToken cancellationToken = default)
        {
            Assert.Equal(DefaultString, param1);
            Assert.Equal(DefaultInt, param2);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task RouteParamOp(string param1, CancellationToken cancellationToken = default)
        {
            Assert.Equal(DefaultString, param1);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task BodyParamOp(CustomDTO param1, CancellationToken cancellationToken = default)
        {
            Assert.Equal(ReferenceNumber, param1.ReferenceNumber);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task ThrowsException(CancellationToken cancellationToken = default)
        {
            throw new Exception(ExceptionMessage);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> GetWrappedPrimitiveGuid(CancellationToken cancellationToken = default)
        {
            return DefaultGuid;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> GetWrappedPrimitiveString(CancellationToken cancellationToken = default)
        {
            return DefaultString;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<int> GetWrappedPrimitiveInt(CancellationToken cancellationToken = default)
        {
            return DefaultInt;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> GetPrimitiveGuid(CancellationToken cancellationToken = default)
        {
            return DefaultGuid;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> GetPrimitiveString(CancellationToken cancellationToken = default)
        {
            return DefaultString;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<int> GetPrimitiveInt(CancellationToken cancellationToken = default)
        {
            return DefaultInt;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<string>> GetPrimitiveStringList(CancellationToken cancellationToken = default)
        {
            return new List<string> { DefaultString };
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task NonHttpSettingsOperation(CancellationToken cancellationToken = default)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<CustomDTO> GetInvoiceOpWithReturnTypeWrapped(CancellationToken cancellationToken = default)
        {
            return CustomDTO.Create(ReferenceNumber);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<CustomDTO>> GetItems(List<string> ids, CancellationToken cancellationToken = default)
        {
            var items = Enumerable.Range(1, 10).Select(s => new CustomDTO { ReferenceNumber = s.ToString() }).ToArray();
            var intersect = items.Where(p => ids.Contains(p.ReferenceNumber)).ToList();
            return intersect;
        }

        public void Dispose()
        {
        }
    }
}