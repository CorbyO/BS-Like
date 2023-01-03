using System;
using Corby.Apps.Processors;
using Corby.Frameworks;
using Corby.Frameworks.Attributes;
using Corby.UI.Screens;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Corby.Apps.Level
{
    public class LGamePlay : LLevel
    {
        [DefineScreen]
        private SGamePlay _screen;

        private PGameReady _gameReady;
        private PPlayerController _playerController;
        private PCameraExtension _cameraExtension;
        
        protected override void AddProcessors()
        {
            AddProcessor(out _gameReady);
            AddProcessor(out _playerController);
            AddProcessor(out _cameraExtension);

            _screen.Handle = new()
            {
                OnInputDirection = _playerController.Input
            };
            
        }

        protected override async UniTask OnPostLoadedScript()
        {
            await base.OnPostLoadedScript();
            await WaitFor(_playerController);
            _cameraExtension.Initialize(Camera.main, _playerController.Player);
        }
    }
}