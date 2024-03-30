using Unity.VisualScripting;
using UnityEngine;

namespace Corby.Framework
{
    /// <summary>
    /// 필요할때 <see cref="GameObject.GetComponent{T}"/>를 호출하고, 캐시 하는 클래스입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyComponent<T> 
        where T : Component
    {
        private GameObject _owner;
        private T _component;
        
        public LazyComponent(GameObject owner)
        {
            _owner = owner;
        }
        
        public LazyComponent(Component component)
        {
            _owner = component.gameObject;
        }

        public T Get()
        {
            if (_component == null)
            {
                _component = _owner.GetComponent<T>();
            }

            return _component.AsUnityNull() ?? throw new MissingComponentException($"'{typeof(T).Name}' 컴포넌트가 없습니다.");
        }
        
        public static implicit operator T(LazyComponent<T> lazyComponent)
        {
            return lazyComponent.Get();
        }
    }
}