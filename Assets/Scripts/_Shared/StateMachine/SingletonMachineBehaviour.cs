using System;
using BW.Util;
using UnityEngine;

namespace BW.StateMachine
{
    public abstract class SingletonMachineBehaviour<T> : MachineBehaviour, ISingletonMonoBehaviour
        where T : MonoBehaviour
    {
        public static T Instance
        {
            get { return UnitySingleton<T>.GetSingleton(true, true); }
        }

        public static T DoesInstanceExist()
        {
            return UnitySingleton<T>.GetSingleton(false, false);
        }

        public static void ActivateSingletonInstance()
        {
            UnitySingleton<T>.GetSingleton(true, true);
        }

        public static void SetSingletonAutoCreate(GameObject autoCreatePrefab)
        {
            UnitySingleton<T>._autoCreatePrefab = autoCreatePrefab;
        }

        public static void SetSingletonType(Type type)
        {
            UnitySingleton<T>._myType = type;
        }

        private void Awake() // should be called in derived class
        {
            if (isSingletonObject)
            {
#if UNITY_FLASH
            UnitySingleton<T>._Awake( this );
#else
                UnitySingleton<T>._Awake(this as T);
#endif
                //Debug.Log( "Awake: " + this.GetType().Name );
            }
        }

        protected virtual void OnDestroy() // should be called in derived class
        {
            if (isSingletonObject)
            {
                UnitySingleton<T>._Destroy();
            }
        }

        public virtual bool isSingletonObject
        {
            get { return true; }
        }
    }
}