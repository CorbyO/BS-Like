namespace Corby.Framework
{
    public interface ISignalListener<in T>
        where T : ISignal
    {
        void OnSignal(T signal);
    }
    
    public static class SignalListenerExtensions
    {
        public static void StartListening<T>(this ISignalListener<T> listener)
            where T : struct, ISignal<T>
        {
            SignalTerminal<T>.AddListener(listener);
        }
        
        public static void StopListening<T>(this ISignalListener<T> listener)
            where T : struct, ISignal<T>
        {
            SignalTerminal<T>.RemoveListener(listener);
        }
    }
}