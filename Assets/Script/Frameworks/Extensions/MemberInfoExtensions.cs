using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Unity.VisualScripting.FullSerializer.Internal;

namespace Corby.Frameworks.Extensions
{
    public static class MemberInfoExtensions
    {
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)memberInfo).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)memberInfo).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).PropertyType;
                default:
                    throw new ArgumentException
                    (
                    "[MemberInfoExtension] Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }

        public static object Get(this MemberInfo member, object owner)
        {
            switch(member.MemberType)
            {
                case MemberTypes.Field:
                    var field = member as FieldInfo;
                    return field.GetValue(owner);
                case MemberTypes.Property:
                    var property = member as PropertyInfo;
                    return property.GetValue(owner);
                default:
                    throw new ArgumentException
                    (
                    "[MemberInfoExtension] Input MemberInfo must be if type FieldInfo, PropertyInfo"
                    );
            }
        }

        public static bool TryGet(this MemberInfo member, object owner, out object value)
        {
            try
            {
                value = member.Get(owner);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        public static void Set(this MemberInfo member, object owner, object value)
        {
            Action<object, object> SetValue;
            Type memberType;
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                {
                    var i = (FieldInfo)member;
                    SetValue = i.SetValue;
                    memberType = i.FieldType;
                    break;
                }
                case MemberTypes.Property:
                {
                    var i = (PropertyInfo)member;
                    SetValue = i.SetValue;
                    memberType = i.PropertyType;
                    break;
                }
                default:
                    throw new ArgumentException(
                        "[MemberInfoExtension] Input MemberInfo must be if type FieldInfo, PropertyInfo");
            }

            if (memberType.IsArray)
            {
                var array = (Array)value;
                var length = array.Length;
                var @ref = (Array)memberType.GetDeclaredConstructor(new[]{ typeof(int) })
                    .Invoke(new object[]{ length });
                
                Array.Copy(array, @ref, length);
                SetValue(owner, @ref);
            }
            else
            {
                SetValue(owner, value);
            }
        }

        public static bool TrySet(this MemberInfo member, object owner, object value, out string detail)
        {
            try
            {
                member.Set(owner, value);
                detail = null;
                return true;
            }
            catch(Exception e)
            {
                detail = e.Message;
                return false;
            }
        }
    }
}