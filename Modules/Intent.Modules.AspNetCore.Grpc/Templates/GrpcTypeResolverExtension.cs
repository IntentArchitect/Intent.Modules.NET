using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.AspNetCore.Grpc.Templates.CommonTypesPartial;
using Intent.Modules.AspNetCore.Grpc.Templates.CommonTypesProtoFile;
using Intent.Modules.AspNetCore.Grpc.Templates.MessageProtoFile;
using Intent.Modules.AspNetCore.Grpc.Templates.PagedResultProtoFile;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.AspNetCore.Grpc.Templates
{
    public interface IGrpcProtoTemplate : IIntentTemplate
    {
        SortedSet<string> Imports { get; }
        string[] PackageParts { get; }
    }

    public interface IGrpcProtoTemplate<out TModel> : IIntentTemplate<TModel>, IGrpcProtoTemplate;

    internal static class GrpcTypeResolverHelper
    {
        public enum MappingType { FromMessage, ToMessage, GetOperationReturnType, ReturnStatement }

        public static string MapFromMessage<TModel>(
            this CSharpTemplateBase<TModel> template,
            ITypeReference typeReference,
            string sourceExpression,
            bool? isNullable = null,
            bool? isCollection = null)
        {
            return MapForMessage(template, MappingType.FromMessage, typeReference, sourceExpression, isNullable, isCollection).Item1;
        }

        public static string MapToOperationReturnType<TModel>(
            this CSharpTemplateBase<TModel> template,
            ITypeReference typeReference)
        {
            return MapForMessage(template, MappingType.GetOperationReturnType, typeReference, sourceExpression: null).Item1;
        }

        public static string MapToReturnStatement<TModel>(
            this CSharpTemplateBase<TModel> template,
            ITypeReference typeReference,
            string sourceExpression)
        {
            return MapForMessage(template, MappingType.ReturnStatement, typeReference, sourceExpression).Item1;
        }

        public static (string, bool IsForMethodCall) MapForMessage<TModel>(
            this CSharpTemplateBase<TModel> template,
            MappingType type,
            ITypeReference typeReference,
            string sourceExpression,
            bool? isNullable = null,
            bool? isCollection = null)
        {
            //var nullable = typeReference.IsNullable ? "?" : string.Empty;

            isNullable ??= typeReference.IsNullable;
            isCollection ??= typeReference.IsCollection;

            var commonTemplate = GetCommonProtoTemplateInstance(template);

            // void
            if (typeReference.Element == null)
            {
                return type switch
                {
                    MappingType.FromMessage => throw new InvalidOperationException(),
                    MappingType.ToMessage => ($"new {UseType("Google.Protobuf.WellKnownTypes.Empty")}()", false),
                    MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Empty"), false),
                    MappingType.ReturnStatement => ($"new {UseType("Google.Protobuf.WellKnownTypes.Empty")}()", false),
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };
            }

            if (typeReference.HasBinaryType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.ToByteArray()", false),
                            MappingType.ToMessage => ($"{UseType("Google.Protobuf.ByteString")}.CopyFrom({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.BytesValue"), false),
                            MappingType.ReturnStatement => ($"new {UseType("Google.Protobuf.WellKnownTypes.BytesValue")} {{ Value = {UseType("Google.Protobuf.ByteString")}.CopyFrom({sourceExpression}) }}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseSelect()}(x => x.ToByteArray()).{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}.{UseSelect()}({UseType("Google.Protobuf.ByteString")}.CopyFrom))", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfBytes"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfBytes")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.ToByteArray()", false),
                            MappingType.ToMessage => ($"{sourceExpression} != null ? {UseType("Google.Protobuf.ByteString")}.CopyFrom({sourceExpression}) : null", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.BytesValue?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression} != null ? new {UseType("Google.Protobuf.WellKnownTypes.BytesValue")} {{ Value = {UseType("Google.Protobuf.ByteString")}.CopyFrom({sourceExpression}) }} : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseSelect()}(x => x.ToByteArray()).{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByString("Bytes")}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfBytes?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfBytes")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasBoolType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => ($"{UseType("Google.Protobuf.WellKnownTypes.BoolValue")}", false),
                            MappingType.ReturnStatement => ($"new {UseType("Google.Protobuf.WellKnownTypes.BoolValue")} {{ Value = {sourceExpression} }}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression})", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfBool"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfBool")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => ($"{UseType("Google.Protobuf.WellKnownTypes.BoolValue")}?", false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? new {UseType("Google.Protobuf.WellKnownTypes.BoolValue")} {{ Value = {sourceExpression}.Value }} : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByTypeReference(typeReference)}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfBool?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfBool")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasByteType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"(byte){sourceExpression}", false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.UInt32Value"), false),
                            MappingType.ReturnStatement => ($"new {UseType("Google.Protobuf.WellKnownTypes.UInt32Value")} {{ Value = {sourceExpression} }}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseSelect()}(x => (byte)x).{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}.{UseSelect()}(x => (uint)x))", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfUInt32"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfUInt32")}.Create({sourceExpression}.{UseSelect()}(x => (uint)x))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"(byte?){sourceExpression}", false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.UInt32Value?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? new {UseType("Google.Protobuf.WellKnownTypes.UInt32Value")} {{ Value = {sourceExpression}.Value }} : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseSelect()}(x => (byte)x).{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByString("UInt32")}.Create({sourceExpression}?.{UseSelect()}(x => (uint)x))", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfUInt32?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfUInt32")}.Create({sourceExpression}?.{UseSelect()}(x => (uint)x))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasCharType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}[0]", false),
                            MappingType.ToMessage => ($"char.ToString({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.StringValue"), false),
                            MappingType.ReturnStatement => ($"new {UseType($"Google.Protobuf.WellKnownTypes.StringValue")} {{ Value = char.ToString({sourceExpression}) }}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseToList()}()", false),
                            MappingType.ToMessage => ($"new string({sourceExpression}.ToArray())", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.StringValue"), false),
                            MappingType.ReturnStatement => ($"new {UseType($"Google.Protobuf.WellKnownTypes.StringValue")} {{ Value = new string({sourceExpression}.ToArray()) }}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?[0]", false),
                            MappingType.ToMessage => ($"{sourceExpression} != null ? char.ToString({sourceExpression}.Value) : null", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.StringValue?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? new {UseType("Google.Protobuf.WellKnownTypes.StringValue")} {{ Value = char.ToString({sourceExpression}.Value) }} : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.{UseToList()}()", false),
                            MappingType.ToMessage => ($"{sourceExpression} != null ? new string({sourceExpression}.ToArray()) : null", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.StringValue?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression} != null ? new {UseType("Google.Protobuf.WellKnownTypes.StringValue")} {{ Value = new string({sourceExpression}.ToArray()) }} : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasDateType())
            {
                string UseDateOnly() => UseType("System.DateOnly");
                string Midnight() => $"{UseType("System.TimeOnly")}.MinValue";

                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{UseDateOnly()}.FromDateTime({sourceExpression}.ToDateTime())", false),
                            MappingType.ToMessage => ($"{UseTimestamp()}.FromDateTime({sourceExpression}.ToDateTime({Midnight()}))", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Timestamp"), false),
                            MappingType.ReturnStatement => ($"{UseTimestamp()}.FromDateTime({sourceExpression}.ToDateTime({Midnight()}))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseSelect()}(x => {UseDateOnly()}.FromDateTime(x.ToDateTime())).{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}.{UseSelect()}(x => {UseTimestamp()}.FromDateTime(x.ToDateTime({Midnight()}))))", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfTimestamp"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfTimestamp")}.Create({sourceExpression}.{UseSelect()}(x => {UseTimestamp()}.FromDateTime(x.ToDateTime({Midnight()}))))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression} != null ? {UseDateOnly()}.FromDateTime({sourceExpression}.ToDateTime()) : null", false),
                            MappingType.ToMessage => ($"{sourceExpression} != null ? {UseTimestamp()}.FromDateTime({sourceExpression}.Value.ToDateTime({Midnight()})) : null", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Timestamp?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? {UseTimestamp()}.FromDateTime({sourceExpression}.Value.ToDateTime({Midnight()})) : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseSelect()}(x => {UseDateOnly()}.FromDateTime(x.ToDateTime())).{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByString("Timestamp")}.Create({sourceExpression}?.{UseSelect()}(x => {UseTimestamp()}.FromDateTime(x.ToDateTime({Midnight()}))))", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfTimestamp?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfTimestamp")}.Create({sourceExpression}?.{UseSelect()}(x => {UseTimestamp()}.FromDateTime(x.ToDateTime({Midnight()}))))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasDateTimeOffsetType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.ToDateTimeOffset()", false),
                            MappingType.ToMessage => ($"{UseTimestamp()}.FromDateTimeOffset({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Timestamp"), false),
                            MappingType.ReturnStatement => ($"{UseTimestamp()}.FromDateTimeOffset({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseSelect()}(x => x.ToDateTimeOffset()).{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}.{UseSelect()}({UseTimestamp()}.FromDateTimeOffset))", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfTimestamp"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfTimestamp")}.Create({sourceExpression}.{UseSelect()}({UseTimestamp()}.FromDateTimeOffset))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression} != null ? {sourceExpression}.ToDateTimeOffset() : null", false),
                            MappingType.ToMessage => ($"{sourceExpression} != null ? {UseTimestamp()}.FromDateTimeOffset({sourceExpression}.Value) : null", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Timestamp?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? {UseTimestamp()}.FromDateTimeOffset({sourceExpression}.Value) : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseSelect()}(x => x.ToDateTimeOffset()).{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByString("Timestamp")}.Create({sourceExpression}?.{UseSelect()}({UseTimestamp()}.FromDateTimeOffset))", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfTimestamp?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfTimestamp")}.Create({sourceExpression}?.{UseSelect()}({UseTimestamp()}.FromDateTimeOffset))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasDateTimeType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.ToDateTime()", false),
                            MappingType.ToMessage => ($"{UseTimestamp()}.FromDateTime({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Timestamp"), false),
                            MappingType.ReturnStatement => ($"{UseTimestamp()}.FromDateTime({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseSelect()}(x => x.ToDateTime()).{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}.{UseSelect()}({UseTimestamp()}.FromDateTime))", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfTimestamp"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfTimestamp")}.Create({sourceExpression}.{UseSelect()}({UseTimestamp()}.FromDateTime))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression} != null ? {sourceExpression}.ToDateTime() : null", false),
                            MappingType.ToMessage => ($"{sourceExpression} != null ? {UseTimestamp()}.FromDateTime({sourceExpression}.Value) : null", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Timestamp?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? {UseTimestamp()}.FromDateTime({sourceExpression}.Value) : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseSelect()}(x => x.ToDateTime()).{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByString("Timestamp")}.Create({sourceExpression}?.{UseSelect()}({UseTimestamp()}.FromDateTime))", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfTimestamp?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfTimestamp")}.Create({sourceExpression}?.{UseSelect()}({UseTimestamp()}.FromDateTime))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasDecimalType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.DecimalValue"), false),
                            MappingType.ReturnStatement => (sourceExpression, false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseSelect()}(x => (decimal)x).{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}?.{UseSelect()}(x => (DecimalValue)x))", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfDecimalValue"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfDecimalValue")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.DecimalValue?"), false),
                            MappingType.ReturnStatement => (sourceExpression, false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseSelect()}(x => (decimal)x).{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByString("DecimalValue")}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfDecimalValue?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfDecimalValue")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasDictionaryType())
            {
                var keyType = typeReference.GenericTypeParameters.First();
                var valueType = typeReference.GenericTypeParameters.Skip(1).First();
                var mapOfName = $"MapOf{keyType.GetClosedGenericTypeName()}And{valueType.GetClosedGenericTypeName()}";
                template.AddUsing(commonTemplate.CSharpNamespace);

                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.ToDictionary()", false),
                            MappingType.ToMessage => ($".Add({sourceExpression})", true),
                            MappingType.GetOperationReturnType => ($"{mapOfName}", false),
                            MappingType.ReturnStatement => ($"{mapOfName}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseSelect()}(x => x.Items.ToDictionary()).{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}.{UseSelect()}({UseType($"{commonTemplate.CSharpNamespace}.{mapOfName}")}.Create))", true),
                            MappingType.GetOperationReturnType => ($"ListOf{mapOfName}", false),
                            MappingType.ReturnStatement => ($"ListOf{mapOfName}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.ToDictionary()", false),
                            MappingType.ToMessage => ($"{UseType($"{commonTemplate.CSharpNamespace}.{mapOfName}")}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => ($"{mapOfName}?", false),
                            MappingType.ReturnStatement => ($"{mapOfName}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseSelect()}(x => x.Items.ToDictionary()).{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOf{mapOfName}")}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => ($"ListOf{mapOfName}?", false),
                            MappingType.ReturnStatement => ($"ListOf{mapOfName}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasDoubleType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.DoubleValue"), false),
                            MappingType.ReturnStatement => ($"new {UseType("Google.Protobuf.WellKnownTypes.DoubleValue")} {{ Value = {sourceExpression} }}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression})", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfDouble"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfDouble")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => ($"{UseType("Google.Protobuf.WellKnownTypes.DoubleValue")}?", false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? new {UseType("Google.Protobuf.WellKnownTypes.DoubleValue")} {{ Value = {sourceExpression}.Value }} : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByTypeReference(typeReference)}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfDouble?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfDouble")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.Element.IsEnumModel())
            {
                string AppType() => template.GetTypeName((IElement)typeReference.Element);

                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"({AppType()}){sourceExpression}", false),
                            MappingType.ToMessage => ($"(int){sourceExpression}", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Int32Value"), false),
                            MappingType.ReturnStatement => ($"new {UseType("Google.Protobuf.WellKnownTypes.Int32Value")} {{ Value = (int){sourceExpression} }}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseCast()}<{AppType()}>().{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}.{UseCast()}<int>())", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfInt32"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfInt32")}.Create({sourceExpression}.{UseSelect()}(x => (int)x))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"({AppType()}?){sourceExpression}", false),
                            MappingType.ToMessage => ($"(int?){sourceExpression}", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Int32Value?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? new {UseType("Google.Protobuf.WellKnownTypes.Int32Value")} {{ Value = (int){sourceExpression} }} : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseCast()}<{AppType()}>().{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByString("Int32")}.Create({sourceExpression}?.{UseCast()}<int>())", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfInt32?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfInt32")}.Create({sourceExpression}?.{UseSelect()}(x => (int)x))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasFloatType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.FloatValue"), false),
                            MappingType.ReturnStatement => ($"new {UseType("Google.Protobuf.WellKnownTypes.FloatValue")} {{ Value = {sourceExpression} }}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression})", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfFloat"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfFloat")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => ($"{UseType("Google.Protobuf.WellKnownTypes.FloatValue")}?", false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? new {UseType("Google.Protobuf.WellKnownTypes.FloatValue")} {{ Value = {sourceExpression}.Value }} : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByTypeReference(typeReference)}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfFloat?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfFloat")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasGuidType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{UseType("System.Guid")}.Parse({sourceExpression})", false),
                            MappingType.ToMessage => ($"{sourceExpression}.ToString()", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.StringValue"), false),
                            MappingType.ReturnStatement => ($"new {UseType("Google.Protobuf.WellKnownTypes.StringValue")} {{ Value = {sourceExpression}.ToString() }}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseSelect()}({UseType("System.Guid")}.Parse).{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}.{UseSelect()}(x => x.ToString()))", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfString"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfString")}.Create({sourceExpression}.Select(x => x.ToString()))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression} != null ? {UseType("System.Guid")}.Parse({sourceExpression}) : null", false),
                            MappingType.ToMessage => ($"{sourceExpression}?.ToString()", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.StringValue?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? new {UseType("Google.Protobuf.WellKnownTypes.StringValue")} {{ Value = {sourceExpression}.Value.ToString() }} : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseSelect()}({UseType("System.Guid")}.Parse).{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByString("String")}.Create({sourceExpression}?.{UseSelect()}(x => x.ToString()))", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfString?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfString")}.Create({sourceExpression}?.Select(x => x.ToString()))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasIntType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Int32Value"), false),
                            MappingType.ReturnStatement => ($"new {UseType("Google.Protobuf.WellKnownTypes.Int32Value")} {{ Value = {sourceExpression} }}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression})", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfInt32"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfInt32")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Int32Value?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? new {UseType("Google.Protobuf.WellKnownTypes.Int32Value")} {{ Value = {sourceExpression}.Value }} : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByString("Int32")}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfInt32?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfInt32")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasLongType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Int64Value"), false),
                            MappingType.ReturnStatement => ($"new {UseType("Google.Protobuf.WellKnownTypes.Int64Value")} {{ Value = {sourceExpression} }}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression})", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfInt64"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfInt64")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Int64Value?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? new {UseType("Google.Protobuf.WellKnownTypes.Int64Value")} {{ Value = {sourceExpression}.Value }} : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByString("Int64")}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfInt64?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfInt64")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasObjectType())
            {
                var comment = "/* Please either use a type other than \"object\" in the Intent Architect designer or manually update this code */";

                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => ($"{UseType("Google.Protobuf.WellKnownTypes.Any")}.Pack(({UseType("Google.Protobuf.IMessage")}){sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Any"), false),
                            MappingType.ReturnStatement => ($"(Any)result {comment}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseCast()}<object>().{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}.{UseSelect()}(x => {UseType("Google.Protobuf.WellKnownTypes.Any")}.Pack(({UseType("Google.Protobuf.IMessage")})x)))", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfAny"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfAny")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => ($"{sourceExpression} != null ? {UseType("Google.Protobuf.WellKnownTypes.Any")}.Pack(({UseType("Google.Protobuf.IMessage")}){sourceExpression}) : null", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Any?"), false),
                            MappingType.ReturnStatement => ($"(Any?)result {comment}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseCast()}<object>().{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByString("Any")}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfAny?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfAny")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (string.Equals(typeReference.Element?.Id, MetadataIds.PagedResultTypeId))
            {
                var name = typeReference.GetClosedGenericTypeName();
                var pagedResultTemplate = GetPagedResultProtoTemplateInstance(template);
                string TypeName() => template.ResolvePagedResultOfName(typeReference);

                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.ToContract()", false),
                            MappingType.ToMessage => ($"{TypeName()}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{pagedResultTemplate.CSharpNamespace}.{name}"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{pagedResultTemplate.CSharpNamespace}.{name}")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseSelect()}(x => x.ToContract()).{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}.{UseSelect()}({TypeName()}.Create))", true),
                            MappingType.GetOperationReturnType => (UseType($"{pagedResultTemplate.CSharpNamespace}.ListOf{name}"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{pagedResultTemplate.CSharpNamespace}.ListOf{name}")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.ToContract()", false),
                            MappingType.ToMessage => ($"{TypeName()}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{pagedResultTemplate.CSharpNamespace}.{name}?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{pagedResultTemplate.CSharpNamespace}.{name}")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseSelect()}(x => x.ToContract()).{UseToList()}()", false),
                            MappingType.ToMessage => ($"ListOf{TypeName()}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{pagedResultTemplate.CSharpNamespace}.ListOf{name}?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{pagedResultTemplate.CSharpNamespace}.ListOf{name}")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasShortType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"(short){sourceExpression}", false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Int32Value"), false),
                            MappingType.ReturnStatement => ($"new {UseType("Google.Protobuf.WellKnownTypes.Int32Value")} {{ Value = {sourceExpression} }}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseSelect()}(x => (short)x).{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}.{UseSelect()}(x => (int)x))", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfInt32"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfInt32")}.Create({sourceExpression}.{UseSelect()}(x => (int)x))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"(short?){sourceExpression}", false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Int32Value?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? new {UseType("Google.Protobuf.WellKnownTypes.Int32Value")} {{ Value = {sourceExpression}.Value }} : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseSelect()}(x => (short)x).{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByString("Int32")}.Create({sourceExpression}?.{UseSelect()}(x => (int)x))", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfInt32?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfInt32")}.Create({sourceExpression}?.{UseSelect()}(x => (int)x))", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasStringType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.StringValue"), false),
                            MappingType.ReturnStatement => ($"new {UseType("Google.Protobuf.WellKnownTypes.StringValue")} {{ Value = {sourceExpression} }}", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression})", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfString"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfString")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => (sourceExpression, false),
                            MappingType.ToMessage => (sourceExpression, false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.StringValue?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression} != null ? new {UseType("Google.Protobuf.WellKnownTypes.StringValue")} {{ Value = {sourceExpression} }} : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByTypeReference(typeReference)}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfString?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfString")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.HasTimeSpanType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.ToTimeSpan()", false),
                            MappingType.ToMessage => ($"{UseType("Google.Protobuf.WellKnownTypes.Duration")}.FromTimeSpan({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Duration"), false),
                            MappingType.ReturnStatement => ($"{UseType($"Google.Protobuf.WellKnownTypes.Duration")}.FromTimeSpan({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseSelect()}(x => x.ToTimeSpan()).{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}.{UseSelect()}({UseType("Google.Protobuf.WellKnownTypes.Duration")}.FromTimeSpan))", true),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfDuration"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfDuration")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.ToTimeSpan()", false),
                            MappingType.ToMessage => ($"{sourceExpression} != null ? {UseType("Google.Protobuf.WellKnownTypes.Duration")}.FromTimeSpan({sourceExpression}.Value) : null", false),
                            MappingType.GetOperationReturnType => (UseType("Google.Protobuf.WellKnownTypes.Duration?"), false),
                            MappingType.ReturnStatement => ($"{sourceExpression}.HasValue ? {UseType($"Google.Protobuf.WellKnownTypes.Duration")}.FromTimeSpan({sourceExpression}.Value) : null", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseSelect()}(x => x.ToTimeSpan()).{UseToList()}()", false),
                            MappingType.ToMessage => ($"{UseListOfNameByString("Duration")}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{commonTemplate.CSharpNamespace}.ListOfDuration?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{commonTemplate.CSharpNamespace}.ListOfDuration")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            if (typeReference.Element?.SpecializationTypeId is MetadataIds.DtoTypeId or MetadataIds.CommandElementTypeId or MetadataIds.QueryElementTypeId)
            {
                var typeName = typeReference.GetClosedGenericTypeName();
                var dtoTemplate = GetTemplateInstance<MessageProtoFileTemplate>(template, MessageProtoFileTemplate.TemplateId, typeReference.Element!.Id);
                string TypeName() => template.ResolveDtoTypeName(typeReference);

                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.ToContract()", false),
                            MappingType.ToMessage => ($"{TypeName()}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{dtoTemplate.CSharpNamespace}.{typeName}"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{dtoTemplate.CSharpNamespace}.{typeName}")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: false, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}.{UseSelect()}(x => x.ToContract()).{UseToList()}()", false),
                            MappingType.ToMessage => ($".AddRange({sourceExpression}.{UseSelect()}({TypeName()}.Create))", true),
                            MappingType.GetOperationReturnType => (UseType($"{dtoTemplate.CSharpNamespace}.ListOf{typeName}"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{dtoTemplate.CSharpNamespace}.ListOf{typeName}")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: false):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.ToContract()", false),
                            MappingType.ToMessage => ($"{TypeName()}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{dtoTemplate.CSharpNamespace}.{typeName}?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{dtoTemplate.CSharpNamespace}.{typeName}")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                    case (isNullable: true, isCollection: true):
                        return type switch
                        {
                            MappingType.FromMessage => ($"{sourceExpression}?.Items.{UseSelect()}(x => x.ToContract()).{UseToList()}()", false),
                            MappingType.ToMessage => ($"ListOf{TypeName()}.Create({sourceExpression})", false),
                            MappingType.GetOperationReturnType => (UseType($"{dtoTemplate.CSharpNamespace}.ListOf{typeName}?"), false),
                            MappingType.ReturnStatement => ($"{UseType($"{dtoTemplate.CSharpNamespace}.ListOf{typeName}")}.Create({sourceExpression})", false),
                            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                        };
                }
            }

            throw new Exception("Unhandled type");

            string UseType(string s) => template.UseType(s);
            string UseToList() => template.UseType("System.Linq.ToList");
            string UseCast() => template.UseType("System.Linq.Cast");
            string UseSelect() => template.UseType("System.Linq.Select");
            string UseListOfNameByString(string s) => template.ResolveCommonListOfName(s);
            string UseListOfNameByTypeReference(ITypeReference t) => template.ResolveCommonListOfName(t);
            string UseTimestamp() => UseType("Google.Protobuf.WellKnownTypes.Timestamp");
        }

        public static string ResolveProtoType<TModel>(this IGrpcProtoTemplate<TModel> template, IHasTypeReference hasTypeReference) =>
            ResolveProtoType(template, hasTypeReference.TypeReference);

        public static string ResolveProtoType<TModel>(
            this IGrpcProtoTemplate<TModel> template,
            ITypeReference typeReference,
            bool? isNullable = null,
            bool? isCollection = null)
        {
            isNullable ??= typeReference.IsNullable;
            isCollection ??= typeReference.IsCollection;

            var commonTypesPartialTemplate = GetTemplateInstance<CommonTypesPartialTemplate>(template, CommonTypesPartialTemplate.TemplateId);

            // void
            if (typeReference.Element == null)
            {
                template.Imports.Add("google/protobuf/empty.proto");
                return "google.protobuf.Empty";
            }

            if (typeReference.HasBinaryType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return "bytes";
                    case (isNullable: false, isCollection: true):
                        return "repeated bytes";
                    case (isNullable: true, isCollection: false):
                        template.Imports.Add("google/protobuf/wrappers.proto");
                        return "google.protobuf.BytesValue";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "Bytes", "bytes");

                        var byteStringType = commonTypesPartialTemplate.UseType("Google.Protobuf.ByteString");
                        commonTypesPartialTemplate.AddUsing("System.Linq");

                        commonTypesPartialTemplate.AddListOf(
                            typeReference: typeReference,
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace,
                            toContractTransform: ".Select(x => x.ToByteArray())",
                            onCreateTransform: $".Select({byteStringType}.CopyFrom)");

                        return messageName;
                }
            }

            if (typeReference.HasBoolType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return "bool";
                    case (isNullable: false, isCollection: true):
                        return "repeated bool";
                    case (isNullable: true, isCollection: false):
                        template.Imports.Add("google/protobuf/wrappers.proto");
                        return "google.protobuf.BoolValue";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "Bool", "bool");

                        commonTypesPartialTemplate.AddListOf(
                            typeReference: typeReference,
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace);

                        return messageName;
                }
            }

            if (typeReference.HasByteType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return "uint32";
                    case (isNullable: false, isCollection: true):
                        return "repeated uint32";
                    case (isNullable: true, isCollection: false):
                        template.Imports.Add("google/protobuf/wrappers.proto");
                        return "google.protobuf.UInt32Value";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "UInt32", "uint32");

                        commonTypesPartialTemplate.AddListOf(
                            applicationType: "List<uint>?",
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace);

                        return messageName;
                }
            }

            if (typeReference.HasDateType() ||
                typeReference.HasDateTimeOffsetType() ||
                typeReference.HasDateTimeType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: _, isCollection: false):
                        template.Imports.Add("google/protobuf/timestamp.proto");
                        return "google.protobuf.Timestamp";
                    case (isNullable: false, isCollection: true):
                        return "repeated google.protobuf.Timestamp";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "Timestamp", "google.protobuf.Timestamp", "google/protobuf/timestamp.proto");

                        var timestampType = commonTypesPartialTemplate.UseType("Google.Protobuf.WellKnownTypes.Timestamp");

                        commonTypesPartialTemplate.AddListOf(
                            applicationType: $"List<{timestampType}>?",
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace);

                        return messageName;
                }
            }

            if (typeReference.HasCharType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: _):
                        return "string";
                    case (isNullable: true, isCollection: _):
                        template.Imports.Add("google/protobuf/wrappers.proto");
                        return "google.protobuf.StringValue";
                }
            }

            if (typeReference.HasDecimalType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: _, isCollection: false):
                        return UseCommonDecimalType(template).MessageName;
                    case (isNullable: false, isCollection: true):
                        return $"repeated {UseCommonDecimalType(template).MessageName}";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOfDecimalType(template);
                        commonTypesPartialTemplate.AddUsing("System.Linq");

                        commonTypesPartialTemplate.AddListOf(
                            applicationType: $"{commonTypesPartialTemplate.UseType("System.Collections.Generic.List")}<decimal>?",
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace,
                            toContractTransform: ".Select(x => (decimal)x)",
                            onCreateTransform: ".Select(x => (DecimalValue)x)");

                        return messageName;
                }
            }

            if (typeReference.HasDictionaryType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        var genericArguments = typeReference.GenericTypeParameters.Select(x => x.IsCollection
                                ? ResolveProtoType(template, x, isNullable: true, isCollection: true)
                                : ResolveProtoType(template, x));

                        return $"map<{string.Join(", ", genericArguments)}>";
                    case (isNullable: false, isCollection: true):
                        {
                            var (messageName, typeName) = UseCommonMapOf(template, typeReference);

                            commonTypesPartialTemplate.AddMapOf(
                                applicationType: commonTypesPartialTemplate.GetTypeName(typeReference, "{0}"),
                                messageName: typeName,
                                @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace);

                            return $"repeated {messageName}";
                        }
                    case (isNullable: true, isCollection: false):
                        {
                            var (messageName, typeName) = UseCommonMapOf(template, typeReference);

                            commonTypesPartialTemplate.AddMapOf(
                                applicationType: commonTypesPartialTemplate.GetTypeName(typeReference),
                                messageName: typeName,
                                @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace);

                            return messageName;
                        }
                    case (isNullable: true, isCollection: true):
                        {
                            var (messageName, typeName) = UseCommonListOfMapOf(template, typeReference);
                            var singularTypeName = UseCommonMapOf(template, typeReference).TypeName;
                            commonTypesPartialTemplate.AddUsing("System.Linq");

                            commonTypesPartialTemplate.AddListOf(
                                typeReference: typeReference,
                                typeName: typeName,
                                @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace,
                                toContractTransform: ".Select(x => x.Items.ToDictionary())",
                                onCreateTransform: $".Select({singularTypeName}.Create)");

                            return messageName;
                        }

                }
            }

            if (typeReference.HasDoubleType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return "double";
                    case (isNullable: false, isCollection: true):
                        return "repeated double";
                    case (isNullable: true, isCollection: false):
                        template.Imports.Add("google/protobuf/wrappers.proto");
                        return "google.protobuf.DoubleValue";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "Double", "double");

                        commonTypesPartialTemplate.AddListOf(
                            typeReference: typeReference,
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace);

                        return messageName;
                }
            }

            if (typeReference.Element.IsEnumModel())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return "int32";
                    case (isNullable: false, isCollection: true):
                        return "repeated int32";
                    case (isNullable: true, isCollection: false):
                        template.Imports.Add("google/protobuf/wrappers.proto");
                        return "google.protobuf.Int32Value";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "Int32", "int32");

                        commonTypesPartialTemplate.AddListOf(
                            applicationType: "List<int>?",
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace);

                        return messageName;
                }
            }

            if (typeReference.HasFloatType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return "float";
                    case (isNullable: false, isCollection: true):
                        return "repeated float";
                    case (isNullable: true, isCollection: false):
                        template.Imports.Add("google/protobuf/wrappers.proto");
                        return "google.protobuf.FloatValue";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "Float", "float");

                        commonTypesPartialTemplate.AddListOf(
                            typeReference: typeReference,
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace);

                        return messageName;
                }
            }

            if (typeReference.HasGuidType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return "string";
                    case (isNullable: false, isCollection: true):
                        return "repeated string";
                    case (isNullable: true, isCollection: false):
                        template.Imports.Add("google/protobuf/wrappers.proto");
                        return "google.protobuf.StringValue";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "String", "string");

                        commonTypesPartialTemplate.AddListOf(
                            applicationType: "List<string>?",
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace);

                        return messageName;
                }
            }

            if (typeReference.HasIntType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return "int32";
                    case (isNullable: false, isCollection: true):
                        return "repeated int32";
                    case (isNullable: true, isCollection: false):
                        template.Imports.Add("google/protobuf/wrappers.proto");
                        return "google.protobuf.Int32Value";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "Int32", "int32");

                        commonTypesPartialTemplate.AddListOf(
                            typeReference: typeReference,
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace);

                        return messageName;
                }
            }

            if (typeReference.HasLongType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return "int64";
                    case (isNullable: false, isCollection: true):
                        return "repeated int64";
                    case (isNullable: true, isCollection: false):
                        template.Imports.Add("google/protobuf/wrappers.proto");
                        return "google.protobuf.Int64Value";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "Int64", "int64");

                        commonTypesPartialTemplate.AddListOf(
                            typeReference: typeReference,
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace);

                        return messageName;
                }
            }

            if (typeReference.HasObjectType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return "google.protobuf.Any";
                    case (isNullable: false, isCollection: true):
                        return "repeated google.protobuf.Any";
                    case (isNullable: true, isCollection: false):
                        template.Imports.Add("google/protobuf/any.proto");
                        return "google.protobuf.Any";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "Any", "google.protobuf.Any", "google/protobuf/any.proto");

                        var messageType = commonTypesPartialTemplate.UseType("Google.Protobuf.IMessage");
                        var anyType = commonTypesPartialTemplate.UseType("Google.Protobuf.WellKnownTypes.Any");
                        commonTypesPartialTemplate.AddUsing("System.Linq");

                        commonTypesPartialTemplate.AddListOf(
                            typeReference: typeReference,
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace,
                            toContractTransform: ".Cast<object>()",
                            onCreateTransform: $".Select(x => {anyType}.Pack(({messageType})x))");

                        return messageName;
                }
            }

            if (typeReference.HasShortType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return "int32";
                    case (isNullable: false, isCollection: true):
                        return "repeated int32";
                    case (isNullable: true, isCollection: false):
                        template.Imports.Add("google/protobuf/wrappers.proto");
                        return "google.protobuf.Int32Value";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "Int32", "int32");

                        commonTypesPartialTemplate.AddListOf(
                            applicationType: "List<int>?",
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace);

                        return messageName;
                }
            }

            if (typeReference.HasStringType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: false, isCollection: false):
                        return "string";
                    case (isNullable: false, isCollection: true):
                        return "repeated string";
                    case (isNullable: true, isCollection: false):
                        template.Imports.Add("google/protobuf/wrappers.proto");
                        return "google.protobuf.StringValue";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "String", "string");

                        commonTypesPartialTemplate.AddListOf(
                            typeReference: typeReference,
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace);

                        return messageName;
                }
            }

            if (typeReference.HasTimeSpanType())
            {
                switch (isNullable, isCollection)
                {
                    case (isNullable: _, isCollection: false):
                        template.Imports.Add("google/protobuf/duration.proto");
                        return "google.protobuf.Duration";
                    case (isNullable: false, isCollection: true):
                        return "repeated google.protobuf.Duration";
                    case (isNullable: true, isCollection: true):
                        var (messageName, typeName) = UseCommonListOf(template, "Duration", "google.protobuf.Duration", "google/protobuf/duration.proto");

                        var durationType = commonTypesPartialTemplate.UseType("Google.Protobuf.WellKnownTypes.Duration");
                        commonTypesPartialTemplate.AddUsing("System.Linq");

                        commonTypesPartialTemplate.AddListOf(
                            $"List<{commonTypesPartialTemplate.UseType("System.TimeSpan")}>?",
                            typeName: typeName,
                            @namespace: GetCommonProtoTemplateInstance(template).CSharpNamespace,
                            toContractTransform: ".Select(x => x.ToTimeSpan())",
                            onCreateTransform: $".Select({durationType}.FromTimeSpan)");

                        return messageName;
                }
            }

            if (string.Equals(typeReference.Element?.Id, MetadataIds.PagedResultTypeId))
            {
                var pagedResultTemplate = GetPagedResultProtoTemplateInstance(template);
                var qualifier = GetQualifier(importingTo: template, importingFrom: pagedResultTemplate);

                switch (isNullable, isCollection)
                {
                    case (isNullable: _, isCollection: false):
                        return $"{qualifier}{pagedResultTemplate.Add(typeReference)}";
                    case (isNullable: false, isCollection: true):
                        return $"repeated {qualifier}{pagedResultTemplate.Add(typeReference)}";
                    case (isNullable: true, isCollection: true):
                        return $"{qualifier}{pagedResultTemplate.AddListOf(typeReference)}";
                }

                return pagedResultTemplate.Add(typeReference);
            }

            if (typeReference.Element?.SpecializationTypeId is MetadataIds.DtoTypeId or MetadataIds.CommandElementTypeId or MetadataIds.QueryElementTypeId)
            {
                var dtoTemplate = GetTemplateInstance<MessageProtoFileTemplate>(template, MessageProtoFileTemplate.TemplateId, typeReference.Element!.Id);
                var qualifier = GetQualifier(importingTo: template, importingFrom: dtoTemplate);

                switch (isNullable, isCollection)
                {
                    case (isNullable: _, isCollection: false):
                        return $"{qualifier}{dtoTemplate.Add(typeReference)}";
                    case (isNullable: false, isCollection: true):
                        return $"repeated {qualifier}{dtoTemplate.Add(typeReference)}";
                    case (isNullable: true, isCollection: true):
                        return $"{qualifier}{dtoTemplate.AddListOf(typeReference)}";
                }
            }

            throw new Exception("Unhandled type");
        }

        private static string GetQualifier(this IGrpcProtoTemplate importingTo, IGrpcProtoTemplate importingFrom)
        {
            var importedFromParts = importingFrom.PackageParts;
            var importedToParts = importingTo.PackageParts;

            var commonPartCount = importedFromParts
                .Zip(importedToParts)
                .TakeWhile(x => x.First == x.Second)
                .Count();

            var qualifier = string.Join('.', importedFromParts.Skip(commonPartCount));
            if (qualifier.Length > 0)
            {
                qualifier = $"{qualifier}.";
            }

            return qualifier;
        }

        private static (string MessageName, string TypeName) UseCommonMapOf(IGrpcProtoTemplate template, ITypeReference typeReference)
        {
            var commonTemplate = GetCommonProtoTemplateInstance(template);
            var typeName = commonTemplate.AddMapOf(typeReference);
            var qualifier = GetQualifier(importingTo: template, importingFrom: commonTemplate);

            return ($"{qualifier}{typeName}", typeName);
        }

        private static (string MessageName, string TypeName) UseCommonListOfMapOf(IGrpcProtoTemplate template, ITypeReference typeReference)
        {
            var commonTemplate = GetCommonProtoTemplateInstance(template);
            var typeName = commonTemplate.AddListOfMapOf(typeReference);
            var qualifier = GetQualifier(importingTo: template, importingFrom: commonTemplate);

            return ($"{qualifier}{typeName}", typeName);
        }

        private static (string MessageName, string TypeName) UseCommonDecimalType(IGrpcProtoTemplate template)
        {
            var commonTemplate = GetCommonProtoTemplateInstance(template);
            var typeName = commonTemplate.AddDecimalType();
            var qualifier = GetQualifier(importingTo: template, importingFrom: commonTemplate);

            return ($"{qualifier}{typeName}", typeName);
        }

        private static (string MessageName, string TypeName) UseCommonListOfDecimalType(IGrpcProtoTemplate template)
        {
            var commonTemplate = GetCommonProtoTemplateInstance(template);
            var typeName = commonTemplate.AddListOfDecimalType();
            var qualifier = GetQualifier(importingTo: template, importingFrom: commonTemplate);

            return ($"{qualifier}{typeName}", typeName);
        }

        private static (string MessageName, string TypeName) UseCommonListOf(
            IGrpcProtoTemplate template,
            string name,
            string type,
            string import = null)
        {
            var commonTemplate = GetCommonProtoTemplateInstance(template);
            var typeName = commonTemplate.AddListOf(name, type, import);
            var qualifier = GetQualifier(importingTo: template, importingFrom: commonTemplate);

            return ($"{qualifier}{typeName}", typeName);
        }

        private static CommonTypesProtoFileTemplate GetCommonProtoTemplateInstance(IIntentTemplate template) =>
            GetTemplateInstance<CommonTypesProtoFileTemplate>(template, CommonTypesProtoFileTemplate.TemplateId);

        private static PagedResultProtoFileTemplate GetPagedResultProtoTemplateInstance(IIntentTemplate template) =>
            GetTemplateInstance<PagedResultProtoFileTemplate>(template, PagedResultProtoFileTemplate.TemplateId);

        public static TTemplate GetTemplateInstance<TTemplate>(IIntentTemplate template, string templateId, string modelId = null)
            where TTemplate : class
        {
            var otherTemplate = modelId != null
                ? template.ExecutionContext.FindTemplateInstance<IIntentTemplate>(templateId, modelId)
                : template.ExecutionContext.FindTemplateInstance<IIntentTemplate>(templateId);

            if (!ReferenceEquals(template, otherTemplate) &&
                template is IGrpcProtoTemplate grpcProtoTemplate &&
                otherTemplate is IGrpcProtoTemplate)
            {
                var relativePath = otherTemplate.GetProtoRelativePathParts().Append(otherTemplate.FileMetadata.FileNameWithExtension());

                grpcProtoTemplate.Imports.Add(string.Join('/', relativePath));
            }

            return otherTemplate as TTemplate ?? throw new InvalidOperationException();
        }
    }
}
