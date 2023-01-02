using System;
using Corby.Frameworks;
using Corby.Frameworks.Attributes;
using Corby.Frameworks.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Corby.UI.Widgets
{
    public class WJoystick : WWidget, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        [Bind("Background")]
        private Image _background;
        [Bind("Background/Controller")]
        private Image _controlled;
        private Vector2 _startPos;
        
        private bool _isPressed;
        private Vector2 _velocity;

        public event Action<Vector2> OnControlled;

        protected override void InitializeValues()
        {
            base.InitializeValues();

            _background.gameObject.SetActive(false);
        }
        
        private void Update()
        {
            if (_isPressed)
            {
                OnControlled?.Invoke(_velocity);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _background.gameObject.SetActive(true);
            _startPos = eventData.pressPosition;
            _background.rectTransform.anchoredPosition = _startPos;
            _controlled.rectTransform.anchoredPosition = Vector2.zero;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _background.gameObject.SetActive(false);
            _isPressed = false;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            var delta = eventData.position - _startPos;
            var length = delta.magnitude;
                    
            //  0~300 => 0~1
            _velocity = delta.normalized * Mathf.Clamp01(length / 300f);
            _controlled.rectTransform.anchoredPosition = _velocity * 100f;
            _isPressed = true;
        }
    }
}