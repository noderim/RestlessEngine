using System;
using RestlessEngine.Diagnostics;
using RestlessLib.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RestlessEngine
{
    /// <summary>
    /// Implements IInitializable, IValidable, and IMonitorable interfaces.
    /// Iinitializable - Initialize method, IValidable - SelfValidable Optional Method used by ValidateUtility, IMonitorable - Monitor context.
    /// Overridable: Init, BeforeInit, AfterInit, OnShutdown methods.
    /// </summary>
    /// <typeparam name="T">The type of the system inheriting from SystemCore.</typeparam>
    public abstract class SystemCore : MonoBehaviour, IValidable, IInitializable
    {
        [SerializeField, ReadOnly]
        protected bool isInitialized = false;
        public bool IsInitialized => isInitialized;

        [SerializeField]
        private int initPriority;
        public int InitPriority => initPriority;

        [SerializeField, ReadOnly]
        protected SystemState systemState;
        public SystemState SystemState => systemState;

        #region // Events
        public UnityEvent OnInitializationEvent { get; private set; } = new UnityEvent();
        public UnityEvent OnShutdownEvent { get; private set; } = new UnityEvent();
        public event Action<SystemState> OnSystemStateChanged;
        #endregion

        #region // INITIALIZATION BLOCK
        /// <summary>
        /// Initializes the system. This method is called automatically by GameInitializationManager Init Sequence.
        /// </summary> 
        /// <returns>Returns true if the system was successfully initialized, false otherwise.</returns>
        /// <remarks>
        /// This method sets the IsInitialized property to true, calls BeforeInit, Init, and AfterInit methods in sequence.
        /// Systems can override these methods to implement custom initialization logic.
        /// </remarks>
        public bool Initialize()
        {
            SetSystemState(SystemState.Initializing);
            LogManager.Log($"Initializing {name}...", LogTag.LifeCycle);
            try
            {
                BeforeInit();

                isInitialized = Init();

                OnInitializationEvent?.Invoke();

                AfterInit();
            }
            catch (Exception ex)
            {
                if (!IsInitialized)
                {
                    SetSystemState(SystemState.Error);
                    LogManager.LogError($"{name} initialization failed: {ex.Message}", LogTag.LifeCycle);
                    return false;
                }
                else
                {
                    SetSystemState(SystemState.Initialized);
                    LogManager.Log($"{name} initialization passed with errors: {ex.Message}", LogTag.LifeCycle);

                    AfterInit();
                    return true;
                }
            }
            if (!IsInitialized)
            {
                SetSystemState(SystemState.Error);
                LogManager.LogError($"{name} initialization failed", LogTag.LifeCycle);
                return false;
            }
            else
            {
                SetSystemState(SystemState.Initialized);
                LogManager.Log($"{name} initialized successfully.", LogTag.LifeCycle);

                AfterInit();
                return true;
            }
        }

        protected virtual bool Init() { return true; }

        protected virtual void BeforeInit() { }

        protected virtual void AfterInit()
        {
            if (SystemState == SystemState.Initialized)
                SetSystemState(SystemState.Running);
        }
        #endregion

        #region // SHUTDOWN BLOCK

        /// <summary>
        /// Called when the system is shutting down.
        /// </summary>
        /// <remarks>
        /// Override OnShutdown to implement custom shutdown logic that will be executed with Shutdown Method.
        /// </remarks>
        public void Shutdown()
        {
            SetSystemState(SystemState.ShuttingDown);
            LogManager.Log($"{name} shutting down...", LogTag.LifeCycle);
            OnShutdownEvent?.Invoke();

            OnShutdown();

            isInitialized = false;
            LogManager.Log($"{name} shutdown.", LogTag.LifeCycle);
            SetSystemState(SystemState.Uninitialized);
        }
        protected virtual void OnShutdown() { }

        #endregion

        #region // VALIDATION BLOCK
        /// <summary>
        /// Function to implement by inheriting classes. Override this method to implement custom validation logic.
        /// Then use ValidateUtility.Validate(this) or Just this.Validate() to validate the system.
        /// </summary>
        /// <returns></returns>
        protected virtual bool Validation()
        {
            return true;
        }

        public bool Validate()
        {
            bool validation = Validation();

            if (!validation)
            {
                LogManager.LogWarning($"Validation failed - {this.GetType().Name} [State: {systemState}]", LogTag.Validation);
            }
            else
            {
                LogManager.Log($"Validation passed - {this.GetType().Name} [State: {systemState}]", LogTag.Validation);

            }
            return validation;
        }
        #endregion

        #region // HELPER METHODS
        protected void SetSystemState(SystemState state)
        {
            systemState = state;
            OnSystemStateChanged?.Invoke(state);
        }

        [ContextMenu("Initialize")]
        protected void EditorInitialize() => Initialize();

        [ContextMenu("Shutdown")]
        protected void EditorShutdown() => Shutdown();

        protected virtual void RegisterSelf()
        {
            // Register this system in ApplicationManager
            SystemsRegistry.RegisterSystem(this);
            LogManager.Log($"{name} registered.", LogTag.LifeCycle);
        }
        #endregion

    }

    public enum SystemState
    {
        Uninitialized,
        Initializing,
        Initialized,
        Running,
        Error,
        ShuttingDown
    }
}
