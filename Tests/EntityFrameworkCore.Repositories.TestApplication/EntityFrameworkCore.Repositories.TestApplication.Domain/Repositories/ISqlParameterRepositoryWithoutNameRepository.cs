using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories
{
    public interface ISqlParameterRepositoryWithoutNameRepository
    {
        Task Input0_tableType0_output0_return0(CancellationToken cancellationToken = default);
        Task<SpResult> Input0_tableType0_output0_return1(CancellationToken cancellationToken = default);
        Task<int> Input0_tableType0_output1_return0(CancellationToken cancellationToken = default);
        Task<(SpResult Result, int Output0Output)> Input0_tableType0_output1_return1(CancellationToken cancellationToken = default);
        Task Input0_tableType1_output0_return0(IEnumerable<SpParameter> tableType0, CancellationToken cancellationToken = default);
        Task<SpResult> Input0_tableType1_output0_return1(IEnumerable<SpParameter> tableType0, CancellationToken cancellationToken = default);
        Task<int> Input0_tableType1_output1_return0(IEnumerable<SpParameter> tableType0, CancellationToken cancellationToken = default);
        Task<(SpResult Result, int Output0Output)> Input0_tableType1_output1_return1(IEnumerable<SpParameter> tableType0, CancellationToken cancellationToken = default);
        Task Input1_tableType0_output0_return0(int input0, CancellationToken cancellationToken = default);
        Task<SpResult> Input1_tableType0_output0_return1(int input0, CancellationToken cancellationToken = default);
        Task<int> Input1_tableType0_output1_return0(int input0, CancellationToken cancellationToken = default);
        Task<(SpResult Result, int Output0Output)> Input1_tableType0_output1_return1(int input0, CancellationToken cancellationToken = default);
        Task Input1_tableType1_output0_return0(IEnumerable<SpParameter> tableType0, int input0, CancellationToken cancellationToken = default);
        Task<SpResult> Input1_tableType1_output0_return1(IEnumerable<SpParameter> tableType0, int input0, CancellationToken cancellationToken = default);
        Task<int> Input1_tableType1_output1_return0(IEnumerable<SpParameter> tableType0, int input0, CancellationToken cancellationToken = default);
        Task<(SpResult Result, int Output0Output)> Input1_tableType1_output1_return1(IEnumerable<SpParameter> tableType0, int input0, CancellationToken cancellationToken = default);
    }
}