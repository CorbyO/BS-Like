using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

namespace Corby.Framework
{
    public abstract class BaseBehavior : SerializedMonoBehaviour
    {
        /// <summary>
        /// <see cref="Awake"/> 될때 호출되는 메서드
        /// </summary>
        protected virtual void OnAwake() { }
        /// <summary>
        /// <see cref="Awake"/> 될때 호출되는 메서드 (비동기)
        /// </summary>
        protected virtual async UniTask OnAwakeAsync() => await UniTask.CompletedTask;
        
        /// <summary>
        /// <see cref="Start"/> 될때 호출되는 메서드
        /// </summary>
        protected virtual void OnStart() { }
        /// <summary>
        /// <see cref="Start"/> 될때 호출되는 메서드 (비동기)
        /// </summary>
        protected virtual async UniTask OnStartAsync() => await UniTask.CompletedTask;
        
        /// <summary>
        /// 비동기 함수를 선언해두기 위해서 오버라이드하지 않고, 두함수로 나누었습니다.
        /// </summary>
        private void Awake()
        {
            OnAwake();
            OnAwakeAsync().Forget();
        }

        /// <summary>
        /// 비동기 함수를 선언해두기 위해서 오버라이드하지 않고, 두함수로 나누었습니다.
        /// </summary>
        private void Start()
        {
            OnStart();
            OnStartAsync().Forget();
        }
    }
}
