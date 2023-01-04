using System;
using System.Linq;
using System.Reflection;
using Corby.Frameworks.Extensions;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.Windows;
using Object = UnityEngine.Object;

namespace Corby.Frameworks.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AddressAttribute : Attribute
    {
        public string Key { get; private set; }
        
        public AddressAttribute(string key)
        {
            Key = key;
        }

        public static async UniTask Do(Component owner)
        {
            foreach (var tuple in owner.GetType().IterateAttributes<AddressAttribute>())
            {
                var field = tuple.member as FieldInfo ?? throw new InvalidCastException();
                var attribute = tuple.attribute;
                var type = field.FieldType;
                if (type.IsArray)
                {
                    var list = await Addressables.LoadAssetsAsync<Object>(attribute.Key, null);
                    var count = list.Count;
                    var elementType = type.GetElementType();
                    var array = Array.CreateInstance(elementType, count);
                    Array.Copy(list.ToArray(), array, count);
                    field.SetValue(owner, array);
                }
                else
                {
                    var obj = await Addressables.LoadAssetAsync<Object>(attribute.Key);
                    Debug.Log($"[AddressAttribute] Load complete: {obj.name}");

                    field.SetValue(owner, obj);
                }

            }
        }
    }
}