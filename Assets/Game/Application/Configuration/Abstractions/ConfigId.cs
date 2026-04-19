using System;
namespace Game.Application.Configuration.Abstractions
{
    /// <summary>
    /// Strongly typed identifier for configuration entries.
    /// </summary>
    public readonly struct ConfigId<T> : IEquatable<ConfigId<T>> where T : IConfig
    {
        public readonly int Value;

        public ConfigId(int value)
        {
            Value = value;
        }

        public bool Equals(ConfigId<T> other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is ConfigId<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public static bool operator ==(ConfigId<T> left, ConfigId<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ConfigId<T> left, ConfigId<T> right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{typeof(T).Name}:{Value}";
        }
    }
}
