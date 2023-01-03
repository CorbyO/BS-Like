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

        protected override void OnLoadedScript()
        {
            _widgetList = new List<WWidget>(10);
        }

        protected override async UniTask OnPostLoadedScript()
        {
            await base.OnPostLoadedScript();
            await InstancingAttribute.Do(this, "Widgets", true);
        }
    }
}