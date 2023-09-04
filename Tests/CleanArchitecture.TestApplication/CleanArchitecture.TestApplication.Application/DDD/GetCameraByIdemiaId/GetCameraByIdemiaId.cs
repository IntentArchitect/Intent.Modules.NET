using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.DDD.GetCameraByIdemiaId
{
    public class GetCameraByIdemiaId : IRequest<GetCameraDto>, IQuery
    {
        public GetCameraByIdemiaId(string idemiaId)
        {
            IdemiaId = idemiaId;
        }

        public string IdemiaId { get; set; }
    }
}