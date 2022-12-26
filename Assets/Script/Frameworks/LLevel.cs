using System;
using System.Collections.Generic;
using Corby.Frameworks.Attributes;
using Corby.Frameworks.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cysharp.Threading.Tasks.UniTask;

namespace Corby.Frameworks
{
    public abstract class LLevel : BaseBehavior
    {
        private List<PProcessor> _mutable;
        private static LLevel _instance;
        private static List<PProcessor> _immutable;
        private static GameObject _container;

        private Camera _mainCam;
        private Camera _uiCam;
        
        protected override void OnBindBefore()
        {
            _mutable = new List<PProcessor>();
            
            _instance = this;
            if (_container != null)
            {
                _immutable = new List<PProcessor>();
                _container = new GameObject("ImmutableContainer");
                DontDestroyOnLoad(_container);
            }
            
            DefineScreenAttribute.GetScreen(this);

            var temp = FindObjectsOfType<Camera>();
            if(temp.Length > 2) Debug.LogWarning("[LLevel] More than 2 cameras found.");
            foreach (var cam in temp)
            {
                if (cam.CompareTag("MainCamera"))
                {
                    _mainCam = cam;
                }
                else
                {
                    _uiCam = cam;
                }
            }
            
            AddProcessors();
        }

        protected override void OnBindAfter()
        {
        }

        protected abstract void AddProcessors();
        
        protected void AddProcessor<T>(out T created) where T : PProcessor
        {
            var instance = _container.AddComponent<T>();
            var container = instance.IsDestroyWithScene ? _mutable : _immutable;
            
            container.Add(instance);
            created = instance;
        }

        public static void ChangeScene(string sceneName, List<PProcessor> removeList)
        {
            foreach (var processor in _immutable)
            {
                processor.OnLevelChange();
            }
            
            foreach (var processor in removeList)
            {
                processor.Dispose();
                Destroy(processor);
            }
            removeList.Clear();
            
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        public static async UniTask ChangeSceneWithLoading(string sceneName, List<PProcessor> removeList)
        {
            ChangeScene("LoadingScene", removeList);
            
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = true;
            await asyncOperation.ToUniTask(new Progress<float>(x =>
            {
                Debug.Log(x);
            }));

            await SceneManager.UnloadSceneAsync("LoadingScene");
        }
    }
}