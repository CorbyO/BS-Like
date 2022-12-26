using System.Collections;
using System.Collections.Generic;
using Corby.Frameworks.Attributes;
using UnityEngine;

namespace Corby.Frameworks
{
    public abstract class BaseBehavior : MonoBehaviour
    {
        protected abstract void OnBindBefore();
        protected abstract void OnBindAfter();

        private void Bind()
        {
            OnBindBefore();
            BindAttribute.Bind(this);
            OnBindAfter();
        }
        
        private void Awake()
        {
            Bind();
        }
    }
}
