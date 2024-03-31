namespace Corby.Framework
{
    public interface ISignal {}
    public interface ISignal<T> : ISignal where T : struct { }
    
    
    public static class SignalExtensions
    {
        public static void Dispatch<T>(this T signal) 
            where T : struct, ISignal<T>
        {
            SignalTerminal<T>.Dispatch(signal);
        }
        
        public static void AddListener<T>(this ISignalListener<T> listener) 
            where T : struct, ISignal<T>
        {
            SignalTerminal<T>.AddListener(listener);
        }
        
        public static void RemoveListener<T>(this ISignalListener<T> listener) 
            where T : struct, ISignal<T>
        {
            SignalTerminal<T>.RemoveListener(listener);
        }
    }
}