using System;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;

public record RequiredEntityProperty(ClassModel Class, string Name, ICanBeReferencedType Type, bool IsNullable = false, bool IsCollection = false, Action<CSharpProperty> ConfigureProperty = null);