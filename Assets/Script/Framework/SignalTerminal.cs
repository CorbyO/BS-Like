using System;

namespace Corby.Framework
{
    public static class SignalTerminal<T>
        where T : struct, ISignal<T>
    {
        private static event Action<T> _signal;
        
        public static void AddListener(ISignalListener<T> listener)
        {
            _signal += listener.OnSignal;
        }
        
        public static void RemoveListener(ISignalListener<T> listener)
        {
            _signal -= listener.OnSignal;
        }
        
        public static void Dispatch(T signal)
        {
            _signal?.Invoke(signal);
        }
    }
}