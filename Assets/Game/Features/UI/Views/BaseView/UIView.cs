
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;
using UnityEngine;

namespace Game.Presentation.UI
{
    public abstract class UIView<TViewModel> : MonoBehaviour , IUIView
        where TViewModel : ViewModelBase
    {
        protected TViewModel ViewModel { get; private set; }

        
        #region LIFECYCLE
        public UniTask ShowAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        public UniTask HideAsync(CancellationToken ct)
        {
            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }
        

        #endregion

        #region VIEWMODEL BINDING

        public void SetViewModel(ViewModelBase vm)
        {
            if (vm is not TViewModel typedVm)
                throw new InvalidOperationException($"Invalid ViewModel type for {GetType().Name}");
            ViewModel = typedVm;
            OnBind(ViewModel);
        }
        protected void AddBinding(IDisposable disposable)
        {
            ViewModel.AddDisposable(disposable);
        }

        protected virtual void OnBind(TViewModel vm) { }

        #endregion


    }
}