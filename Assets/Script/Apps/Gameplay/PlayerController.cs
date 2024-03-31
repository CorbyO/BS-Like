using System;
using Corby.Framework;
using UnityEngine;

namespace Corby.Apps.Gameplay
{
    /// <summary>
    /// top-down 방식의 플레이어 컨트롤러
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : BaseBehavior
    {
        [SerializeField] private float _speed = 5f;
        private Rigidbody2D _rigidbody2D;
        
        protected override void OnAwake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        protected override void OnStart()
        {
            _rigidbody2D.gravityScale = 0;
        }

        // private void FixedUpdate()
        // {
        //     _rigidbody2D.velocity = move * _speed;
        // }
    }
}