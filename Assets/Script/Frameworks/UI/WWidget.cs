using System;
using System.Collections.Generic;
using Corby.Frameworks.Attributes;
using UnityEngine;

namespace Corby.Frameworks.UI
{
    public class WWidget : BaseBehavior, IInitializable
    {
        private WWidget _parent;
        private List<WWidget> _children;
        
        [Bind]
        private RectTransform _rectTransform;
        protected RectTransform RectTransform => _rectTransform;

        public bool IsInitialized { get; private set; }

        protected override void OnBindAfter()
        {
            LoadResource();
            InitializeValues();
        }

        protected override void OnBindBefore()
        {
            SetEvents();
        }

        protected virtual void InitializeValues()
        {
            IsInitialized = true;
        }
        protected virtual void LoadResource() {}

        protected virtual void SetEvents() {}

        public void Initialize()
        {
            OnInitialized();
        }
        public virtual void OnInitialized()
        {
        }
    }
}