using UnityEngine;
namespace Game.Application.Core
{
    public interface IOnPreInitialize 
    {
        void OnPreInitialize();
    }
    public interface IOnPostInitialize 
    {
        void OnPostInitialize();
    }
    public interface IOnPreShutdown 
    {
        void OnPreShutdown();
    }
    public interface IOnPostShutdown 
    {
        void OnPostShutdown();
    }
}
