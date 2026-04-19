using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Application.UI.Commands
{
    public interface ICommand
    {
        bool CanExecute();
        void Execute();

        event Action CanExecuteChanged;
    }
    // public abstract class BaseCommand : ICommand
    // {
    // public event Action CanExecuteChanged;

    //     public bool CanExecute()
    //     {
    //         return OnCanExecute(default);
    //     }

    //     public void Execute()
    //     {
    //         if (!CanExecute())
    //             return;

    //         OnExecute(default);
    //     }

    //     public void Execute(T parameter)
    //     {
    //         if (!OnCanExecute(parameter))
    //             return;

    //         OnExecute(parameter);
    //     }

    //     protected virtual bool OnCanExecute(T parameter) => true;

    //     protected abstract void OnExecute(T parameter);

    //     protected void RaiseCanExecuteChanged()
    //     {
    //         CanExecuteChanged?.Invoke();
    //     }
    // }
    public abstract class BaseCommand : ICommand
    {
    public event Action CanExecuteChanged;

        public bool CanExecute()
        {
            return OnCanExecute();
        }

        public void Execute()
        {
            if (!CanExecute())
                return;

            OnExecute();
        }

        protected virtual bool OnCanExecute() => true;

        protected abstract void OnExecute();

        protected void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke();
        }
    }
    public sealed class RelayCommand : BaseCommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;
        

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        protected override void OnExecute()
        {
            _execute();
        }

        protected override bool OnCanExecute()
        {
            return _canExecute?.Invoke() ?? true;
        }
    }



    // public sealed class RelayCommand : ICommand
    // {
    //     private readonly Func<bool> _canExecute;
    //     private readonly Action _execute;

    //     public event Action CanExecuteChanged;

    //     public RelayCommand(Action execute, Func<bool> canExecute = null)
    //     {
    //         _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    //         _canExecute = canExecute;
    //     }

    //     public bool CanExecute()
    //     {
    //         return _canExecute?.Invoke() ?? true;
    //     }

    //     public void Execute()
    //     {
    //         if (!CanExecute()) return;
    //         _execute();
    //     }

    //     public void RaiseCanExecuteChanged()
    //     {
    //         CanExecuteChanged?.Invoke();
    //     }
    // }
    /// <summary>
    /// dùng cho:
    // load scene
    // call server
    // animation
    /// </summary>
    public sealed class AsyncRelayCommand : BaseCommand
    {
    private readonly Func<CancellationToken, UniTask> _execute;
    private bool _isExecuting;

        public AsyncRelayCommand(Func<CancellationToken, UniTask> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        protected override bool OnCanExecute()
        {
            return !_isExecuting;
        }

        protected override async void OnExecute()
        {
            if (_isExecuting)
                return;

            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                await _execute(CancellationToken.None);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }
    }
    // public sealed class AsyncRelayCommand : ICommand
    // {
    //     private readonly Func<CancellationToken, UniTask> _execute;
    //     private bool _isExecuting;

    //     public event Action CanExecuteChanged;

    //     public AsyncRelayCommand(Func<CancellationToken, UniTask> execute)
    //     {
    //         _execute = execute;
    //     }

    //     public bool CanExecute() => !_isExecuting;

    //     public async void Execute()
    //     {
    //         if (_isExecuting) return;

    //         _isExecuting = true;
    //         RaiseCanExecuteChanged();

    //         try
    //         {
    //             await _execute(CancellationToken.None);
    //         }
    //         finally
    //         {
    //             _isExecuting = false;
    //             RaiseCanExecuteChanged();
    //         }
    //     }

    //     private void RaiseCanExecuteChanged()
    //     {
    //         CanExecuteChanged?.Invoke();
    //     }
    // }
}