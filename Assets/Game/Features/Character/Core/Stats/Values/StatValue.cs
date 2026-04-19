using System;
using System.Collections.Generic;


namespace Game.Character.Core.Stats
{
    /// <summary>
    /// Represents a stat value with support for modifiers that affect the final calculated value.
    /// Uses lazy recalculation to optimize performance.
    /// Biểu thị một giá trị thống kê với hỗ trợ các bộ điều chỉnh ảnh hưởng đến giá trị cuối cùng được tính toán.
    /// Sử dụng tính toán lại lười để tối ưu hiệu suất.
    /// </summary>
    public class StatValue : IReadOnlyStat
    {
        protected float _baseValue;
        protected float _finalValue;
        protected bool _isDirty;

        /// <summary>
        /// List of all modifiers currently applied to this stat. Modifiers are immutable and should be added/removed through the provided methods to ensure proper recalculation.
        /// Danh sách tất cả các bộ điều chỉnh hiện đang được áp dụng cho thống kê này. Các bộ điều chỉnh là không thể thay đổi và nên được thêm/bỏ thông qua các phương thức được cung cấp để đảm bảo tính toán lại đúng cách.
        /// </summary>
        protected readonly List<StatModifier> _modifiers;

        /// <summary>
        /// Gets a read-only list of all modifiers currently applied to this stat.
        /// Lấy một danh sách chỉ đọc của tất cả các bộ điều chỉnh hiện đang được áp dụng cho thống kê này.
        /// </summary>
        public IReadOnlyList<StatModifier> Modifiers => _modifiers.AsReadOnly();

        
        public event Action<float, float> OnValueChanged;
        public event Action OnModifiersChanged;

        /// <summary>
        /// Gets the base value of the stat before any modifications are applied.
        /// Lấy giá trị cơ bản của thống kê trước khi bất kỳ sửa đổi nào được áp dụng.
        /// </summary>
        public virtual float BaseValue
        {
            get { return _baseValue; }
        }

        /// <summary>
        /// Gets the final value of the stat after all modifications have been applied.
        /// The value is recalculated lazily when accessed if any modifier has changed.
        /// Lấy giá trị cuối cùng của thống kê sau khi tất cả các sửa đổi đã được áp dụng.
        /// Giá trị được tính toán lại lười khi truy cập nếu bất kỳ bộ điều chỉnh nào đã thay đổi.
        /// </summary>
        public float FinalValue
        {
            get
            {
                if (_isDirty)
                {
                    RecalculateFinalValue();
                }
                return _finalValue;
            }
        }

        private static bool Approximately(float a, float b)
        {
            return Math.Abs(a - b) < 0.0001f;
        }

        /// <summary>
        /// Initializes a new instance of the StatValue class with a base value.
        /// Khởi tạo một instance mới của lớp StatValue với một giá trị cơ bản.
        /// </summary>
        /// <param name="baseValue">The initial base value of the stat.</param>
        public StatValue(float baseValue)
        {
            _baseValue = baseValue;
            _finalValue = baseValue;
            _isDirty = false;
            _modifiers = new List<StatModifier>();
        }

        /// <summary>
        /// Adds a modifier to this stat.
        /// Thêm một bộ điều chỉnh vào thống kê này.
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when modifier is null.</exception>
        public void AddModifier(StatModifier modifier)
        {
            if (modifier == null)
            {
                throw new ArgumentNullException(nameof(modifier));
            }

            _modifiers.Add(modifier);
            _isDirty = true;

            OnModifiersChanged?.Invoke();
        }

        /// <summary>
        /// Removes a specific modifier from this stat.
        /// Loại bỏ một bộ điều chỉnh cụ thể khỏi thống kê này.
        /// </summary>
        /// <param name="modifier">The modifier to remove.</param>
        /// <returns>True if the modifier was found and removed; otherwise, false.</returns>
        public bool RemoveModifier(StatModifier modifier)
        {
            if (modifier == null)
            {
                throw new ArgumentNullException(nameof(modifier));
            }

            bool removed = _modifiers.Remove(modifier);
            if (removed)
            {
                _isDirty = true;
                OnModifiersChanged?.Invoke();
            }
            return removed;
        }

        /// <summary>
        /// Removes all modifiers from a specific source.
        /// Loại bỏ tất cả các bộ điều chỉnh từ một nguồn cụ thể.
        /// </summary>
        /// <param name="source">The source object to remove modifiers from.</param>
        /// <returns>The number of modifiers removed.</returns>
        public int RemoveAllFromSource(IStatModifierSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            int removedCount = _modifiers.RemoveAll(m => m.Source == source);
            if (removedCount > 0)
            {
                _isDirty = true;
                OnModifiersChanged?.Invoke();
            }
            return removedCount;
        }

        /// <summary>
        /// Removes all modifiers from this stat.
        /// Loại bỏ tất cả các bộ điều chỉnh khỏi thống kê này.
        /// </summary>
        public void ClearModifiers()
        {
            if (_modifiers.Count > 0)
            {
                _modifiers.Clear();
                _isDirty = true;
                OnModifiersChanged?.Invoke();
            }
        }

        /// <summary>
        /// Sets the base value of this stat and marks the final value for recalculation.
        /// Đặt giá trị cơ bản của thống kê này và đánh dấu giá trị cuối cùng để tính toán lại.
        /// </summary>
        /// <param name="value">The new base value.</param>
        public virtual void SetBaseValue(float value)
        {
            if (Approximately(_baseValue, value))
                return;
            _baseValue = value;
            _isDirty = true;
        }

        /// <summary>
        /// Recalculates the final value based on the base value and all applied modifiers.
        /// Modifiers are applied in the following order:
        /// 1. All Flat modifiers are summed and added to the base value.
        /// 2. All PercentAdd modifiers are summed and applied multiplicatively (as 1 + sum of percentages).
        /// 3. All PercentMultiply modifiers are applied multiplicatively in sequence.
        /// </summary>
        protected virtual void RecalculateFinalValue()
        {
            float oldValue = _finalValue;

            float flatBonus = 0f;
            float percentAddBonus = 0f;
            float percentMultiplyFactor = 1f;
            
            int count = _modifiers.Count;
            for (int i = 0; i < count; i++)
            {
                StatModifier modifier = _modifiers[i];

                switch (modifier.Type)
                {
                    case StatModifierType.Flat:
                        flatBonus += modifier.Value;
                        break;

                    case StatModifierType.PercentAdd:
                        percentAddBonus += modifier.Value;
                        break;

                    case StatModifierType.PercentMultiply:
                        percentMultiplyFactor *= (1f + modifier.Value);
                        break;
                }
            }

            _finalValue = (_baseValue + flatBonus) * (1f + percentAddBonus) * percentMultiplyFactor;
            _isDirty = false;

            if (!Approximately(oldValue, _finalValue))
            {
                OnValueChanged?.Invoke(oldValue, _finalValue);
            }
            
        }

        /// <summary>
        /// Checks if there are any modifiers from a specific source currently applied to this stat.
        /// Kiểm tra xem có bất kỳ bộ điều chỉnh nào từ một nguồn cụ thể hiện đang được áp dụng cho thống kê này hay không.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool HasModifierFromSource(IStatModifierSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            for (int i = 0; i < _modifiers.Count; i++)
            {
                if (_modifiers[i].Source == source)
                    return true;
            }

            return false;
        }
    }
}