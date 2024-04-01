using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Corby.Framework
{
    /// <summary>
    /// Addressable에서 로드한 인스턴스를 제공하고 관리합니다.<br/>
    /// 인스턴스 생성은 <see cref="Create"/>로 합니다. 이후 인스턴스 제거는 <see cref="Dispose"/>로 하거나,
    /// using 구문을 사용하여 자동으로 제거할 수 있습니다.
    /// </summary>
    /// <example>
    /// <code>
    /// public void Open()
    /// {
    ///     var newSprite = await AddressableInstance&lt;Sprite&gt;.Create("Sprites/Cat");
    ///     _spriteRenderer.sprite = newSprite;
    /// }
    /// 
    /// public void Close()
    /// {
    ///     _spriteRenderer.sprite = null;
    ///     newSprite.Dispose();
    /// }
    /// </code>
    /// </example>
    /// <typeparam name="T">로드할 타입</typeparam>
    public struct AddressableInstance<T> : IAddressableInstance, IDisposable
        where T : class
    {
        private static readonly Dictionary<string, AddressableInstanceWrapper> s_instanceMap = new(256);
        public static AddressableInstance<T> Empty => new("empty", null);

        /// <summary>
        /// Addressable 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async UniTask<AddressableInstance<T>> Create(string key)
        {
            if (s_instanceMap.TryGetValue(key, out var wrapper))
            {
                // 미리 점유 되있으면 대기 (동시 로딩 방지)
                if (wrapper.Instance == null)
                {
                    await UniTask.WaitWhile(() => wrapper.IsEmpty);
                }

                wrapper.Counting();

                return (AddressableInstance<T>)wrapper.Instance;
            }

            // 미리 점유
            wrapper = AddressableInstanceWrapper.Empty;
            s_instanceMap.Add(key, wrapper);

            // 로딩 시작
            wrapper.Instance = new AddressableInstance<T>(key, await Addressables.LoadAssetAsync<T>(key));

            Debug.Log($"AddressableInstance<{typeof(T).Name}>({key}) 가 로드 되었습니다.");
            return (AddressableInstance<T>)wrapper.Instance;
        }

        private string _key;
        private T _object;
    
        public bool IsEmpty => _object == null && _key == "empty";

        private AddressableInstance(string key, T @object)
        {
            _key = key;
            _object = @object;
        }

        public static implicit operator T(AddressableInstance<T> instance)
        {
            if (!s_instanceMap.ContainsKey(instance._key))
            {
                instance._object = null;
                throw new InvalidOperationException("이 인스턴스는 이미 Dispose 되었거나 존재하지 않습니다.");
            }

            return instance._object;
        }
    
        /// <summary>
        /// 실제로 인스턴스를 제거하진 않고 참조 카운트를 줄입니다. <br/>
        /// 참조 카운트가 모두 줄었을때 인스턴스가 제거됩니다.
        /// </summary>
        /// <exception cref="InvalidOperationException">이미 해제 되었거나, 없는 인스턴스일때</exception>
        public void Dispose()
        {
            if (IsEmpty)
            {
                Debug.LogWarning("비어있는 인스턴스에 대해 Dispose를 시도했습니다.");
            }
            
            if (!s_instanceMap.TryGetValue(_key, out var wrapper))
            {
                throw new InvalidOperationException($"이 인스턴스는({_key}) 이미 Dispose 되었거나 존재하지 않습니다.");
            }

            if (wrapper.TryDiscounting()) return;

            Addressables.Release(_object);
            _object = null;
            s_instanceMap.Remove(_key);
            Debug.Log($"AddressableInstance<{typeof(T).Name}>({_key}) 가 해제 되었습니다.");
            _key = "empty";
        }
    }
}