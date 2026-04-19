using System;
using System.Collections.Generic;

namespace Game.Application.ReactiveProperty
{
    public interface IReadOnlyReactiveProperty<T>
    {
        T Value { get; }
        IDisposable Subscribe(Action<T> listener);
    }

    public interface IReactiveProperty<T> : IReadOnlyReactiveProperty<T>
    {
        new T Value { get; set; }
    }

    public sealed class ReactiveProperty<T> : IReactiveProperty<T>
    {
        private T _value;
        private readonly List<Action<T>> _listeners = new(4);

        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value))
                    return;

                _value = value;
                Notify();
            }
        }

        public ReactiveProperty(T initialValue = default)
        {
            _value = initialValue;
        }

        public IDisposable Subscribe(Action<T> listener)
        {
            _listeners.Add(listener);

            // push current value immediately (important for UI)
            listener(_value);

            return new Subscription(this, listener);
        }

        private void Notify()
        {
            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].Invoke(_value);
            }
        }

        private void Unsubscribe(Action<T> listener)
        {
            _listeners.Remove(listener);
        }

        private sealed class Subscription : IDisposable
        {
            private readonly ReactiveProperty<T> _owner;
            private readonly Action<T> _listener;

            public Subscription(ReactiveProperty<T> owner, Action<T> listener)
            {
                _owner = owner;
                _listener = listener;
            }

            public void Dispose()
            {
                _owner.Unsubscribe(_listener);
            }
        }
    }
    // /// <summary>
    // /// ^v có vẻ giống nhau
    // /// </summary>
    // public sealed class ComputedProperty<T>
    // {
    //     private readonly Func<T> _compute;
    //     private readonly List<Action<T>> _listeners = new();

    //     public ComputedProperty(Func<T> compute)
    //     {
    //         _compute = compute;
    //     }

    //     public void Notify()
    //     {
    //         var value = _compute();

    //         for (int i = 0; i < _listeners.Count; i++)
    //         {
    //             _listeners[i].Invoke(value);
    //         }
    //     }

    //     public IDisposable Subscribe(Action<T> listener)
    //     {
    //         _listeners.Add(listener);
    //         listener(_compute());
    //         return new Subscription(this, listener);
    //     }

    //     private void Unsubscribe(Action<T> l)
    //     {
    //         _listeners.Remove(l);
    //     }

    //     private sealed class Subscription : IDisposable
    //     {
    //         private readonly ComputedProperty<T> _owner;
    //         private readonly Action<T> _listener;

    //         public Subscription(ComputedProperty<T> owner, Action<T> listener)
    //         {
    //             _owner = owner;
    //             _listener = listener;
    //         }

    //         public void Dispose()
    //         {
    //             _owner.Unsubscribe(_listener);
    //         }
    //     }
    // }





    public static class ReactivePropertyExtensions
    {
        public static IReadOnlyReactiveProperty<TResult> Select<T, TResult>(
            this IReadOnlyReactiveProperty<T> source,
            Func<T, TResult> selector)
        {
            var result = new ReactiveProperty<TResult>(selector(source.Value));

            source.Subscribe(value =>
            {
                result.Value = selector(value);
            });

            return result;
        }
        public static IReadOnlyReactiveProperty<(T1, T2)> Combine<T1, T2>(
            IReadOnlyReactiveProperty<T1> p1,
            IReadOnlyReactiveProperty<T2> p2)
        {
            var result = new ReactiveProperty<(T1, T2)>((p1.Value, p2.Value));

            void Update() => result.Value = (p1.Value, p2.Value);

            p1.Subscribe(_ => Update());
            p2.Subscribe(_ => Update());

            return result;
        }

    }
    /// <summary>
    /// One-way binding helper
    /// </summary>
    public static class BindingExtensions
    {
        public static IDisposable BindTo<T>(
            this ReactiveProperty<T> property,
            Action<T> setter)
        {
            return property.Subscribe(setter);
        }
    }
}