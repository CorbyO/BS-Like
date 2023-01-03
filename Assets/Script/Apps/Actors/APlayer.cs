using System;
using Corby.Frameworks.Attributes;
using Cysharp.Threading.Tasks;
using Script.Apps.Components;
using UnityEngine;

namespace Corby.Apps.Actor
{
    public class APlayer : ACharacter
    {
        [Bind("Sprite")]
        private CFlipBooker _flipBooker;

        protected override async UniTask OnPostLoadedScript()
        {
            await base.OnPostLoadedScript();
            await WaitFor(_flipBooker);
            _flipBooker.Branch(0, 1, () => IsMoving);
            _flipBooker.Branch(1, 0, () => !IsMoving);
        }
    }
}