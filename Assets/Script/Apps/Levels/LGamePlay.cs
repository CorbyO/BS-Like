using System;
using Corby.Apps.Processors;
using Corby.Frameworks;
using Corby.Frameworks.Attributes;
using Corby.UI.Screens;

namespace Corby.Apps.Level
{
    public class LGamePlay : LLevel
    {
        [DefineScreen]
        private SGamePlay _screen;

        private PGameReady _gameReady;
        private PPlayerController _playerController;
        
        protected override void AddProcessors()
        {
            AddProcessor(out _gameReady);
            AddProcessor(out _playerController);

            _screen.Handle = new()
            {
                OnInputDirection = _playerController.Input
            };
        }
    }
}