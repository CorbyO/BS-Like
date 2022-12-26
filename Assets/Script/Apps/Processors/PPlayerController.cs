using Corby.Apps.Actor;
using Corby.Frameworks;

namespace Corby.Apps.Processors
{
    public class PPlayerController : PProcessor
    {
        private ACharacter _character;

        public override bool IsDestroyWithScene => true;
        public override void OnLevelChange()
        {
            
        }

        protected override void Load()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnLoad()
        {
            throw new System.NotImplementedException();
        }
    }
}