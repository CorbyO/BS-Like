using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Corby.Frameworks
{
    public abstract class PProcessor : BaseBehavior, IDisposable
    {
        public abstract bool IsDestroyWithScene { get; }
        public abstract void OnLevelChange();
        protected override void OnBindBefore()
        {
        }

        protected override void OnBindAfter()
        {
        }
        
        protected abstract void Load();
        protected abstract void OnLoad();
        
        // TODO: 오브젝트 생성을 Attribute 화 시키자.
        /*
        private async UniTask TaskingLoad<T>(WidgetBox<T> box)
            where T : AActor
        {
            var path = $"Assets/Prefabs/Widgets/{nameof(T)}";
            var newWidget = await Addressables.LoadAssetAsync<T>(path);

            _widgetList.Add(instanced);
            box.Instance = instanced;
            
            if (++_loadCompletedCount == _loadCount)
            {
                OnLoad();
            }
        }

        protected void ReserveLoad<T>(WidgetBox<T> box)
            where T : WWidget
        {
            _loadCount++;
            TaskingLoad<T>(box).Forget();
        }
        */

        public void Dispose()
        {
            
        }
    }
}