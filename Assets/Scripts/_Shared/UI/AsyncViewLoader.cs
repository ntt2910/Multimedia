using System;
using BW.Pools;
using BW.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace BW.UI
{
    public class AsyncViewLoader : MonoBehaviour
    {
        [SerializeField] private AssetReference viewAssetReference;

#if UNITY_EDITOR

        private void Start()
        {
            SceneVisibilityManager.instance.Hide(gameObject, true);
        }

        [Button]
        void SelectAsset()
        {
            Selection.activeObject =
                AssetDatabase.LoadMainAssetAtPath(
                    AssetDatabase.GUIDToAssetPath(this.viewAssetReference.AssetGUID));
        }

#endif

        private Action<AsyncOperationStatus, GameObject> OnLoadCompleted;

        public void SetLoadAction(Action<AsyncOperationStatus, GameObject> action)
        {
            OnLoadCompleted = action;
        }

        public bool IsLoaded { private set; get; }

        public void Load()
        {
            this.viewAssetReference.LoadAssetAsync<GameObject>().Completed += OnViewLoaded;
        }

        public void Unload()
        {
            if (IsLoaded)
            {
                IsLoaded = false;
                this.viewAssetReference.ReleaseAsset();
            }
        }

        private void OnViewLoaded(AsyncOperationHandle<GameObject> operationHandle)
        {
            GameObject viewGo = null;

            if (operationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Assert.IsNotNull(operationHandle.Result);
                IsLoaded = true;
                viewGo = ServiceLocator.GetService<IPoolService>().Spawn(operationHandle.Result);
                ParentingView(viewGo.transform.rectTransform(), transform.rectTransform());
            }

            OnLoadCompleted?.Invoke(operationHandle.Status, viewGo);
        }

        public void ParentingView(RectTransform viewTrans, RectTransform root)
        {
            viewTrans.SetParentUI(root);
            viewTrans.offsetMax = Vector2.zero;
            viewTrans.offsetMin = Vector2.zero;
            viewTrans.pivot = new Vector2(0.5f, 0.5f);
            viewTrans.anchorMin = Vector2.zero;
            viewTrans.anchorMax = Vector2.one;
        }
    }
}