using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RPG.Utils
{
    /// <summary>
    /// 쿨타임 전용 타이머입니다.<br/>
    /// 기본적인 플레이 기능에는 <see cref="Start"/>, <see cref="Stop"/>, <see cref="Pause"/>, <see cref="Resume"/>이 있습니다.<br/>
    /// 그외에 추가로 컨트롤을 위해 <see cref="Speed"/>를 바꾸거나, <see cref="PushTime"/>로 시간을 직접 누적시킬 수 있습니다.<br/>
    /// </summary>
    public class CooldownTimer
    {
        /// <summary>
        /// 재생 상태 입니다.
        /// </summary>
        public enum EState
        {
            /// <summary>
            /// 실행중
            /// </summary>
            Running,
            /// <summary>
            /// 일시정지
            /// </summary>
            Paused,
            /// <summary>
            /// 정지, 작동중 아님
            /// </summary>
            Stopped
        }
        
        /// <summary>
        /// 시작 시간
        /// </summary>
        private float _startTime;
        
        /// <summary>
        /// 누적 시간
        /// </summary>
        private float _accumulatedTime = 0.0f;

        /// <summary>
        /// 현재 상태입니다.
        /// </summary>
        public EState State { get; private set; } = EState.Stopped;
        /// <summary>
        /// 쿨다운 시간 입니다.
        /// </summary>
        private float _duration;
        private float _speed = 1f;

        /// <summary>
        /// 배속
        /// </summary>
        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                if (State != EState.Running) return;
                Accumulate();
            }
        }
        /// <summary>
        /// 쿨다운 시간 입니다.
        /// </summary>
        public float Duration
        {
            get => _duration;
            private set
            {
                _duration = value;
                _duration = Mathf.Max(0, _duration);
            }
        }

        public CooldownTimer(float duration)
        {
            Duration = duration;
        }
        
        /// <summary>
        /// 쿨다운 타이머를 시작합니다. <see cref="Duration"/>이 0이면 시작하지 않습니다.
        /// </summary>
        public void Start()
        {
            StartAsync().Forget();
        }

        /// <summary>
        /// 강제로 종료합니다.
        /// </summary>
        public void Stop()
        {
            State = EState.Stopped;
        }
        
        /// <summary>
        /// 일시정지 합니다.
        /// </summary>
        public void Pause()
        {
            State = EState.Paused;
            Accumulate();
        }
        
        /// <summary>
        /// 시간을 누적시켜서 빼거나 더합니다.
        /// </summary>
        /// <param name="time"></param>
        public void PushTime(float time)
        {
            _accumulatedTime += time;
        }
        
        /// <summary>
        /// 일시정지를 해제합니다.
        /// </summary>
        public void Resume()
        {
            State = EState.Running;
            _startTime = Time.time;
        }
        
        private async UniTask StartAsync()
        {
            if (_duration == 0)
            {
                return;
            }
            
            State = EState.Running;
            _startTime = Time.time;
            _accumulatedTime = 0.0f;
            
            while (State != EState.Stopped)
            {
                if (State == EState.Paused)
                {
                    await UniTask.Yield();
                    continue;
                }
                
                var elapsedTime = Time.time - _startTime * Speed;
                var totalElapsedTime = elapsedTime + _accumulatedTime;
                
                if (totalElapsedTime >= _duration)
                {
                    State = EState.Stopped;
                    continue;
                }
                
                await UniTask.Yield();
            }
        }
        
        public async UniTask WaitForStop()
        {
            while (State != EState.Stopped)
            {
                await UniTask.Yield();
            }
        }

        private void Accumulate()
        {
            var time = Time.time;
            _accumulatedTime += (time - _startTime) * Speed;
            _startTime = time;
        }
    }
}