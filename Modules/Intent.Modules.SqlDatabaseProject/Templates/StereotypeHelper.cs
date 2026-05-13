using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.SqlDatabaseProject.Templates;

internal static class StereotypeHelper
{
    public static IStereotype? GetTextLimits(this AttributeModel model)
    {
        return model.GetStereotype("13649b19-4dfe-43ec-967f-0b85a5801dd6");
    }
}
