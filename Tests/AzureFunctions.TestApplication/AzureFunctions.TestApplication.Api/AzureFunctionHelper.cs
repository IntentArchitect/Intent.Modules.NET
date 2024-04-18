using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClassHelper", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api
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

        public static IEnumerable<T> GetQueryParamCollection<T>(string paramName, IQueryCollection query, ParseDelegate<T> parse)
            where T : struct
        {
            var result = new List<T>();
            var strVal = query[paramName];
            var values = strVal.ToString().Split(",");
            foreach (var v in values)
            {
                if (string.IsNullOrEmpty(v) || !parse(v, out T parsed))
                {
                    throw new FormatException($"Parameter '{paramName}' could not be parsed as a {typeof(T).Name}.");
                }
                result.Add(parsed);
            }

            return result;
        }

        public static T GetHeadersParam<T>(string paramName, IHeaderDictionary headers, ParseDelegate<T> parse)
            where T : struct
        {
            var strVal = headers[paramName];
            if (string.IsNullOrEmpty(strVal) || !parse(strVal, out T parsed))
            {
                throw new FormatException($"Parameter '{paramName}' could not be parsed as a {typeof(T).Name}.");
            }

            return parsed;
        }

        public static T? GetHeadersParamNullable<T>(string paramName, IHeaderDictionary headers, ParseDelegate<T> parse)
            where T : struct
        {
            var strVal = headers[paramName];
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

        public static IEnumerable<T> GetHeadersParamCollection<T>(string paramName, IHeaderDictionary headers, ParseDelegate<T> parse)
            where T : struct
        {
            var result = new List<T>();
            var strVal = headers[paramName];
            var values = strVal.ToString().Split(",");
            foreach (var v in values)
            {
                if (string.IsNullOrEmpty(v) || !parse(v, out T parsed))
                {
                    throw new FormatException($"Parameter '{paramName}' could not be parsed as a {typeof(T).Name}.");
                }
                result.Add(parsed);
            }

            return result;
        }

        public static TEnum GetEnumParam<TEnum>(string paramName, string enumString)
            where TEnum : struct
        {
            if (!Enum.TryParse<TEnum>(enumString, true, out var enumValue))
            {
                throw new FormatException($"Parameter {paramName} has value of {enumString} which is not a valid literal value for Enum {typeof(TEnum).Name}");
            }

            return enumValue;
        }

        public static TEnum? GetEnumParamNullable<TEnum>(string paramName, [AllowNull] string enumString)
            where TEnum : struct
        {
            if (enumString is null)
            {
                return null;
            }

            if (!Enum.TryParse<TEnum>(enumString, true, out var enumValue))
            {
                throw new FormatException($"Parameter {paramName} has value of {enumString} which is not a valid literal value for Enum {typeof(TEnum).Name}");
            }

            return enumValue;
        }

        public delegate bool ParseDelegate<T>(string strVal, out T parsed);
    }
}