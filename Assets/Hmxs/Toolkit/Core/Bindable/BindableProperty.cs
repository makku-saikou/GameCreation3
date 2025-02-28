using System;
using UnityEngine;
using UnityEngine.Events;

namespace Hmxs.Toolkit.Core.Bindable
{
    [Serializable]
    public struct BindableProperty<T>
    {
        [SerializeField] private T value;

        public UnityEvent<T> onValueChanged;

        public T Value
        {
            get => value;
            set
            {
                if (Equals(this.value, value)) return;
                this.value = value;
                onValueChanged?.Invoke(value);
            }
        }

        public BindableProperty(T value = default, UnityAction<T> callback = null)
        {
            this.value = value;
            onValueChanged = new UnityEvent<T>();
            if (callback != null) onValueChanged.AddListener(callback);
        }
    }
}