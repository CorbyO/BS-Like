using System;
using System.Linq;
using System.Threading;
using Corby.Framework;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Corby.Apps.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FlipBook : BaseBehavior, IPlayer, IDisposable
    {
        private LazyComponent<SpriteRenderer> _spriteRenderer;
        [Title("Settings")]
        [SerializeField] private Sprite[] _sprites = Array.Empty<Sprite>();
        [SerializeField] private float _fps = 10;
        [SerializeField] [ProgressBar("MinFrame", "MaxFrame")] [ReadOnly] private int _frame;
        [Title("Options")]
        [SerializeField] private bool _isLoop;
        [SerializeField] private bool _isPlayOnAwake;
        [ShowInInspector] [ReadOnly] public bool IsPlaying { get; private set; }
        [ShowInInspector] [ReadOnly] public bool IsPaused { get; private set; }
        private int MinFrame => 0;
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
        private CancellationTokenSource _cts;
        
        [Button("Preview (Play/Stop)")]
        public void Preview()
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
                Debug.Log("1");
                _cts = new CancellationTokenSource();
                PlayPreview(_cts.Token).Forget();
            }
            else
            {
                Debug.Log("2");
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
                renderer.sprite = original;
                _frame = 0;
            }
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