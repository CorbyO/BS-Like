using Corby.Apps.Actor;
using Corby.Frameworks;
using Corby.Frameworks.Attributes;
using UnityEngine;

namespace Corby.Apps.Processors
{
    public class PPlayerController : PProcessor
    {
        [Instancing("Player")]
        private APlayer _player;
        public ReadonlyTransform Player => _player.transform;

        public override bool IsDestroyWithScene => true;

        public void Input(Vector2 velocity)
        {
            _player.Move(velocity);
        }
    }
}