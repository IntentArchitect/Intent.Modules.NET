using System;
using Intent.Modules.Common;

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    public class ConstructorParameter : IEquatable<ConstructorParameter>
    {
        public ConstructorParameter(string type, string name)
        {
            ParameterType = type;
            ParameterName = name;
        }

        public string ParameterType { get; }
        public string ParameterName { get; }

        public bool Equals(ConstructorParameter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ParameterType == other.ParameterType && ParameterName == other.ParameterName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConstructorParameter) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ParameterType, ParameterName);
        }
    }
}