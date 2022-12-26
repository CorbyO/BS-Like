using System;
using System.Reflection;
using Corby.Frameworks.Extensions;
using UnityEngine;

namespace Corby.Frameworks.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DefineScreenAttribute : Attribute
    {
        public static void GetScreen(LLevel owner)
        {
            foreach (var pair in owner.GetType().IterateAttributes<DefineScreenAttribute>())
            {
                if (pair.member is FieldInfo fieldInfo)
                {
                    var screenType = fieldInfo.FieldType;
                    
                    var canvas = GameObject.Find("Canvas");
                    var screen = canvas.AddComponent(screenType);
                    
                    fieldInfo.SetValue(owner, screen);
                    break;
                }
                
            }
        }
    }
}