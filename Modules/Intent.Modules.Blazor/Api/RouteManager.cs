using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Intent.Modules.Blazor.Api;

public class RouteManager
{
    public RouteManager(string route)
    {
        Route = route;
    }

    public string Route { get; private set; }

    public bool HasParameterExpression(string parameterName)
    {
        foreach (var expression in GetExpressions(Route))
        {
            var name = Regex.Split(expression, @"[:?]").First();
            if (parameterName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public void ReplaceParameterExpression(string parameterName, string replaceWith)
    {
        foreach (var expression in GetExpressions(Route))
        {
            var name = Regex.Split(expression, @"[:?]").First();
            if (parameterName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                Route = Route.Replace($"{{{expression}}}", replaceWith);
            }
        }
    }

    private List<string> GetExpressions(string str)
    {
        var result = new List<string>();
        while (str.IndexOf("{", StringComparison.Ordinal) != -1 && str.IndexOf("}", StringComparison.Ordinal) != -1)
        {
            var fromPos = str.IndexOf("{", StringComparison.Ordinal) + 1;
            var toPos = str.IndexOf("}", StringComparison.Ordinal);
            var expression = str[fromPos..toPos];
            result.Add(expression);
            str = str[(str.IndexOf("}", StringComparison.Ordinal) + 1)..];
        }

        return result;
    }
}