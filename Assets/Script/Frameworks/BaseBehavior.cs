using System;
using System.Diagnostics;
using Corby.Frameworks.Attributes;
using UnityEngine;

namespace Corby.Frameworks
{
    public abstract class BaseBehavior : MonoBehaviour
    {
        protected abstract void OnLoadedScript();
        protected abstract void OnBound();

        protected bool IsBound { get; set; }

        private void Bind()
        {
            BindAttribute.Bind(this);
            IsBound = true;
            OnBound();
        }
        
        private void Awake()
        {
            IsBound = false;
            OnLoadedScript();
            Bind();
        }
        
        protected void Join(ref Action action, Action newAction)
        {
            action += () =>
            {
                newAction?.Invoke();
            };
        }
        
        [Conditional("CORBY_DEVELOPMENT")]
        protected void Log(string message)
        {
            Dbug.Log(GetType(), message, 2, name);
        }
        
        [Conditional("CORBY_DEVELOPMENT")]
        protected void Assert(bool condition, string message = null)
        {
            if (!condition)
            {
                Dbug.Error(GetType(), message ?? "Assertion failed.", 2, name);
            }
        }
        
        [Conditional("CORBY_DEVELOPMENT")]
        protected void Warning(string message)
        {
            Dbug.Warning(GetType(), message, 2, name);
        }
        
        [Conditional("CORBY_DEVELOPMENT")]
        protected void Error(string message)
        {
            Dbug.Error(GetType(), message, 2, name);
        }
    }
}
