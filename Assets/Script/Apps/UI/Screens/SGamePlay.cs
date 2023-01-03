using Corby.Frameworks;
using Corby.Frameworks.Attributes;
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
        [Instancing("Joystick")]
        private WJoystick _joystick;
        public HdSGamePlay Handle { get; set; }

        protected override async UniTask OnPostLoadedScript()
        {
            await base.OnPostLoadedScript();
            await WaitFor(_joystick);
            _joystick.OnControlled += vec => Handle.OnInputDirection?.Invoke(vec);
        }
    }
}