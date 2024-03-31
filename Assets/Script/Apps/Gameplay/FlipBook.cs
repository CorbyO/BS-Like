using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Corby.Framework;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Corby.Apps.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FlipBook : BaseBehavior, IPlayer
    {
        private SpriteRenderer _spriteRenderer;
        private Sprite _originalSprite;
        private CancellationTokenSource _cts;
        [Title("Settings")]
        [SerializeField] private Sprite[] _sprites = Array.Empty<Sprite>();
        [SerializeField] private float _fps = 10;
        [SerializeField] [ProgressBar("MinFrame", "MaxFrame")] [ReadOnly] private int _frame;
        [Title("Options")]
        [SerializeField] private bool _isLoop;
        [SerializeField] private bool _isPlayOnStart;
        [ShowInInspector] [ReadOnly] public bool IsPlaying => _cts != null;
        [ShowInInspector] [ReadOnly] public bool IsPaused { get; private set; }
        private int MinFrame => 0;
        public int MaxFrame => _sprites.Length - 1;

        public bool IsLoop
        {
            get => _isLoop;
            set => _isLoop = value;
        }

        public bool IsPlayOnStart
        {
            get => _isPlayOnStart;
            set => _isPlayOnStart = value;
        }

        public event Action<int> OnFrameChanged;
        public event Action OnPlay;
        public event Action OnStop;
        public event Action OnLoopComplete;
        public event Action<bool> OnPause;

#region Preview (Editor Only)

        [Conditional("UNITY_EDITOR")]
        [Button("Preview (Play/Stop)")]
        private void Preview()
        {
            if (_sprites.Length <= 1)
            {
                Debug.Log("Should have more than 1 sprite.");
            }

            foreach (var flipBook in GetComponents<FlipBook>())
            {
                if (flipBook == this) continue;
                flipBook._cts?.Cancel();
                flipBook._cts?.Dispose();
                flipBook._cts = null;
            }

            if (_cts == null)
            {
                _cts = new CancellationTokenSource();
                PlayPreview(_cts.Token).Forget();
            }
            else
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
        
        private async UniTask PlayPreview(CancellationToken token)
        {
            var renderer = GetComponent<SpriteRenderer>();
            var original = renderer.sprite;
            _frame = 0;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    _frame = (_frame + 1) % (MaxFrame + 1);
                    renderer.sprite = _sprites[_frame];
                    await UniTask.Delay(TimeSpan.FromSeconds(1f / _fps), cancellationToken: token);
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                GetComponent<SpriteRenderer>().sprite = original;
                _frame = 0;
            }
        }

#endregion

        protected override void OnAwake()
        {
            _spriteRenderer = new LazyComponent<SpriteRenderer>(this);
            _originalSprite = _spriteRenderer.sprite;
            _frame = 0;
        }

        protected override void OnStart()
        {
            if (IsPlayOnStart)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        private async UniTask PlayProcess(CancellationToken token)
        {
            var length = _sprites.Length;
            var waitTime = TimeSpan.FromSeconds(1 / _fps);

            OnPlay?.Invoke();

            try
            {
                while (IsLoop)
                {
                    for (var i = 0; i < length; i++)
                    {
                        if (IsPaused)
                        {
                            OnPause?.Invoke(true);
                            await UniTask.WaitWhile(() => IsPaused, cancellationToken: token);
                            if (!IsPlaying) return;
                            OnPause?.Invoke(false);
                        }

                        _spriteRenderer.sprite = _sprites[i];
                        OnFrameChanged?.Invoke(i);

                        await UniTask.Delay(waitTime, cancellationToken: token);
                    }

                    OnLoopComplete?.Invoke();
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                _spriteRenderer.sprite = _originalSprite;
                _frame = 0;
                IsPaused = false;
            }

            OnStop?.Invoke();
        }

        public void SetSprites(Sprite[] sprites)
        {
            var wasPlaying = IsPlaying;
            if (wasPlaying)
            {
                Stop();
            }

            _sprites = sprites;
            
            if (wasPlaying)
            {
                Play();
            }
        }

        public void Play()
        {
            if (IsPlaying) return;
            IsPaused = false;
            PlayProcess(_cts.Token).Forget();
        }

        public void Pause()
        {
            if (!IsPlaying) return;

            IsPaused = true;
        }

        public void Unpause()
        {
            if (!IsPlaying) return;

            IsPaused = false;
        }

        public void Stop()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}