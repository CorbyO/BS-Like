using UnityEngine.PlayerLoop;

namespace Corby.Frameworks
{
    public interface IHandleable : IInitializable { }
    public interface IHandleable<in T>
        where T : class, new()
    {
        void Initialize(T handler);
    }
}