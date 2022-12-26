using Corby.Frameworks;
using Corby.Frameworks.UI;
using Corby.UI.Widgets;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Corby.UI.Screens
{
    public class HdSGamePlay : IHandle
    {
        public Signal<Vector2> OnInputDirection;
    }
    public class SGamePlay : SScreen, IHandleable<HdSGamePlay>
    {
        private WidgetBox<WJoystick> _joystick;
        public bool IsInitialized { get; private set; }
        public HdSGamePlay Handle { get; set; }

        protected override void Load()
        {
            ReserveLoad(_joystick);
        }

        protected override void OnLoad()
        {
            _joystick.Instance.OnControlled += v => Handle?.OnInputDirection?.Invoke(v);
        }
    }
}