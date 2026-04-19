using System;
using System.Collections.Generic;

namespace Game.Shared.Lifecycle
{
    public abstract class DisposableObject : IDisposable
    {
        private readonly List<IDisposable> _disposables = new(8);
        private bool _disposed;

        public void AddDisposable(IDisposable disposable)
        {
            if (_disposed)
            {
                disposable.Dispose();
                return;
            }

            _disposables.Add(disposable);
        }

        public virtual void Dispose()
        {
            if (_disposed) 
                return;

            _disposed = true;

            for (int i = _disposables.Count - 1; i >= 0; i--)
            {
                _disposables[i]?.Dispose();
            }

            _disposables.Clear();
        }
    }
}