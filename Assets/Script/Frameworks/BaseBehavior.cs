using System;
using System.Diagnostics;
using Corby.Frameworks.Attributes;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Corby.Frameworks
{
    public abstract class BaseBehavior : MonoBehaviour
    {
        protected abstract void OnLoadedScript();

        protected virtual void OnBound() { }

        protected virtual async UniTask OnPostLoadedScript() => await UniTask.CompletedTask;

        private bool _isLoaded;

        private void Bind()
        {
            BindAttribute.Bind(this);
            OnBound();
        }
        
        private void Awake()
        {
            _isLoaded = false;
            OnLoadedScript();
            Bind();
            OnPostLoadedScript().ContinueWith(() => _isLoaded = true).Forget();
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

        protected UniTask WaitFor(BaseBehavior behavior)
        {
            return UniTask.WaitUntil(() => behavior._isLoaded);
        }
    }
}
