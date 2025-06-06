using System;
using UnityEngine;

namespace Inactive
{
    [Serializable]
    public class ObservableVariable<T> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private T _value;

        [NonSerialized]
        private T _valueBeforeChange;

        [NonSerialized]
        private bool _hasBeenDeserializedAtLeastOnce = false;

        [NonSerialized]
        private bool _isCurrentlyInDeserializationContext = false;

        public T value
        {
            get => _value;
            set
            {
                T previousValue = _value;

                bool valuesAreEqual;
                if (object.ReferenceEquals(previousValue, null) && object.ReferenceEquals(value, null)) valuesAreEqual = true;
                else if (object.ReferenceEquals(previousValue, null) || object.ReferenceEquals(value, null)) valuesAreEqual = false;
                else valuesAreEqual = previousValue.Equals(value);

                if (valuesAreEqual)
                {
                    return;
                }

                _value = value;

                if (!_isCurrentlyInDeserializationContext)
                {
                    _valueBeforeChange = previousValue;
                }

                InvokeOnChangeEvents(previousValue, _value);
            }
        }

        public event Action<T, T> OnChange;          // (TOldValue, TNewValue)
        public event Action<T> OnChangeNewVal;       // (TNewValue)


        private void InvokeOnChangeEvents(T oldValue, T newValue)
        {
            if (OnChange != null)
            {
                if (_isCurrentlyInDeserializationContext)
                {
                    Action actionToQueue = () =>
                    {
                        try { OnChange?.Invoke(oldValue, newValue); }
                        catch (Exception ex) { Debug.LogError($"ObservableVariable: Error in deferred OnChange handler: {ex}"); }
                    };
                    UnityMainThread.ExecuteOnMainThread(actionToQueue);
                }
                else
                {
                    try { OnChange.Invoke(oldValue, newValue); }
                    catch (Exception ex) { Debug.LogError($"ObservableVariable: Error in direct OnChange handler: {ex}"); }
                }
            }

            if (OnChangeNewVal != null)
            {
                if (_isCurrentlyInDeserializationContext)
                {
                    Action actionToQueue = () =>
                    {
                        try { OnChangeNewVal?.Invoke(newValue); }
                        catch (Exception ex) { Debug.LogError($"ObservableVariable: Error in deferred OnChangeNewVal handler: {ex}"); }
                    };
                    UnityMainThread.ExecuteOnMainThread(actionToQueue);
                }
                else
                {
                    try { OnChangeNewVal.Invoke(newValue); }
                    catch (Exception ex) { Debug.LogError($"ObservableVariable: Error in direct OnChangeNewVal handler: {ex}"); }
                }
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            _isCurrentlyInDeserializationContext = true;
            try
            {
                if (!_hasBeenDeserializedAtLeastOnce)
                {
                    _valueBeforeChange = _value;
                    _hasBeenDeserializedAtLeastOnce = true;
                    return;
                }

                T currentValueFromInspector = _value;
                T previousValueStored = _valueBeforeChange;

                bool valuesAreEqual;
                if (object.ReferenceEquals(previousValueStored, null) && object.ReferenceEquals(currentValueFromInspector, null)) valuesAreEqual = true;
                else if (object.ReferenceEquals(previousValueStored, null) || object.ReferenceEquals(currentValueFromInspector, null)) valuesAreEqual = false;
                else valuesAreEqual = previousValueStored.Equals(currentValueFromInspector);

                if (!valuesAreEqual)
                {
                    InvokeOnChangeEvents(previousValueStored, currentValueFromInspector);
                }
                _valueBeforeChange = _value;
            }
            finally
            {
                _isCurrentlyInDeserializationContext = false;
            }
        }

        public static implicit operator T(ObservableVariable<T> obs) => obs._value;
        public override string ToString() => _value != null ? _value.ToString() : "null";
    }
}