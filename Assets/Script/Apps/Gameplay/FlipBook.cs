using System;
using Corby.Framework;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

namespace Corby.Apps.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FlipBook : BaseBehavior, IPlayer, IDisposable
    {
        private LazyComponent<SpriteRenderer> _spriteRenderer;
        [SerializeField] [NaughtyAttributes.ReorderableList] private Sprite[] _sprites = Array.Empty<Sprite>();
        [SerializeField] private float _fps = 10;
        [NaughtyAttributes.ProgressBar("Frame", "MaxFrame")] [SerializeField] private int _frame;
        [SerializeField] private bool _isLoop;
        [SerializeField] private bool _isPlayOnAwake;
        [ShowNativeProperty] public bool IsPlaying { get; private set; }
        [ShowNativeProperty] public bool IsPaused { get; private set; }
        public int MaxFrame => _sprites.Length - 1;

        public bool IsLoop
        {
            get => _isLoop;
            set => _isLoop = value;
        }

        public bool IsPlayOnAwake
        {
            get => _isPlayOnAwake;
            set => _isPlayOnAwake = value;
        }

        public event Action<int> OnFrameChanged;
        public event Action OnPlay;
        public event Action OnStop;
        public event Action OnLoopComplete;
        public event Action<bool> OnPause;

#region Preview (Editor Only)

#if UNITY_EDITOR
        private bool _isPreview;
        
        [Button("Preview (Play/Stop)")]
        public void Preview()
        {
            if (_sprites.Length <= 1)
            {
                Debug.Log("Should have more than 1 sprite.");
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
            while (_isPreview)
            {
                _frame = (_frame + 1) % (MaxFrame + 1);
                renderer.sprite = _sprites[_frame];
                await UniTask.Delay(TimeSpan.FromSeconds(1f / _fps));
            }

            renderer.sprite = original;
            _frame = 0;
        }
#endif

#endregion

        protected override void OnAwake()
        {
            _spriteRenderer = new LazyComponent<SpriteRenderer>(this);
            _frame = 0;
        }

        protected override void OnStart()
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
                for (var i = 0; i < length; i++)
                {
                    if (IsPaused)
                    {
                        OnPause?.Invoke(true);
                        await UniTask.WaitWhile(() => IsPaused);
                        if (!IsPlaying) goto STOP;
                        OnPause?.Invoke(false);
                    }

                    if (!IsPlaying) goto STOP;
                    _spriteRenderer.Get().sprite = _sprites[i];
                    OnFrameChanged?.Invoke(i);

                    await UniTask.Delay(TimeSpan.FromSeconds(1 / _fps));
                }

                OnLoopComplete?.Invoke();
            }

            STOP:
            OnStop?.Invoke();
        }

        public void SetSprites(Sprite[] sprites)
        {
            if (IsPlaying)
            {
                Stop();
            }

            _sprites = sprites;
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
                Debug.Log("Cannot pause in not playing.");
            }

            IsPaused = true;
        }

        public void Unpause()
        {
            if (!IsPlaying)
            {
                Debug.Log("Cannot unpause in not playing.");
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