using Corby.Frameworks;
using Corby.Frameworks.Attributes;
using UnityEngine;

namespace Corby.Apps.Actor
{
    public class ACharacter : AActor
    {
        [Bind("Sprite")]
        private SpriteRenderer _spriteRenderer;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        private float _speed;
        public bool IsMoving { get; private set; }
        
        /// <summary> Right: false, Left: true </summary>
        private bool _isFlip;

        public bool IsFlip
        {
            get => _isFlip;
            private set
            {
                if (_isFlip == value) return;
                _isFlip = value;
                _spriteRenderer.flipX = _isFlip;
            }
        }

        protected override void OnLoadedScript()
        {
            base.OnLoadedScript();
            _speed = 5f;
            _isFlip = false;
            IsMoving = false;
        }

        public void Move(Vector2 velocity)
        {
            var x = velocity.x;
            if(x == 0)
            {
                if (velocity.y == 0)
                {
                    IsMoving = false;
                    return;
                }
            }
            else IsFlip = x < 0;

            Position += velocity * (_speed * Time.deltaTime);
            IsMoving = true;
        }
    }
}