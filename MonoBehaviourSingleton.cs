using UnityEngine;
namespace Hiyazcool 
{
    namespace Unity
    {
        public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
        {
            protected bool isPersistent = true;
            public static T instance { get; protected set; }
            void Awake()
            {
                if (instance != null && instance != this)
                {
                    Destroy(this);
                    throw new System.Exception("An instance of this singleton already exists.");
                }
                else
                {
                    instance = (T)this;
                    if (isPersistent)
                        DontDestroyOnLoad((T)this);

                }
            }
        }
    }
}