using UnityEngine;

namespace Corby.Frameworks
{
    public static class Ref
    {
        public static Camera MainCamera { get; private set; }
        public static Camera UICamera { get; private set; }
        
        public static void Init()
        {
            InitCams();
        }

        private static void InitCams()
        {
            var temp = GameObject.FindObjectsOfType<Camera>();
            if(temp.Length > 2) Debug.LogWarning("[LLevel] More than 2 cameras found.");
            foreach (var cam in temp)
            {
                if (cam.CompareTag("MainCamera"))
                {
                    MainCamera = cam;
                }
                else
                {
                    UICamera = cam;
                }
            }
        }
    }
}