namespace Audio.Core.Data
{
    using System;

    public readonly struct AudioId : IEquatable<AudioId>
    {
        public readonly string Value;

        public AudioId(string value)
        {
            Value = value;
        }

        public bool Equals(AudioId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is AudioId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;
    }
}