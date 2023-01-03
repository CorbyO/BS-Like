using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Text;
using Corby.Frameworks.Extensions;

namespace Corby.Frameworks.Attributes
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class BindAttribute : Attribute
    {
        private static readonly Type[] _types;
        private static readonly object[] _size;
        private enum EBindType
        {
            Single = 100,
            Array = 200,
        }
        private readonly EBindType _bindType;
        private string _path;
        private readonly string[] _paths;
        private readonly bool _canAdd;
        private readonly string _errorMessage;

        public BindAttribute(bool canAdd = false)
        {
            _bindType = EBindType.Single;
            _canAdd = canAdd;
        }

        public BindAttribute(string path, bool canAdd = false)
        {
            _path = path;
            _bindType = EBindType.Single;
            _canAdd = canAdd;
        }

        public BindAttribute(params string[] paths)
        {
            _paths = paths;
            _bindType = EBindType.Array;
        }

        public BindAttribute(string path, int startIndex, int size, bool canAdd = false)
        {
            if (startIndex < 0) _errorMessage = $"start Index must be greater than or equal to 0";
            if (size < 0) _errorMessage = $"size must be greater than or equal to 0";
            
            _paths = new string[size];
            for(int i = 0; i < size; i++)
            {
                _paths[i] = string.Format(path, startIndex + i);
            }

            _bindType = EBindType.Array;
            _canAdd = canAdd;
        }

        public BindAttribute(string path, Type enumType, bool canAdd = false)
        {
            if (!enumType.IsEnum) _errorMessage = "Got type is must be an enum";
            
            var enums = Enum.GetNames(enumType);
            _paths = new string[enums.Length];
            for(int i = 0 ; i < enums.Length; i++)
            {
                _paths[i] = string.Format(path, enums[i]);
            }

            _bindType = EBindType.Array;
            _canAdd = canAdd;
        }

        static BindAttribute()
        {
            _size = new object[] { 0 };
            _types = new[] { typeof(int) };
        }

        public static void Bind(Component owner)
        {
            Bind(owner, owner);
        }
        private static object Bind(Component root, object owner)
        {
            foreach (var member in GetMembers(owner))
            {
                var att = member.GetCustomAttribute<BindAttribute>(true);
                if (att == null) continue;
                var type = member.GetMemberType();
                if (att._errorMessage != null) 
                    throw new Exception($"[BindAttribute] \"{owner.GetType()}::({type}){member.Name}\" can not bind.\nDetail: {att._errorMessage}");

                if(ShouldFindBind(member.GetMemberType()))
                {
                    if(!member.TryGet(owner, out var value))
                        throw new Exception($"[BindAttribute] \"{owner.GetType()}::({type}){member.Name}\" must be not null");
                    // BindAttribute가 붙어있는데 관련 데이터가 없으면 재귀적으로 찾는다. (struct나 class로 인식)
                    var changedValue = Bind(root, value);
                    if (!member.TrySet(owner, changedValue, out var error))
                        throw new Exception($"[BindAttribute] \"{owner.GetType()}::({type}){member.Name}\" can not set value,\nDetail: {error}");
                }
                else att.BindMember(root, owner, member);
            }

            return owner;
        }

        /// <summary>
        /// BindAttribute가 붙어있는 멤버들을 찾고 컴포넌트를 찾아서 레퍼런싱 한다.
        /// </summary>
        /// <param name="root">BindAttribute가 있는 맴버들을 보유한 컴포넌트</param>
        /// <param name="owner">실제 맴버를 소유한 컴포넌트 (root내의 structure나 class를 위해)</param>
        /// <param name="memberInfo">owner가 보유하고 컴포넌트를 얻을 맴버</param>
        public void BindMember(Component root, object owner, MemberInfo memberInfo)
        {
            var t = root.transform;
            var type = memberInfo.GetMemberType();
            object value;
            switch (_bindType)
            {
                case BindAttribute.EBindType.Single: // 맴버가 리스트가 아닐때
                {
                    // _path가 null이면 자신에게 붙어있는 컴포넌트를 찾는다.
                    value = _path != null ?
                        AttachComponent(t, _path, type, _canAdd) :
                        AttachComponent(t, type, _canAdd);
                    if (value == null)
                        throw new Exception($"[BindAttribute] \"{owner.GetType()}::({type}){memberInfo.Name}\" can not attach \"{type}\" at \"{root.GetType()}/{_path}\".");
                }
                break;
                case BindAttribute.EBindType.Array: // 맴버가 리스트일때 리스트의 타입을 찾아서 리스트를 생성하고 리스트의 타입에 맞는 컴포넌트를 찾아서 리스트에 넣는다.
                {
                    Type eleType;
                    try
                    {
                        eleType = type.GetGenericArguments().Single();
                    } 
                    catch(InvalidOperationException e) { throw new Exception($"[BindAttribute] \"{owner.GetType()}::({type}){memberInfo.Name}\" must be an List<T> type.\ndetail: {e.Message}"); }
                    var size = _paths.Length;
                    var list = CreateList(type, size);
                    value = list ?? throw new Exception($"[BindAttribute] \"{owner.GetType()}::({type}){memberInfo.Name}\" can not create \"{type}\".");

                    for (int i = 0; i < size; i++)
                    {
                        _path = _paths[i];
                        var com = AttachComponent(t, _path, eleType, _canAdd);
                        if (com == null)
                            throw new Exception($"[BindAttribute] \"{owner.GetType()}::({eleType}){memberInfo.Name}[{i}]\" can not attach \"{eleType}\" at \"{root.GetType()}/{_path}\".");
                        list.Add(com);
                    }
                }
                break;
                default: 
                    throw new Exception($"[BindAttribute] unknown error at \"{_path}\" in \"{owner}\".");
            }

            if (!memberInfo.TrySet(owner, value, out var error))
                throw new Exception($"[BindAttribute] \"{owner.GetType()}::({type}){memberInfo.Name}\" can not set to \"{value}\" at \"{root.GetType()}/{_path}\".\ndetail: {error}");
        }
        private static IList CreateList(Type type, int size)
        {
            _size[0] = size;
            return type.GetConstructor(_types).Invoke(_size) as IList;
        }

        public static ReadOnlyCollection<MemberInfo> GetMembers(object owner)
        {
            var temp = new List<MemberInfo>(50);
            for(var type = owner.GetType(); type != null; type = type.BaseType)
            {
                temp.AddRange(type.GetMembers((BindingFlags)(-1)));
            }
            return temp.AsReadOnly();
        }

        private static object AttachComponent(Transform transform, Type type, bool canAdd)
        {
            if (type.IsArray)
            {
                var eleType = type.GetElementType();
                return transform.GetComponents(eleType);
            }
            var com = transform.GetComponent(type);
            if (com == null)
            {
                if (!canAdd) return null;
                com = transform.gameObject.AddComponent(type);
                Debug.Log($"[BindAttribute] \"{transform.name}\" add \"{type}\"");
            }

            return com;
        }

        private static object AttachComponent(Transform transform, string path, Type type, bool canAdd)
        {
            var found = transform?.Find(path);
            if(found == null) return null;

            return AttachComponent(found, type, canAdd);
        }

        private static bool ShouldFindBind(Type type)
        {
            return 
                ( // class 
                    type.IsClass && 
                    type != typeof(Delegate) &&
                    !type.IsSubclassOf(typeof(Component)) &&
                    !typeof(IList).IsAssignableFrom(type) &&
                    !typeof(IDictionary).IsAssignableFrom(type) &&
                    !type.IsArray
                ) 
                || 
                ( // structures
                    type.IsValueType && 
                    !type.IsPrimitive &&
                    !type.IsEquivalentTo(typeof(decimal)) &&
                    type != typeof(DateTime) &&
                    !type.IsEnum
                );
        }
    }
}