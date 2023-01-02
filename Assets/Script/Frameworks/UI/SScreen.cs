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
        
        protected override void OnLoadedScript()
        {
            _widgetList = new List<WWidget>(10);
            _loadCount = 0;
            _loadCompletedCount = 0;
        }

        protected override void OnBound()
        {
            Instancing().Forget();
        }

        private async UniTask Instancing()
        {
            await InstancingAttribute.Do(this, "Widgets", true);
            OnInstancing();
        }
        protected abstract void OnInstancing();
        
    }
}