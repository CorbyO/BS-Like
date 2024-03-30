using System;
using System.Collections.Generic;
using Corby.Framework.Processors;
using Corby.Option;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Corby.Framework
{
    public abstract class TableScriptableObject : SerializedScriptableObject
    {
    }
    
    public abstract class TableScriptableObject<TThis, TRecord> : TableScriptableObject
        where TThis : TableScriptableObject<TThis, TRecord>
        where TRecord : RecordScriptableObject
    {
        [Serializable]
        public sealed class Key
        {
            /// <summary>
            /// 키
            /// </summary>
            [SerializeField]
            private int _key;

            /// <summary>
            /// 키의 값
            /// </summary>
            public int Value => _key;
            /// <summary>
            /// 해당 키에 대한 레코드를 가져옵니다.
            /// </summary>
            public Option<TRecord> Record => PDatabase.Instance.Get<TThis>().Unwrap() switch
            {
                Some<TThis> some => some.Value.Get(_key),
                None<TThis> => None<TRecord>.New
            };
        }
        
        [SerializeField] private Dictionary<int, TRecord> _records;
        
        public TRecord this[int index] => _records[index];
        
        public bool TryAdd(int index, TRecord field)
        {
            return _records.TryAdd(index, field);
        }
        
        public Option<TRecord> Get(int index)
        {
            _records.TryGetValue(index, out var field);
            return field.ToOption();
        }
    }
}