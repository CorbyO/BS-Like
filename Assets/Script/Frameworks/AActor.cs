using UnityEngine;

namespace Corby.Frameworks
{
    public abstract class AActor : BaseBehavior
    {
        private Transform _tempTransform;
        protected Vector2 Position
        {
            get => _tempTransform.position; 
            set => _tempTransform.position = value;
        }

        protected override void OnLoadedScript()
        {
            _tempTransform = transform;
        }

        protected override void OnBound()
        {
        }
    }
}