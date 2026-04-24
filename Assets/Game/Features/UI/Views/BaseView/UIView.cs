
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;
using UnityEngine;

namespace Game.Presentation.UI
{
    public abstract class UIView: MonoBehaviour , IUIView
    {
        public UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        public UniTask HideAsync(CancellationToken ct)
        {
            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }
        
    }
}