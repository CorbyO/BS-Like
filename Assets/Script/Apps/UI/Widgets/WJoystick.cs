using System;
using Corby.Frameworks;
using Corby.Frameworks.Attributes;
using Corby.Frameworks.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Corby.UI.Widgets
{
    public class WJoystick : WWidget
    {
        [Bind("Controller")]
        private Image _controlledImage;

        private Vector2 _initPos;
        private Vector2 _startPos;
        private bool _isTouching;

        public event Action<Vector2> OnControlled;

        protected override void InitializeValues()
        {
            base.InitializeValues();
            var worldPosition = RectTransform.position;
            _initPos = Ref.MainCamera.WorldToScreenPoint(worldPosition);
        }

        private void Update()
        {
            #if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                Vector2 mousePos = Input.mousePosition;
                if (_isTouching == false) // 누르기 시작
                {
                    _startPos = mousePos;
                    RectTransform.position = _startPos;
                    _isTouching = true;
                }
                else
                {
                    OnControlled?.Invoke((_startPos - mousePos).normalized);
                }
            }
            else
            {
                RectTransform.position = _initPos;
                _isTouching = false;
            }
            #endif
        }
    }
}