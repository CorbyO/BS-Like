using Corby.Framework;
using UnityEngine;

namespace Corby.Apps.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Monster Table", menuName = "ScriptableObjects/Monster/Table")]
    public class MonsterTable : TableScriptableObject<MonsterTable, MonsterRecord>
    {
        
    }
}