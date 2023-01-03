using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Corby.Frameworks
{
    public class ReadonlyTransform
    {
        private readonly Transform _transform;
        
        public string Name => _transform.name;
        public ReadonlyTransform Parent => new ReadonlyTransform(_transform.parent);
        public ReadonlyTransform Root => new ReadonlyTransform(_transform.root);
        public Vector3 Position => _transform.position;
        public Vector3 LocalPosition => _transform.localPosition;
        public Vector3 Scale => _transform.lossyScale;
        public Vector3 LocalScale => _transform.localScale;
        public Quaternion Rotation => _transform.rotation;
        public Quaternion LocalRotation => _transform.localRotation;

        public ReadonlyTransform(Transform transform)
        {
            _transform = transform;
        }

        public static implicit operator ReadonlyTransform([NotNull] Transform transform)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            return new ReadonlyTransform(transform);
        }
    }
}