using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Security.Shared;
public class PermissionConversionOptions
{
    public string ConvertedCollectionFormat { get; set; } = "$\"{0}\"";

    public string ConvertedNameFormat { get; set; } = "{{{0}}}";

    public string UnconvertedCollectionFormat { get; set; } = "\"{0}\"";

    public string UnconvertedNameFormat { get; set; } = "{0}";

    public bool OuputFullyQualifiedName { get; set; } = false;
}
