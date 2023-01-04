using System;
using Corby.Frameworks;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Corby.Apps.Processors
{
    public class PCameraExtension : PProcessor, IInitializable<Camera, ReadonlyTransform>
    {
        public override bool IsDestroyWithScene => true;
        public bool IsInitialized { get; private set; }
        
        private Camera _camera;
        private Transform _cameraTransform;
        private ReadonlyTransform _target;

        public float Speed { get; set; }

        protected override void OnLoadedScript()
        {
            base.OnLoadedScript();
            IsInitialized = false;
            Speed = 10f;
        }

        public void Initialize([NotNull] Camera cam, [NotNull] ReadonlyTransform target)
        {
            if (cam == null) throw new ArgumentNullException(nameof(cam));
            if (target == null) throw new ArgumentNullException(nameof(target));
            Initialize();
            _camera = cam;
            _cameraTransform = cam.transform;
            _target = target;
            
            CamUpdate().Forget();
        }

        public void Initialize()
        {
            IsInitialized = true;
        }

        private async UniTask CamUpdate()
        {
            await UniTask.Yield();
            var position = _cameraTransform.position;
            while (!IsDisposed)
            {
                var distance = _target.Position - position;
                distance.z = 0;

                position += Speed * Time.deltaTime * distance;
                
                _cameraTransform.position = position;
                await UniTask.Yield();
            }
        }
    }
}