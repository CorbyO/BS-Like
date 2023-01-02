using Corby.Frameworks.Attributes;
using Script.Apps.Components;
using UnityEngine;

namespace Corby.Apps.Actor
{
    public class APlayer : ACharacter
    {
        [Bind("Sprite")]
        private CFlipBooker _flipBooker; 
        private float _speed;
        
        private bool _isMoving;

        protected override void OnLoadedScript()
        {
            base.OnLoadedScript();
            _speed = 8;
            _isMoving = false;
        }
        
        protected override void OnBound()
        {
            base.OnBound();
            _flipBooker.Branch(0, 1, () =>
            {
                var temp = _isMoving;
                _isMoving = false;
                return temp;
            });
        }

        public void Move(Vector2 velocity)
        {
            Position += velocity * _speed * Time.deltaTime;
            _isMoving = true;
        }
    }
}