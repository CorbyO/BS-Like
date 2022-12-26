using Corby.Frameworks;

namespace Corby.Apps.Processors
{
    public class PGameReady : PProcessor
    {
        public override bool IsDestroyWithScene => false;
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