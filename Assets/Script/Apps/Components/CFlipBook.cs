using System;
using Corby.Frameworks;
using Corby.Frameworks.Attributes;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

namespace Script.Apps.Components
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CFlipBook : CComponent, IPlayer, IDisposable
    {
        [SerializeField]
        private Sprite[] _sprites;
        [SerializeField]
        private float _fps = 1;
        [SerializeField]
        private bool _isLoop;
        [SerializeField]
        private bool _isPlayOnAwake;
        [Bind]
        private SpriteRenderer _spriteRenderer;
        [ShowNativeProperty]
        public bool IsPlaying { get; private set; }
        [ShowNativeProperty]
        public bool IsPaused { get; private set; }
        public bool IsLoop { get => _isLoop; set => _isLoop = value; }
        public bool IsPlayOnAwake { get => _isPlayOnAwake; set => _isPlayOnAwake = value; }
        
        public event Action<int> OnFrameChanged;
        public event Action OnPlay;
        public event Action OnStop;
        public event Action OnLoopComplete;
        public event Action<bool> OnPause;

        #region Preview (Editor Only)
#if UNITY_EDITOR
        private bool _isPreview = false;
        [ProgressBar("Frame", "_length")]
        [ShowIf("_isPreview")]
        [SerializeField]
        private int _frame = 0;
        private int _length = 0;
        [Button("Preview (Play/Stop)")]
        private void Preview()
        {
            if (_sprites.Length <= 1)
            {
                Log("Should have more than 1 sprite.");
            }
            _isPreview = !_isPreview;
            if (_isPreview)
            {
                PlayPreview().Forget();
            }
        }
        private async UniTask PlayPreview()
        {
            var renderer = GetComponent<SpriteRenderer>();
            var original = renderer.sprite;

            _frame = 0;
            _length = _sprites.Length - 1;
            while (_isPreview)
            {
                _frame = (_frame + 1) % (_length + 1);
                renderer.sprite = _sprites[_frame];
                await UniTask.Delay(TimeSpan.FromSeconds(1f / _fps));
            }
            renderer.sprite = original;
        }
#endif
        #endregion

        private void Start()
        {
            if (IsPlayOnAwake)
            {
                Play();
            }
        }

        private async UniTask PlayProcess()
        {
            var length = _sprites.Length;
            
            OnPlay?.Invoke();
            while (IsLoop)
            {
                for (var i = 0 ; i < length; i++)
                {
                    if (IsPaused)
                    {
                        OnPause?.Invoke(true);
                        await UniTask.WaitWhile(() => IsPaused);
                        if (!IsPlaying) goto STOP;
                        OnPause?.Invoke(false);
                    }
                    
                    if (!IsPlaying) goto STOP;
                    _spriteRenderer.sprite = _sprites[i];
                    OnFrameChanged?.Invoke(i);
                    
                    await UniTask.Delay(TimeSpan.FromSeconds(1 / _fps));
                }
                OnLoopComplete?.Invoke();
            }
            
            STOP:
            OnStop?.Invoke();
        }

        #region IPlayer
        public void Play()
        {
            if (IsPlaying) return;
            IsPlaying = true;
            IsPaused = false;
            PlayProcess().Forget();
        }

        public void Pause()
        {
            if (!IsPlaying)
            {
                Log("Cannot pause in not playing.");
            }

            IsPaused = true;
        }

        public void Unpause()
        {
            if (!IsPlaying)
            {
                Log("Cannot unpause in not playing.");
                return;
            }

            IsPaused = false;
        }

        public void Stop()
        {
            IsPlaying = false;
            IsPaused = false;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Stop();
        }
        #endregion
    }
}