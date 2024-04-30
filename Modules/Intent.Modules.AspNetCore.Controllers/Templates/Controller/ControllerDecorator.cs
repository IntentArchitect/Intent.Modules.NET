using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using OperationModelExtensions = Intent.Metadata.WebApi.Api.OperationModelExtensions;
using ServiceModelExtensions = Intent.Metadata.WebApi.Api.ServiceModelExtensions;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller
{
    public class ControllerDecorator : ITemplateDecorator
    {
        public virtual int Priority { get; protected set; } = 0;

        public virtual IEnumerable<string> GetControllerAttributes() => Enumerable.Empty<string>();

        //public virtual string BaseClass() => null;

        //public virtual string EnterClass() => null;

        //public virtual string ExitClass() => null;

        public virtual string ConstructorImplementation() => null;

        public virtual string ConstructorBaseCall() => null;

        public virtual IEnumerable<string> ConstructorParameters() => Enumerable.Empty<string>();

        public virtual IEnumerable<string> GetOperationAttributes(OperationModel operationModel) => Enumerable.Empty<string>();

        //public virtual string EnterOperationBody(OperationModel operationModel) => null;

        //public virtual string MidOperationBody(OperationModel operationModel) => null;

        //public virtual string ExitOperationBody(OperationModel operationModel) => null;

        public virtual void UpdateServiceAuthorization(AuthorizationModel authorizationModel, ServiceSecureModel secureModel)
        {
        }

        public virtual void UpdateOperationAuthorization(AuthorizationModel authorizationModel, OperationSecureModel secureModel)
        {
        }
    }

    public class ServiceSecureModel
    {
        internal ServiceSecureModel(
            IControllerModel serviceModel,
            ServiceModelStereotypeExtensions.Secured stereotype)
        {
            ServiceModel = serviceModel;
            Stereotype = stereotype;
        }

        public IControllerModel ServiceModel { get; }
        public ServiceModelStereotypeExtensions.Secured Stereotype { get; }
    }

    public class OperationSecureModel
    {
        internal OperationSecureModel(
            IControllerOperationModel operationModel,
            OperationModelStereotypeExtensions.Secured stereotype)
        {
            OperationModel = operationModel;
            Stereotype = stereotype;
        }

        public IControllerOperationModel OperationModel { get; }
        public OperationModelStereotypeExtensions.Secured Stereotype { get; }
    }
}
