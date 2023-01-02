using System;
using Corby.Frameworks.Attributes;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Corby.Frameworks
{
    public abstract class PProcessor : BaseBehavior, IDisposable
    {
        public abstract bool IsDestroyWithScene { get; }
        public abstract void OnLevelChange();
        protected override void OnLoadedScript()
        {
        }

        protected override void OnBound()
        {
            Instancing().Forget();
        }
        
        private async UniTask Instancing()
        {
            await InstancingAttribute.Do(this, "Actors", false);
            OnInstancing();
        }
        protected abstract void OnInstancing();

        public void Dispose()
        {
            
        }
    }
}