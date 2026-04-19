using System;

namespace Game.Character.Core.Stats
{
    /// <summary>
    /// Base class for all character resources such as Health or Stamina.
    /// Lớp cơ sở cho tất cả các tài nguyên của nhân vật như Sức khỏe hoặc Thể lực.
    /// Handles current value, max stat reference, consumption, restoration,
    /// Xử lý giá trị hiện tại, tham chiếu chỉ số tối đa, mức tiêu thụ, khôi phục.
    /// and value change events.
    /// và các sự kiện thay đổi giá trị.
    /// </summary>
    public abstract class Resource : IReadOnlyResource
    {
        protected readonly StatValue _max;
        protected float _current;

        /// <summary>
        /// Triggered when the resource value changes.
        /// Được kích hoạt khi giá trị tài nguyên thay đổi.
        /// (current, max)
        /// </summary>
        public event Action<float, float> OnValueChanged;

        /// <summary>
        /// Triggered when the resource reaches zero.
        /// Được kích hoạt khi tài nguyên đạt đến số không.
        /// </summary>
        public event Action OnEmpty;

        /// <summary>
        /// Gets the current resource value.
        /// Lấy giá trị tài nguyên hiện tại.
        /// </summary>
        public float Current => _current;

        /// <summary>
        /// Gets the maximum resource value.
        /// Lấy giá trị tài nguyên tối đa.
        /// </summary>
        public float Max => _max.FinalValue;

        /// <summary>
        /// Initializes a new resource using the provided max stat.
        /// Khởi tạo một tài nguyên mới sử dụng chỉ số tối đa được cung cấp.
        /// </summary>
        protected Resource(StatValue maxStat)
        {
            _max = maxStat ?? throw new ArgumentNullException(nameof(maxStat));

            _current = _max.FinalValue;

            _max.OnValueChanged += HandleMaxChanged;
        }

        /// <summary>
        /// Consumes a specified amount of the resource.
        /// Tiêu thụ một lượng nhất định của tài nguyên.
        /// </summary>
        public void Consume(float amount)
        {
            if (amount <= 0f)
                return;

            SetCurrent(_current - amount);
        }

        /// <summary>
        /// Restores a specified amount of the resource.
        /// Khôi phục một lượng nhất định của tài nguyên.
        /// </summary>
        public void Restore(float amount)
        {
            if (amount <= 0f)
                return;

            SetCurrent(_current + amount);
        }

        /// <summary>
        /// Sets the current resource value and clamps it within valid bounds.
        /// Thiết lập giá trị tài nguyên hiện tại và giới hạn nó trong phạm vi hợp lệ.
        /// </summary>
        public void SetCurrent(float value)
        {
            float oldValue = _current;

            float max = _max.FinalValue;

            _current = Math.Clamp(value, 0f, max);

            if (Math.Abs(oldValue - _current) > 0.0001f)
            {
                OnValueChanged?.Invoke(_current, max);

                if (_current <= 0f)
                {
                    OnEmpty?.Invoke();
                }
            }
        }

        /// <summary>
        /// Handles max stat changes and clamps the current value if necessary.
        /// Xử lý các thay đổi của chỉ số tối đa và giới hạn giá trị hiện tại nếu cần thiết.
        /// </summary>
        private void HandleMaxChanged(float oldValue, float newValue)
        {
            float oldCurrent = _current;

            if (_current > newValue)
            {
                _current = newValue;
            }

            if (Math.Abs(oldCurrent - _current) > 0.0001f)
            {
                OnValueChanged?.Invoke(_current, newValue);
            }
        }
    }
}