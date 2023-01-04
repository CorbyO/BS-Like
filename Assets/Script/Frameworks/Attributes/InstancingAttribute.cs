using System;
using System.Reflection;
using Corby.Frameworks.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Corby.Frameworks.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InstancingAttribute : Attribute
    {
        public string Name { get; private set; }

        public InstancingAttribute(string name)
        {
            Name = name;   
        }

        public static async UniTask Do(Component owner, string kind, bool isChild)
        {
            foreach (var tuple in owner.GetType().IterateAttributes<InstancingAttribute>())
            {
                var field = tuple.member as FieldInfo ?? throw new InvalidCastException();
                var attribute = tuple.attribute;
                var type = field.FieldType;
                
                var path = $"Assets/Prefabs/{kind}/{type.Name}.prefab";
                Debug.Log($"[InstancingAttribute] Load start: {path}");
                var obj = await Addressables.InstantiateAsync(path, isChild ? owner.transform : null);
                obj.name = attribute.Name;
                field.SetValue(owner, obj.GetComponent(type));
            }
        }
    }
}