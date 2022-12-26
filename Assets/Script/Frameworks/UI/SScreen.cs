using System.Collections.Generic;
using Corby.Frameworks.Attributes;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Corby.Frameworks.UI
{
    public abstract class SScreen : BaseBehavior
    {
        [Bind]
        private Canvas _canvas;
        private List<WWidget> _widgetList;

        private int _loadCount;
        private int _loadCompletedCount;
        
        protected override void OnBindBefore()
        {
            _widgetList = new List<WWidget>(10);
            _loadCount = 0;
            _loadCompletedCount = 0;
        }

        protected override void OnBindAfter()
        {
            Load();
        }

        protected abstract void Load();
        protected abstract void OnLoad();
        
        private async UniTask TaskingLoad<T>(WidgetBox<T> box)
            where T : WWidget
        {
            var path = $"Assets/Prefabs/Widgets/{nameof(T)}";
            var newWidget = await Addressables.LoadAssetAsync<T>(path);
            var instanced = Instantiate(newWidget, _canvas.transform);

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
    }
}