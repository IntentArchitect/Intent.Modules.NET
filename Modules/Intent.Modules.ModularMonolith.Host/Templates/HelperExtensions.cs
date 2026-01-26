using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.ModularMonolith.Host.Templates;

public static class HelperExtensions
{
    public static bool IsNetAbove(string tfm, Version minimumExclusive)
    {
        if (string.IsNullOrWhiteSpace(tfm)) return false;

        // Remove platform qualifier: "net9.0-windows10.0" -> "net9.0"
        var baseTfm = tfm.Split('-', 2)[0];

        // Must start with "net"
        if (!baseTfm.StartsWith("net", StringComparison.OrdinalIgnoreCase))
            return false;

        // Grab what's after "net"
        var verText = baseTfm.Substring(3);

        // Version.TryParse handles "9.0", "10.0", "8.0.1", etc.
        if (!Version.TryParse(verText, out var v))
            return false;

        return v > minimumExclusive;
    }
}
