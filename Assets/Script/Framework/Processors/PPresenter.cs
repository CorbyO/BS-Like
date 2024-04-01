using System;
using System.Collections.Generic;
using Corby.Framework;
using Corby.Framework.UI;
using UnityEngine;

namespace RPG.Managers
{
    public class PPresenter : PPersistentProcessor<PPresenter>
    {
        public readonly List<(Presenter presenter, View view)> PresenterViewPairs = new(64);
        private Presenter[] _presenter;

        protected override void OnAwake()
        {
            base.OnAwake();

            foreach (var view in FindObjectsByType<View>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                var type = typeof(Presenter);
                var instance = (Presenter)Activator.CreateInstance(type);
                
                PresenterViewPairs.Add((instance, view));
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            foreach (var (presenter, view) in PresenterViewPairs)
            {
                presenter.Initialize(view);
            }
            
            PresenterViewPairs.Clear();
        }

        private void OnDestroy()
        {
            foreach (var t in _presenter)
            {
                t.Release();
            }

            _presenter = null;
        }
    }
}