using Corby.Frameworks;
using Corby.Frameworks.Attributes;
using Script.Apps.Components;
using Script.Apps.ScriptableObjects;
using Unity.VisualScripting;

namespace Corby.Apps.Actor
{
    public class AMonster : ACharacter, IInitializable<MonsterVo>
    {
        [Bind("Sprite")]
        private CFlipBook _flipBook;
        private MonsterVo _monsterVo;
        public bool IsInitialized { get; private set; }
        
        public void Initialize()
        {
            IsInitialized = true;
        }
        public void Initialize(MonsterVo mobVo)
        {
            _monsterVo = mobVo;
            _flipBook.SetSprites(mobVo.Animation);
            Initialize();
            _flipBook.Play();
        }
    }
}