using Corby.Framework;
using UnityEngine;

namespace Corby.Apps.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Monster Record", menuName = "ScriptableObjects/Monster/Record")]
    public class MonsterRecord : RecordScriptableObject
    {
        [SerializeField] private Sprite[] _animation;
        [SerializeField] private int _health;
        [SerializeField] private int _attack;
        [SerializeField] private int _speed;
        
        public Sprite[] Animation => _animation;
        public int Health => _health;
        public int Attack => _attack;
        public int Speed => _speed;
    }
}