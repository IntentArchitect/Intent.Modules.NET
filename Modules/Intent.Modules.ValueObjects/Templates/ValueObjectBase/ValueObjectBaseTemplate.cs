using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate", Version= "1.0")]

namespace Intent.Modules.ValueObjects.Templates.ValueObjectBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ValueObjectBaseTemplate
    {

        public bool UsingEF
        {
            get
            {
                return this.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.EntityFrameworkCore");
            }
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var sameTypeImplementation = UsingEF ? GetEFSameType() : GetDefaultSameType();
            var privateSameTypeImplementation = UsingEF ? GetPrivateEFSameType() : string.Empty;
            return $@"
using System.Collections.Generic;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace {Namespace}
{{
    /// <summary>
    /// Value Object implementation based on Microsoft's documentation:
    /// <see href=""https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/implement-value-objects#value-object-implementation-in-c""/>.
    /// </summary>
    public abstract class ValueObject
    {{
        public static bool operator ==(ValueObject one, ValueObject two)
        {{
            return EqualOperator(one, two);
        }}

        public static bool operator !=(ValueObject one, ValueObject two)
        {{
            return NotEqualOperator(one, two);
        }}

        public override bool Equals(object? obj)
        {{
            if (obj == null || !AreSameType(obj, this))
            {{
                return false;
            }}

            var other = (ValueObject)obj;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }}

        public override int GetHashCode()
        {{
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }}

        {sameTypeImplementation}

        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {{
            if (left is null ^ right is null)
            {{
                return false;
            }}

            return ReferenceEquals(left, right) || left!.Equals(right);
        }}

        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {{
            return !EqualOperator(left, right);
        }}

        protected abstract IEnumerable<object?> GetEqualityComponents();

        {privateSameTypeImplementation}
    }}
}}";
        }

        private string GetEFSameType()
        {
            this.AddUsing("System");
            return @"public static bool AreSameType(object obj1, object obj2)
        {
            if (obj1 == null || obj2 == null)
            {
                return false;
            }

            var type1 = obj1.GetType();
            var type2 = obj2.GetType();

            if (IsEFProxy(type1))
            {
                type1 = type1.BaseType!;
            }

            if (IsEFProxy(type2))
            {
                type2 = type2.BaseType!;
            }

            return type1 == type2;
        }";

        }

        private string GetPrivateEFSameType()
        {
            return @" private static bool IsEFProxy(Type type)
            {
                return type.Namespace == ""Castle.Proxies"";
            }";
        }

        private string GetDefaultSameType()
        {
            return @"public static bool AreSameType(object obj1, object obj2)
        {
            if (obj1 == null || obj2 == null)
            {
                return false;
            }

            var type1 = obj1.GetType();
            var type2 = obj2.GetType();

            return type1 == type2;
        }";

        }

    }
}