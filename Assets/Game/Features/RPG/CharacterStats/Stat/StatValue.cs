using System;
using System.Collections.Generic;


namespace Game.Character.Core.Stats
{
    public interface IStat
    {
        void AddModifier(StatModifier modifier);
        bool RemoveModifier(StatModifier modifier);
        int RemoveAllFromSource(IStatModifierSource source);
        void ClearModifiers();
        void SetBaseValue(float value);
    }
    /// <summary>
    /// Defines a read-only view of a stat value with base and final (modified) values.
    /// Xác định một cái nhìn chỉ đọc về giá trị thống kê với giá trị cơ bản và cuối cùng (đã sửa đổi).
    /// </summary>
    public interface IReadOnlyStat
    {
        /// <summary>
        /// Gets the base value of the stat before any modifications are applied.
        /// Lấy giá trị cơ bản của thống kê trước khi bất kỳ sửa đổi nào được áp dụng.
        /// </summary>
        float BaseValue { get; }

        /// <summary>
        /// Gets the final value of the stat after all modifications have been applied.
        /// Lấy giá trị cuối cùng của thống kê sau khi tất cả các sửa đổi đã được áp dụng.
        /// </summary>
        float FinalValue { get; }
    }
    public sealed class StatValue : IReadOnlyStat, IStat
    {
        private float _baseValue;
        private float _finalValue;

        private StatModifier[] _modifiers;
        private int _count;

        public float BaseValue => _baseValue;
        public float FinalValue => _finalValue;

        // ========================
        // EVENTS
        // ========================

        /// <summary>
        /// Triggered when final value changes (old, new)
        /// </summary>
        public event Action<float, float> OnValueChanged;

        /// <summary>
        /// Triggered when modifier list changes
        /// (add/remove/clear)
        /// </summary>
        public event Action OnModifiersChanged;

        private const int DefaultCapacity = 8;
        private bool _isRecalculating;

        public StatValue(float baseValue, int capacity = DefaultCapacity)
        {
            _baseValue = baseValue;
            _finalValue = baseValue;

            _modifiers = new StatModifier[Math.Max(capacity, 1)];
            _count = 0;
        }

        // ========================
        // ADD
        // ========================

        public void AddModifier(StatModifier modifier)
        {
            if (modifier == null)
                throw new ArgumentNullException(nameof(modifier));

            if (float.IsNaN(modifier.Value) || float.IsInfinity(modifier.Value))
                throw new ArgumentException("Modifier value is invalid", nameof(modifier));

            EnsureCapacity();

            InsertSorted(modifier);

            OnModifiersChanged?.Invoke(); // ✔ structural change first
            Recalculate();
        }

        // ========================
        // REMOVE (single)
        // ========================

        public bool RemoveModifier(StatModifier modifier)
        {
            if (modifier == null)
                throw new ArgumentNullException(nameof(modifier));

            for (int i = 0; i < _count; i++)
            {
                if (_modifiers[i] == modifier)
                {
                    RemoveAt(i);

                    OnModifiersChanged?.Invoke();
                    Recalculate();

                    return true;
                }
            }

            return false;
        }

        // ========================
        // REMOVE BY SOURCE
        // ========================

        public int RemoveAllFromSource(IStatModifierSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            int removed = 0;

            for (int i = _count - 1; i >= 0; i--)
            {
                if (_modifiers[i].Source == source)
                {
                    RemoveAt(i);
                    removed++;
                }
            }

            if (removed > 0)
            {
                OnModifiersChanged?.Invoke();
                Recalculate();
            }

            return removed;
        }

        // ========================
        // CLEAR
        // ========================

        public void ClearModifiers()
        {
            if (_count == 0)
                return;

            Array.Clear(_modifiers, 0, _count);
            _count = 0;

            OnModifiersChanged?.Invoke();
            Recalculate();
        }

        // ========================
        // BASE VALUE
        // ========================

        public void SetBaseValue(float value)
        {
            if (Approximately(_baseValue, value))
                return;

            _baseValue = value;
            Recalculate();
        }

        // ========================
        // CORE CALCULATION
        // ========================

        private void Recalculate()
        {
            if (_isRecalculating)
                return;

            _isRecalculating = true;

            float oldValue = _finalValue;

            float flat = 0f;
            float percentAdd = 0f;
            float percentMul = 1f;

            for (int i = 0; i < _count; i++)
            {
                var mod = _modifiers[i];

                switch (mod.Type)
                {
                    case StatModifierType.Flat:
                        flat += mod.Value;
                        break;

                    case StatModifierType.PercentAdd:
                        percentAdd += mod.Value;
                        break;

                    case StatModifierType.PercentMultiply:
                        percentMul *= (1f + mod.Value);
                        break;
                }
            }

            float newValue = (_baseValue + flat) * (1f + percentAdd) * percentMul;

            if (!Approximately(oldValue, newValue))
            {
                _finalValue = newValue;
                OnValueChanged?.Invoke(oldValue, newValue);
            }

            _isRecalculating = false;
        }

        // ========================
        // INTERNAL HELPERS
        // ========================

        private void RemoveAt(int index)
        {
            _count--;

            if (index < _count)
            {
                _modifiers[index] = _modifiers[_count];
            }

            _modifiers[_count] = null;
        }

        private void EnsureCapacity()
        {
            if (_count < _modifiers.Length)
                return;

            int newSize = _modifiers.Length * 2;

            var newArr = new StatModifier[newSize];
            Array.Copy(_modifiers, newArr, _modifiers.Length);

            _modifiers = newArr;
        }

        private static bool Approximately(float a, float b)
        {
            return Math.Abs(a - b) < 0.0001f;
        }
        private void InsertSorted(StatModifier modifier)
        {
            EnsureCapacity();

            int i = _count;

            while (i > 0 && _modifiers[i - 1].Priority > modifier.Priority)
            {
                _modifiers[i] = _modifiers[i - 1];
                i--;
            }

            _modifiers[i] = modifier;
            _count++;
        }    
    }

}