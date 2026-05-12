using System;
using UnityEngine.UI;
using Game.Application.ReactiveProperty;
using Game.Application.UI.Commands;
using System.Collections.Generic;

namespace Game.Presentation.UI.Binding
{
    public static class BinderDSL
    {
        
        // =============================
        // TOGGLE
        // =============================
        public static IDisposable TwoWay(this Toggle toggle, IReactiveProperty<bool> property)
        {
            bool updating = false;

            var d1 = property.Subscribe(v =>
            {
                if (updating) return;
                updating = true;
                toggle.isOn = v;
                updating = false;
            });

            var d2 = toggle.Bind(v =>
            {
                if (updating) return;
                updating = true;
                property.Value = v;
                updating = false;
            });

            return new CompositeDisposable(d1, d2);
        }

        public static IDisposable To(this Toggle toggle, IReadOnlyReactiveProperty<bool> property)
        {
            return property.Subscribe(v => toggle.isOn = v);
        }

        public static IDisposable To<T>(
            this Toggle toggle,
            IReadOnlyReactiveProperty<T> property,
            T targetValue)
            where T : Enum
        {
            return property.Subscribe(value =>
            {
                bool isOn =
                    EqualityComparer<T>.Default.Equals(
                        value,
                        targetValue);

                toggle.SetIsOnWithoutNotify(isOn);
            });
        }
        
        public static IDisposable BindEnum<T>(
            this Toggle toggle,
            IReactiveProperty<T> property,
            T targetValue)
            where T : Enum
        {
            // =========================
            // ViewModel -> Toggle
            // =========================

            var d1 = property.Subscribe(value =>
            {
                bool isOn =
                    EqualityComparer<T>.Default.Equals(
                        value,
                        targetValue);

                toggle.SetIsOnWithoutNotify(isOn);
            });

            // =========================
            // Toggle -> ViewModel
            // =========================

            var d2 = toggle.Bind(isOn =>
            {
                if (!isOn)
                    return;

                if (EqualityComparer<T>.Default.Equals(
                        property.Value,
                        targetValue))
                    return;

                property.Value = targetValue;
            });

            return new CompositeDisposable(d1, d2);
        }
        // =============================
        // SLIDER
        // =============================

        public static IDisposable TwoWay(this Slider slider, IReactiveProperty<float> property)
        {
            bool updating = false;

            var d1 = property.Subscribe(v =>
            {
                if (updating) return;
                updating = true;
                slider.value = v;
                updating = false;
            });

            var d2 = slider.Bind(v =>
            {
                if (updating) return;
                updating = true;
                property.Value = v;
                updating = false;
            });

            return new CompositeDisposable(d1, d2);
        }

        public static IDisposable To(this Slider slider, IReadOnlyReactiveProperty<float> property)
        {
            return property.Subscribe(v => slider.value = v);
        }

        // =============================
        // INPUT FIELD
        // =============================

        public static IDisposable TwoWay(this TMPro.TMP_InputField input, IReactiveProperty<string> property)
        {
            bool updating = false;

            var d1 = property.Subscribe(v =>
            {
                if (updating) return;
                updating = true;
                input.text = v;
                updating = false;
            });

            var d2 = input.Bind(v =>
            {
                if (updating) return;
                updating = true;
                property.Value = v;
                updating = false;
            });

            return new CompositeDisposable(d1, d2);
        }

        public static IDisposable To(this TMPro.TMP_InputField input, IReadOnlyReactiveProperty<string> property)
        {
            return property.Subscribe(v => input.text = v);
        }

        // =============================
        // BUTTON
        // =============================

        public static IDisposable Command(this Button button, ICommand command)
        {
            var click = button.Bind(() =>
            {
                if (command.CanExecute())
                    command.Execute();
            });

            void UpdateState()
            {
                button.interactable = command.CanExecute();
            }

            command.CanExecuteChanged += UpdateState;
            UpdateState();

            return new DisposableAction(() =>
            {
                click.Dispose();
                command.CanExecuteChanged -= UpdateState;
            });
        }
        // =============================
        // TextMesh
        // =============================
        public static IDisposable To(this TMPro.TextMeshProUGUI text,
            IReadOnlyReactiveProperty<string> property)
        {
            return property.Subscribe(value => text.text = value);
        }
    }

    // =============================
    // COMPOSITE
    // =============================

    internal sealed class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables;

        public CompositeDisposable(params IDisposable[] disposables)
        {
            _disposables = new List<IDisposable>(disposables);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }

            _disposables.Clear();
        }
    }
}
