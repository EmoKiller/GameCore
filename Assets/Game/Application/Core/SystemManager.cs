using System;
using System.Collections.Generic;
using Game.Application.Core;
using UnityEngine;

    public interface ISystemManager : IService
    {
        void Register(IGameSystem system, IServiceContainer services);
        void Unregister(IGameSystem system);
    }
    public sealed class SystemManager : ISystemManager
    {
        private readonly List<IGameSystem> _systems = new();
        private readonly List<IGameSystem> _pendingAdd = new();
        private readonly List<IGameSystem> _pendingRemove = new();

        private bool _isTicking;

        #region Register / Unregister

        public void Register(IGameSystem system, IServiceContainer services)
        {
            if (system == null) return;

            if (_isTicking)
            {
                _pendingAdd.Add(system);
                return;
            }

            AddInternal(system, services);
        }

        public void Unregister(IGameSystem system)
        {
            if (system == null) return;

            if (_isTicking)
            {
                _pendingRemove.Add(system);
                return;
            }

            RemoveInternal(system);
        }

        private void AddInternal(IGameSystem system, IServiceContainer services)
        {
            _systems.Add(system);

            try
            {
                system.Initialize(services);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SystemManager] Init error in {system.GetType().Name}\n{e}");
            }

            SortSystems();
        }

        private void RemoveInternal(IGameSystem system)
        {
            _systems.Remove(system);
        }

        #endregion

        #region Tick Pipeline

        public void Tick(float dt)
        {
            _isTicking = true;

            for (int i = 0; i < _systems.Count; i++)
            {
                try
                {
                    _systems[i].Tick(dt);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SystemManager] Tick error in {_systems[i].GetType().Name}\n{e}");
                }
            }

            _isTicking = false;

            ApplyPending();
        }

        public void FixedTick(float dt)
        {
            for (int i = 0; i < _systems.Count; i++)
            {
                try
                {
                    _systems[i].FixedTick(dt);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SystemManager] FixedTick error in {_systems[i].GetType().Name}\n{e}");
                }
            }
        }

        public void LateTick(float dt)
        {
            for (int i = 0; i < _systems.Count; i++)
            {
                try
                {
                    _systems[i].LateTick(dt);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SystemManager] LateTick error in {_systems[i].GetType().Name}\n{e}");
                }
            }
        }

        #endregion

        #region Pending Apply

        private void ApplyPending()
        {
            // REMOVE
            if (_pendingRemove.Count > 0)
            {
                for (int i = 0; i < _pendingRemove.Count; i++)
                {
                    _systems.Remove(_pendingRemove[i]);
                }

                _pendingRemove.Clear();
            }

            // ADD
            if (_pendingAdd.Count > 0)
            {
                for (int i = 0; i < _pendingAdd.Count; i++)
                {
                    _systems.Add(_pendingAdd[i]);
                }

                _pendingAdd.Clear();

                SortSystems();
            }
        }

        #endregion

        #region Shutdown

        public void Shutdown()
        {
            for (int i = _systems.Count - 1; i >= 0; i--)
            {
                try
                {
                    _systems[i].Shutdown();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SystemManager] Shutdown error in {_systems[i].GetType().Name}\n{e}");
                }
            }

            _systems.Clear();
            _pendingAdd.Clear();
            _pendingRemove.Clear();
        }

        #endregion

        #region Helpers

        private void SortSystems()
        {
            _systems.Sort((a, b) => a.Order.CompareTo(b.Order));
        }

        #endregion
    }