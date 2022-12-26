namespace Corby.Frameworks
{
    public interface IInitializable
    {
        bool IsInitialized { get; }
        void Initialize();
        void OnInitialized();
    }
}