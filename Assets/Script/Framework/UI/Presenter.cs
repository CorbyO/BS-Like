using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Corby.Framework.UI
{
    [Serializable]
    public abstract class Presenter
    {
        [ShowInInspector]
        public string Name => GetType().Name;
        private View _viewInternal;
        private Stack<IDisposable> _disposables = new();
        
        protected View ViewInternal { get => _viewInternal; private set => _viewInternal = value; }
        
        /// <summary>
        /// 초기화(<see cref="Initialize"/>)후 호출되는 메서드<br/>
        /// 호출 시점은 Start와 같음
        /// </summary>
        public abstract void OnInitialize();
        /// <summary>
        /// 해제(<see cref="Release"/>)후 호출되는 메서드
        /// </summary>
        public abstract void OnRelease();
        
        /// <summary>
        /// 객체가 생성되고 나서 호출되는 메서드<br/>
        /// 이 함수를 직접 호출하지 마세요.
        /// </summary>
        public void Initialize(View view)
        {
            _viewInternal = view;
            if (_viewInternal == null)
            {
                Debug.LogWarning(@$"'{GetType().Name}' 프레젠터가 View({nameof(View)})을 찾을 수 없습니다.
View가 없어서 프레젠터가 작동 하지 않습니다.");
                return;
            }
            
            OnInitialize();
        }
        
        /// <summary>
        /// 객체가 소멸되기전 호출되는 메서드<br/>
        /// 이 함수를 직접 호출하지 마세요.
        /// </summary>
        public virtual void Release()
        {
            if (_viewInternal == null) return;
            
            OnRelease();
            
            if (_disposables == null) return;
            
            while (_disposables.TryPop(out var disposable))
            {
                disposable.Dispose();
            }
        }
        
        /// <summary>
        /// <see cref="IDisposable"/>을 추가합니다.<br/>
        /// </summary>
        /// <param name="disposable"></param>
        public void AddDisposable(IDisposable disposable)
        {
            _disposables ??= new();
            _disposables.Push(disposable);
        }
    }

    /// <summary>
    /// MV(R)P 패턴의 P(Presenter)에 해당하는 클래스<br/>
    /// <see cref="Manager"/>
    /// </summary>
    /// <typeparam name="TView">MV(R)P 패턴의 V(View)에 해당하는 클래스</typeparam>
    [Serializable]
    public abstract class Presenter<TView> : Presenter
        where TView : View
    {
        public TView View => (TView)ViewInternal;
    }
}