using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using OperationModelExtensions = Intent.Metadata.WebApi.Api.OperationModelExtensions;
using ServiceModelExtensions = Intent.Metadata.WebApi.Api.ServiceModelExtensions;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller
{
    public class ControllerDecorator : ITemplateDecorator
    {
        public virtual int Priority { get; protected set; } = 0;

        public virtual IEnumerable<string> GetControllerAttributes() => Enumerable.Empty<string>();

        public virtual string BaseClass() => null;

        public virtual string EnterClass() => null;

        public virtual string ExitClass() => null;

        public virtual string ConstructorImplementation() => null;

        public virtual IEnumerable<string> ConstructorParameters() => Enumerable.Empty<string>();

        public virtual string EnterOperationBody(OperationModel operationModel) => null;

        public virtual string MidOperationBody(OperationModel operationModel) => null;

        public virtual string ExitOperationBody(OperationModel operationModel) => null;

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
            ServiceModel serviceModel,
            ServiceModelStereotypeExtensions.Secured stereotype)
        {
            ServiceModel = serviceModel;
            Stereotype = stereotype;
        }

        public ServiceModel ServiceModel { get; }
        public ServiceModelStereotypeExtensions.Secured Stereotype { get; }
    }

    public class OperationSecureModel
    {
        internal OperationSecureModel(
            OperationModel operationModel,
            OperationModelStereotypeExtensions.Secured stereotype)
        {
            OperationModel = operationModel;
            Stereotype = stereotype;
        }

        public OperationModel OperationModel { get; }
        public OperationModelStereotypeExtensions.Secured Stereotype { get; }
    }

    public class AuthorizationModel
    {
        ///<summary>
        /// Gets or sets the Authentication Schemes that determines access to this resource. Note the format will generate exactly in C#.
        ///</summary>
        public string AuthenticationSchemesExpression { get; set; }

        ///<summary>
        /// Gets or sets the policy name that determines access to the resource. Note the format will generate exactly in C#.
        ///</summary>
        public string Policy { get; set; }

        ///<summary>
        /// Gets or sets the Roles that determines access to this Resource. Note the format will generate exactly in C#.
        ///</summary>
        public string RolesExpression { get; set; }
    }
}
