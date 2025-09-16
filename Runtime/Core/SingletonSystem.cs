namespace RestlessEngine
{
    /// <summary>
    /// Singleton Base Systems class implements SystemCore, adds Singleton generic architecture
    /// </summary>
    public abstract class SingletonSystem<T> : SystemCore where T : SingletonSystem<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Avoid duplicates
                return;
            }

            Instance = this as T;

            RegisterSelf();
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
