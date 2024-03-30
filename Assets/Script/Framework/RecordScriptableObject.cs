using UnityEngine;

namespace Corby.Framework
{
    public abstract class RecordScriptableObject : ScriptableObject
    {
        [SerializeField] private int _key;
        
        public int Key => _key;
    }
}