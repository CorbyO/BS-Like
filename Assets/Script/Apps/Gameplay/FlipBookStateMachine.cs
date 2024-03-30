using System;
using System.Diagnostics;
using Corby.Framework;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Corby.Apps.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(FlipBook))]
    public class FlipBookStateMachine : BaseBehavior
    {
        [SerializeField] private FlipBook[] _flipBooks;
        [SerializeField] private int _startPivot;

        public int Pivot { get; private set; }

        public delegate bool Filter();

        protected override void OnStart()
        {
            Pivot = _startPivot;
            _flipBooks[Pivot].Play();
        }

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            foreach (var flipBook in _flipBooks)
            {
                flipBook.IsPlayOnAwake = false;
            }
            
            _flipBooks[_startPivot].IsPlayOnAwake = true;
        }

        public void Pause()
        {
            _flipBooks[Pivot].Pause();
        }

        public void Unpause()
        {
            _flipBooks[Pivot].Unpause();
        }

        public void Branch(int index, int targetIndex, Filter condition)
        {
            Debug.Assert(index < _flipBooks.Length, "Index is out of range.");
            Debug.Assert(targetIndex < _flipBooks.Length, "TargetIndex is out of range.");

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
            Debug.Assert(index < _flipBooks.Length, "Index is out of range.");
            Debug.Assert(targetIndex < _flipBooks.Length, "TargetIndex is out of range.");

            var current = _flipBooks[index];
            current.OnStop += () =>
            {
                current.Stop();
                _flipBooks[targetIndex].Play();
            };
        }
    }
}