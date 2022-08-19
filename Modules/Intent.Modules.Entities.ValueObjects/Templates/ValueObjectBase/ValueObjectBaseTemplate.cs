using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]
namespace Intent.Modules.Entities.ValueObjects.Templates.ValueObjectBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ValueObjectBaseTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $@"
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace {Namespace}
{{
    public abstract class {ClassName}
    {{
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {{
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {{
                return false;
            }}
            return ReferenceEquals(left, right) || left.Equals(right);
        }}

        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {{
            return !(EqualOperator(left, right));
        }}

        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj)
        {{
            if (obj == null || obj.GetType() != GetType())
            {{
                return false;
            }}

            var other = (ValueObject)obj;

            return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }}

        public override int GetHashCode()
        {{
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }}

        public static bool operator ==(ValueObject one, ValueObject two)
        {{
            return EqualOperator(one, two);
        }}

        public static bool operator !=(ValueObject one, ValueObject two)
        {{
            return NotEqualOperator(one, two);
        }}
    }}
}}";
        }

        private IEnumerable<string> GetMembers()
        {
            var members = new List<string>();

            // example: adding a constructor
            members.Add($@"
        public {ClassName}()
        {{
        }}");
            return members;
        }
    }
}