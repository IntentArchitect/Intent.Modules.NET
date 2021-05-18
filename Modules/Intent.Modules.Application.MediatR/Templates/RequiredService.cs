using System;

namespace Intent.Modules.Application.MediatR.Templates
{
    public class RequiredService : IEquatable<RequiredService>
    {
        public RequiredService(string type, string name)
        {
            Type = type;
            Name = name;
        }
        public string Type { get; }
        public string Name { get; }
        public string FieldName => $"_{Name}";

        public bool Equals(RequiredService other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RequiredService) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Name);
        }
    }
}