namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#nullable enable

internal class RouteParameter
{
    public string Name { get; set; }
    public string? DefaultValue { get; set; }
    public List<string> Constraints { get; set; } = [];
    public bool IsOptional { get; set; }
    public bool IsCatchAll { get; set; }

    public RouteParameter(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        
        if (IsCatchAll)
            sb.Append('*');
        
        sb.Append(Name);
        
        if (Constraints.Count != 0)
        {
            sb.Append(':');
            sb.Append(string.Join(":", Constraints));
        }
        
        if (!string.IsNullOrEmpty(DefaultValue))
        {
            sb.Append('=');
            sb.Append(DefaultValue);
        }
        
        return sb.ToString();
    }
}

internal class RouteExpressionParser
{
    private static readonly Regex ParameterRegex = new Regex(
        @"\{(\*?)([^}:=?]+)(?::([^}=?]+))?(?:=([^}?]+))?(\?)?\}",
        RegexOptions.Compiled);

    private static readonly Regex DoubleSlashRegex = new Regex(
        @"/+",
        RegexOptions.Compiled);

    private string? _originalRoute;
    public Dictionary<string, RouteParameter> Parameters { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

    public RouteExpressionParser Parse(string? routeExpression)
    {
        _originalRoute = routeExpression ?? string.Empty;
        Parameters.Clear();

        var matches = ParameterRegex.Matches(_originalRoute);
        
        foreach (Match match in matches)
        {
            var parameter = ParseParameter(match);
            Parameters[parameter.Name] = parameter;
        }

        return this;
    }

    private static RouteParameter ParseParameter(Match match)
    {
        var parameter = new RouteParameter(match.Groups[2].Value);
        
        // Check for catch-all (*)
        parameter.IsCatchAll = !string.IsNullOrEmpty(match.Groups[1].Value);
        
        // Constraints
        if (match.Groups[3].Success)
        {
            parameter.Constraints = match.Groups[3].Value.Split(':').ToList();
        }
        
        // Default value
        if (match.Groups[4].Success)
        {
            parameter.DefaultValue = match.Groups[4].Value;
        }
        
        // Optional parameter
        parameter.IsOptional = match.Groups[5].Success;

        return parameter;
    }

    public RouteParameter? GetParameter(string name)
    {
        Parameters.TryGetValue(name, out var parameter);
        return parameter;
    }

    public bool HasParameter(string name)
    {
        return Parameters.ContainsKey(name);
    }

    public void AddParameter(
        string name, 
        string? constraints = null, 
        string? defaultValue = null, 
        bool isOptional = false, 
        bool isCatchAll = false)
    {
        var parameter = new RouteParameter(name)
        {
            DefaultValue = defaultValue,
            IsOptional = isOptional,
            IsCatchAll = isCatchAll
        };

        if (!string.IsNullOrEmpty(constraints))
        {
            parameter.Constraints = constraints.Split(':').ToList();
        }

        Parameters[name] = parameter;
    }

    public bool RemoveParameter(string name)
    {
        return Parameters.Remove(name);
    }

    public void UpdateParameter(string name, Action<RouteParameter> updateAction)
    {
        if (Parameters.TryGetValue(name, out var parameter))
        {
            updateAction(parameter);
        }
    }

    public string BuildRouteExpression()
    {
        if (string.IsNullOrEmpty(_originalRoute))
        {
            return string.Empty;
        }

        var result = _originalRoute;

        // Replace existing parameters
        var matches = ParameterRegex.Matches(_originalRoute);
        var replacements = new List<(int start, int length, string replacement)>();

        foreach (Match match in matches)
        {
            var paramName = match.Groups[2].Value;
            
            if (Parameters.TryGetValue(paramName, out var parameter))
            {
                var paramString = parameter.IsOptional ? 
                    $"{{{parameter}?}}" : $"{{{parameter}}}";
                
                replacements.Add((match.Index, match.Length, paramString));
            }
            else
            {
                // Parameter was removed, replace it with empty string
                replacements.Add((match.Index, match.Length, string.Empty));
            }
        }

        // Apply replacements in reverse order to maintain indices
        foreach (var (start, length, replacement) in replacements.OrderByDescending(r => r.start))
        {
            result = result.Remove(start, length).Insert(start, replacement);
        }

        // Add new parameters that weren't in the original route
        var originalParamNames = matches
            .Select(m => m.Groups[2].Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var newParameters = Parameters.Values
            .Where(p => !originalParamNames.Contains(p.Name))
            .ToList();

        if (newParameters.Count != 0)
        {
            var newParamStrings = newParameters.Select(p => 
                p.IsOptional ? $"{{{p}?}}" : $"{{{p}}}");
            
            if (!string.IsNullOrEmpty(result) && !result.EndsWith('/'))
                result += "/";
            
            result += string.Join("/", newParamStrings);
        }
        
        return result;
    }

    public override string ToString()
    {
        return BuildRouteExpression();
    }
}
