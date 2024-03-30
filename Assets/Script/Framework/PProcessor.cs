using UnityEngine;

namespace Corby.Framework
{
    /// <summary>
    /// 해당 프레임워크에서 싱글톤인 클래스를 Processor 이라고 불립니다.<br/>
    /// Processor는 접두사로 P로 시작해야합니다.
    /// </summary>
    public abstract class PProcessor<T> : BaseBehavior 
        where T : PProcessor<T>
    {
        private static T s_instance;

        /// <summary>
        /// 해당 프로세스의 인스턴드를 가져옵니다.<br/>
        /// 만약 인스턴스가 없다면 최선을 다해서 만들어옵니다.
        /// </summary>
        public static T Instance
        {
            get
            {
                // 없으면 찾기
                s_instance ??= FindFirstObjectByType<T>(FindObjectsInactive.Include);
                
                // 그래도 없으면 만들기
                s_instance ??= new GameObject(typeof(T).Name).AddComponent<T>();
                
                s_instance.gameObject.SetActive(true);

                return s_instance;
            }
            private set => s_instance = value;
        }
        
        protected static bool HasInstance => s_instance != null;
        
        
        protected override void OnAwake()
        {
            Debug.Assert(!HasInstance, $"'{typeof(T).Name}' Processor는 이미 있지만 덮어 썼습니다.");
            
            // 자신을 인스턴스로 항상 덮도록 합니다.
            // XXX: 자신을 삭제하는것도 생각해볼 필요가 있습니다.
            s_instance = this as T;
        }

        private void OnDestroy()
        {
            if (!HasInstance)
            {
                s_instance = null;
            }
        }
    }
}