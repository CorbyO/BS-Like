using UnityEngine;

namespace Script.Apps.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Monster VO", menuName = "ScriptableObjects/Monster VO")]
    public class MonsterVo : ScriptableObject
    {
        [SerializeField] private string _key;
        [SerializeField] private Sprite[] _animation;
        [SerializeField] private int _health;
        [SerializeField] private int _attack;
        [SerializeField] private int _speed;
        
        public string Key => _key;
        public Sprite[] Animation => _animation;
        public int Health => _health;
        public int Attack => _attack;
        public int Speed => _speed;
    }
}