using System.Collections.Generic;
using Corby.Option;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Corby.Framework.Processors
{
    public class PDatabase : PPersistentProcessor<PDatabase>
    {
        [SerializeField]
        [AssetList(AutoPopulate = true, Path = "ScriptableObjects/Tables")]
        private List<TableScriptableObject> _tables = new();
        
        private readonly Dictionary<string, TableScriptableObject> _tableMap = new();
        
        private int _indexingPivot = 0;

        protected override void OnAwake()
        {
            base.OnAwake();

            var count = _tables.Count;
            for (var i = _indexingPivot; i < count; i++)
            {
                var table = _tables[i];
                _tableMap.Add(table.GetType().Name, table);
            }
            
            _indexingPivot = count;
        }

        public Option<T> Get<T>()
            where T : TableScriptableObject
        {
            if (_tableMap.TryGetValue(typeof(T).Name, out var table))
            {
                return (table as T).ToOption();
            }
            
            var count = _tables.Count;
            for (var i = _indexingPivot; i < count; i++)
            {
                var newTable = _tables[i];
                _tableMap.Add(newTable.GetType().Name, newTable);

                if (newTable is not T castedTable) continue;
                
                _indexingPivot = i + 1;
                return castedTable.ToOption();
            }

            return None<T>.New;
        }
        
    }
}