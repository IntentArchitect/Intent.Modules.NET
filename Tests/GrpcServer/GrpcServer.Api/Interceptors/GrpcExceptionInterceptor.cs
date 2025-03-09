using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Google.Rpc;
using Grpc.Core;
using Grpc.Core.Interceptors;
using GrpcServer.Application.Common.Exceptions;
using GrpcServer.Domain.Common.Exceptions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Status = Google.Rpc.Status;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.GrpcExceptionInterceptor", Version = "1.0")]

namespace GrpcServer.Api.Interceptors
{
    public class GrpcExceptionInterceptor : Interceptor
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GrpcExceptionInterceptor(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await base.UnaryServerHandler(request, context, continuation);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ValidationException exception:
                        throw new Status
                        {
                            Code = (int)Code.InvalidArgument,
                            Message = "Bad request",
                            Details =
                            {
                                Any.Pack(
                                    new BadRequest
                                    {
                                        FieldViolations =
                                        {
                                            exception.Errors.Select(x => new BadRequest.Types.FieldViolation { Field = x.PropertyName, Description = x.ErrorMessage })
                                        }
                                    })
                            }
                        }.ToRpcException();
                    case ForbiddenAccessException:
                        throw new Status { Code = (int)Code.PermissionDenied, Message = "Permission denied" }.ToRpcException();
                    case UnauthorizedAccessException:
                        throw new Status { Code = (int)Code.Unauthenticated, Message = "Unauthenticated" }.ToRpcException();
                    case NotFoundException exception:
                        throw new Status { Code = (int)Code.NotFound, Message = exception.Message }.ToRpcException();
                    default:
                        if (_webHostEnvironment.IsDevelopment())
                        {
                            throw new Status
                            {
                                Code = (int)Code.Internal,
                                Message = "Internal error",
                                Details =
                                {
                                    Any.Pack(e.ToRpcDebugInfo())
                                }
                            }.ToRpcException();
                        }

                        throw;
                }
            }
        }
    }
}