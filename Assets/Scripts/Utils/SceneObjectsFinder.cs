using System;
using UnityEngine.SceneManagement;

namespace UnfrozenTestWork
{
    public static class SceneObjectsFinder
    {
        //Finds first root component of type <T> in given scene
        public static T FindFirstInRoot<T>(Scene scene) where T : class
        {
            if (scene == null)
            {
                throw new InvalidOperationException("[SceneObjectsFinder] Cannot find object because scene is null.");
            }
            var rootGOs = scene.GetRootGameObjects();
            foreach (var rootGO in rootGOs)
            {
                var component = rootGO.GetComponentInChildren<T>(true);
                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }
    }
}