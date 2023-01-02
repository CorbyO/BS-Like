using System;

namespace Corby.Frameworks
{
    public interface IPlayer
    {
        bool IsPlaying { get; }
        bool IsPaused { get; }
        bool IsLoop { get; }
        bool IsPlayOnAwake { get; }
        event Action OnPlay;
        event Action OnStop;
        event Action<bool> OnPause;
        void Play();
        void Pause();
        void Unpause();
        void Stop();
    }

    public static class PlayerExtensions
    {
        public static bool IsStopped(this IPlayer player)
        {
            return !player.IsPlaying;
        }
    }
}