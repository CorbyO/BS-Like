using Corby.Apps.Actor;
using Corby.Frameworks;
using Corby.Frameworks.Attributes;
using Script.Apps.ScriptableObjects;

namespace Corby.Apps.Processors
{
    public class PGameMode : PProcessor
    {
        public override bool IsDestroyWithScene => false;
        
        [Instancing("OriginalMonster")]
        private AMonster _originalMonster;
        
        private AMonster Create(MonsterVo vo)
        {
            var monster = Instantiate(_originalMonster);
            monster.Initialize(vo);
            return monster;
        }
    }
}