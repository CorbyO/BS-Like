using System;
using System.Collections.Generic;
using Corby.Frameworks;
using Corby.Frameworks.Attributes;
using UnityEngine;

namespace Script.Apps.Components
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CFlipBooker : CComponent
    {
        [Bind]
        private CFlipBook[] _flipBooks;
        [SerializeField]
        private int _startPivot;

        public int Pivot { get; private set; }

        public delegate bool Filter();
        protected override void OnBound()
        {
            base.OnBound();
            foreach (var f in _flipBooks)
            {
                f.IsLoop = true;
                f.IsPlayOnAwake = false;
            }
        }

        public void Branch(int index, int targetIndex, Filter condition)
        {
            Assert(IsBound, "Please Call it in OnBound().");
            Assert(index < _flipBooks.Length, "Index is out of range.");
            Assert(targetIndex < _flipBooks.Length, "TargetIndex is out of range.");
            
            var current = _flipBooks[index];
            current.OnFrameChanged += _ =>
            {
                if (!condition.Invoke()) return;
                current.Stop();
                _flipBooks[targetIndex].Play();
            };

            current.OnLoopComplete += () =>
            {
                if (!condition.Invoke()) return;
                current.Stop();
                _flipBooks[targetIndex].Play();
            };
        }

        public void BranchOnStop(int index, int targetIndex)
        {
            Assert(IsBound, "Please Call it in OnBound().");
            Assert(index < _flipBooks.Length, "Index is out of range.");
            Assert(targetIndex < _flipBooks.Length, "TargetIndex is out of range.");
            
            var current = _flipBooks[index];
            current.OnStop += () =>
            {
                current.Stop();
                _flipBooks[targetIndex].Play();
            };
        }

        private void Start()
        {
            Pivot = _startPivot;
            _flipBooks[Pivot].Play();
        }

        public void Pause()
        {
            _flipBooks[Pivot].Pause();
        }
        
        public void Unpause()
        {
            _flipBooks[Pivot].Unpause();
        }
    }
}