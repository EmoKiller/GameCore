using System;

namespace Game.Character.Core.Stats
{
    /// <summary>
    /// Defines a read-only view of a resource with current and maximum values.
    /// Xác định một cái nhìn chỉ đọc về một tài nguyên với giá trị hiện tại và tối đa.
    /// </summary>
    public interface IReadOnlyResource
    {
        /// <summary>
        /// Gets the current value of the resource.
        /// Lấy giá trị hiện tại của tài nguyên.
        /// </summary>
        float Current { get; }

        /// <summary>
        /// Gets the maximum value of the resource.
        /// Lấy giá trị tối đa của tài nguyên.
        /// </summary>
        float Max { get; }

        /// <summary>
        /// Event triggered when the resource value changes, providing the current and max values.
        /// Sự kiện được kích hoạt khi giá trị tài nguyên thay đổi, cung cấp giá trị hiện tại và tối đa.
        /// </summary>
        event Action<float, float> OnValueChanged;

        /// <summary>
        /// Event triggered when the resource reaches zero.
        /// Sự kiện được kích hoạt khi tài nguyên đạt đến số không.
        /// </summary>
        event Action OnEmpty;

    }
    /// <summary>
    /// Base class for all character resources such as Health or Stamina.
    /// Lớp cơ sở cho tất cả các tài nguyên của nhân vật như Sức khỏe hoặc Thể lực.
    /// Handles current value, max stat reference, consumption, restoration,
    /// Xử lý giá trị hiện tại, tham chiếu chỉ số tối đa, mức tiêu thụ, khôi phục.
    /// and value change events.
    /// và các sự kiện thay đổi giá trị.
    /// </summary>
    public sealed class Resource : IReadOnlyResource, IDisposable
    {
        private readonly StatValue _max;
        private readonly IResourcePolicy _policy;

        private float _current;
        private int _version;

        public int Version => _version;
        public float Current => _current;
        public float Max => _max.FinalValue;

        public event Action<float, float> OnValueChanged;
        public event Action OnEmpty;
        public event Action<float> OnConsumed;

        public Resource(StatValue max, IResourcePolicy policy)
        {
            _max = max ?? throw new ArgumentNullException(nameof(max));
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));

            _current = max.FinalValue;

            _max.OnValueChanged += HandleMaxChanged;
        }

        public void Consume(float amount)
        {
            if (amount <= 0f) return;

            SetCurrent(_current - amount);
            OnConsumed?.Invoke(amount);
        }

        public void Restore(float amount)
        {
            if (amount <= 0f) return;

            SetCurrent(_current + amount);
        }

        public void SetCurrent(float value)
        {
            float old = _current;

            float newValue = _policy.Clamp(value, _max.FinalValue);

            if (Math.Abs(old - newValue) < 0.0001f)
                return;

            _current = newValue;
            _version++;

            OnValueChanged?.Invoke(_current, _max.FinalValue);

            if (_policy.IsDepleted(_current))
            {
                OnEmpty?.Invoke();
            }
        }

        private void HandleMaxChanged(float oldMax, float newMax)
        {
            SetCurrent(_current);
        }

        public void Dispose()
        {
            _max.OnValueChanged -= HandleMaxChanged;
        }
    }
}