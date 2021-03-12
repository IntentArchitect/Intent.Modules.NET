using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modelers.Services.Api;
using Intent.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller
{
    public class ControllerDecorator : ITemplateDecorator
    {
        public virtual int Priority { get; protected set; } = 0;

        public virtual string BaseClass()
        {
            return null;
        }

        public virtual string EnterClass()
        {
            return null;
        }

        public virtual string ExitClass()
        {
            return null;
        }

        public virtual string ConstructorImplementation()
        {
            return null;
        }

        public virtual IEnumerable<string> ConstructorParameters()
        {
            return new string[0];
        }

        public virtual string EnterOperationBody(OperationModel operationModel)
        {
            return null;
        }

        public virtual string MidOperationBody(OperationModel operationModel)
        {
            return null;
        }

        public virtual string ExitOperationBody(OperationModel operationModel)
        {
            return null;
        }

        public virtual void UpdateServiceAuhtorization(AuthorizationModel authorizationModel, ServiceSecureModel secureModel)
        {
        }

        public virtual void UpdateOperationAuhtorization(AuthorizationModel authorizationModel, OperationSecureModel secureModel)
        {
        }
    }

    public class ServiceSecureModel
    {
        internal ServiceSecureModel(
            ServiceModel serviceModel,
            Intent.AspNetCore.Controllers.Api.ServiceModelExtensions.Secured stereotype)
        {
            ServiceModel = serviceModel;
            Stereotype = stereotype;
        }

        public ServiceModel ServiceModel { get; }
        public Intent.AspNetCore.Controllers.Api.ServiceModelExtensions.Secured Stereotype { get; }
    }

    public class OperationSecureModel
    {
        internal OperationSecureModel(
            OperationModel operationModel,
            Intent.AspNetCore.Controllers.Api.OperationModelExtensions.Secured stereotype)
        {
            OperationModel = operationModel;
            Stereotype = stereotype;
        }

        public OperationModel OperationModel { get; }
        public Intent.AspNetCore.Controllers.Api.OperationModelExtensions.Secured Stereotype { get; }
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
