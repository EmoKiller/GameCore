using System;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Game.Presentation.UI.Binding
{
    internal sealed class DisposableAction : IDisposable
    {
        private Action _dispose;

        public DisposableAction(Action dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            _dispose?.Invoke();
            _dispose = null;
        }
    }
    // ================================
    // UNITY EVENT EXTENSIONS
    // ================================

    public static class UnityEventExtensions
    {
        // ---------- BUTTON ----------

        public static IDisposable Bind(this Button button, UnityAction action)
        {
            button.onClick.AddListener(action);

            return new DisposableAction(() =>
            {
                if (button != null)
                    button.onClick.RemoveListener(action);
            });
        }

        // ---------- TOGGLE ----------

        public static IDisposable Bind(this Toggle toggle, UnityAction<bool> action)
        {
            toggle.onValueChanged.AddListener(action);

            return new DisposableAction(() =>
            {
                if (toggle != null)
                    toggle.onValueChanged.RemoveListener(action);
            });
        }

        // ---------- SLIDER ----------

        public static IDisposable Bind(this Slider slider, UnityAction<float> action)
        {
            slider.onValueChanged.AddListener(action);

            return new DisposableAction(() =>
            {
                if (slider != null)
                    slider.onValueChanged.RemoveListener(action);
            });
        }

        // ---------- TMP INPUT FIELD ----------

        public static IDisposable Bind(this TMP_InputField input, UnityAction<string> action)
        {
            input.onValueChanged.AddListener(action);

            return new DisposableAction(() =>
            {
                if (input != null)
                    input.onValueChanged.RemoveListener(action);
            });
        }

        // ---------- TMP INPUT FIELD (SUBMIT) ----------

        public static IDisposable BindSubmit(this TMP_InputField input, UnityAction<string> action)
        {
            input.onSubmit.AddListener(action);

            return new DisposableAction(() =>
            {
                if (input != null)
                    input.onSubmit.RemoveListener(action);
            });
        }

        // ---------- GENERIC UNITYEVENT ----------

        public static IDisposable Bind(this UnityEvent evt, UnityAction action)
        {
            evt.AddListener(action);

            return new DisposableAction(() =>
            {
                evt.RemoveListener(action);
            });
        }

        public static IDisposable Bind<T>(this UnityEvent<T> evt, UnityAction<T> action)
        {
            evt.AddListener(action);

            return new DisposableAction(() =>
            {
                evt.RemoveListener(action);
            });
        }
    }

    
}