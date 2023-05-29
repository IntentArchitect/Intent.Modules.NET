using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClassHelper", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Api
{
    static class AzureFunctionHelper
    {
        public static T GetQueryParam<T>(string paramName, IQueryCollection query, ParseDelegate<T> parse)
            where T : struct
        {
            var strVal = query[paramName];
            if (string.IsNullOrEmpty(strVal) || !parse(strVal, out T parsed))
            {
                throw new FormatException($"Parameter '{paramName}' could not be parsed as a {typeof(T).Name}.");
            }

            return parsed;
        }

        public static T? GetQueryParamNullable<T>(string paramName, IQueryCollection query, ParseDelegate<T> parse)
            where T : struct
        {
            var strVal = query[paramName];
            if (string.IsNullOrEmpty(strVal))
            {
                return null;
            }

            if (!parse(strVal, out T parsed))
            {
                throw new FormatException($"Parameter '{paramName}' could not be parsed as a {typeof(T).Name}.");
            }

            return parsed;
        }

        public delegate bool ParseDelegate<T>(string strVal, out T parsed);
    }
}