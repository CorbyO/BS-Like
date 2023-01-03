using System;
using Corby.Frameworks.Attributes;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Corby.Frameworks
{
    public abstract class PProcessor : BaseBehavior, IDisposable
    {
        protected bool IsDisposed { get; private set; }
        public abstract bool IsDestroyWithScene { get; }
        public virtual void OnLevelChange() {}
        protected override void OnLoadedScript()
        {
            IsDisposed = false;
        }

        protected override async UniTask OnPostLoadedScript()
        {
            await base.OnPostLoadedScript();
            await InstancingAttribute.Do(this, "Actors", false);
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}