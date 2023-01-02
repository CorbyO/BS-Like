using Corby.Frameworks;

namespace Corby.Apps.Processors
{
    public class PGameReady : PProcessor
    {
        public override bool IsDestroyWithScene => false;
        public override void OnLevelChange()
        {
        }

        protected override void OnInstancing()
        {
        }
    }
}