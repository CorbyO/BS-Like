using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Corby.Frameworks.Extensions
{
    public static class TypeExtensions
    {
        public static ReadOnlyCollection<MemberInfo> GetMembers(this Type owner)
        {
            var temp = new List<MemberInfo>(50);
            for(var type = owner; type != null; type = type.BaseType)
            {
                temp.AddRange(type.GetMembers((BindingFlags)(-1)));
            }
            return temp.AsReadOnly();
        }
        
        public static IEnumerable<MemberInfo> IterateMembers(this Type owner)
        {
            for(var type = owner; type != null; type = type.BaseType)
            {
                foreach(var member in type.GetMembers((BindingFlags)(-1)))
                {
                    yield return member;
                }
            }
        }
        
        public static IEnumerable<(MemberInfo member, T attribute)> IterateAttributes<T>(this Type owner) where T : Attribute
        {
            for(var type = owner; type != null; type = type.BaseType)
            {
                foreach(var member in type.GetMembers((BindingFlags)(-1)))
                {
                    if(member.IsDefined(typeof(T), true))
                    {
                        yield return (member, member.GetCustomAttribute<T>());
                    }
                }
            }
        }
    }
}