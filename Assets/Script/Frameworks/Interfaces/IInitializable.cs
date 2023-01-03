namespace Corby.Frameworks
{
    public interface IInitializable
    {
        bool IsInitialized { get; }
        void Initialize();
    }
    
    public interface IInitializable<in T> : IInitializable
    {
        void Initialize(T arg);
    }
    
    public interface IInitializable<in T1, in T2> : IInitializable
    {
        void Initialize(T1 arg1, T2 arg2);
    }
}