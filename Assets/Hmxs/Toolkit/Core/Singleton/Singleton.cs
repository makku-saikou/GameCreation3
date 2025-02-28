using System;

namespace Hmxs.Toolkit
{
    /// <summary>
    /// generic singleton base class
    /// </summary>
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static readonly Lazy<T> InstanceHolder = new(() =>
        {
            var instance = new T();
            instance.OnInstanceInit(instance);
            return instance;
        });

        public static T Instance => InstanceHolder.Value;

        /// <summary>
        /// called when instance is initialized
        /// </summary>
        protected virtual void OnInstanceInit(T instance) {}

        protected Singleton() { }
    }
}