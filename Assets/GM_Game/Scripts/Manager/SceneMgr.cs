using System;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using GM;
using UnityEngine;
using YooAsset;
using static Common.ConstDefine;

namespace Manager
{
    /**
     * Title:场景管理器
     * Desciption:负责场景切换与离场景时的资源/对象池清理。
     **/
    public class SceneMgr : Singleton<SceneMgr>
    {
        /// <summary>
        /// 离开登录域：清空对象池并释放已缓存的 Prefab Handle。
        /// </summary>
        public void LeaveLoginScene()
        {
            GameObjectPoolMgr.Instance.ClearAllAndReset();
            ResourceMgr.Instance.ReleaseAll();
        }

        /// <summary>
        /// 加载创角场景（Single 模式），加载前会先执行 LeaveLoginScene。
        /// </summary>
        public void LoadCreateRoleScene(Action onSuccess, Action onFailed = null)
        {
            LoadSceneWithPathAsync(CREATE_ROLE_SCENE_PATH, onSuccess, onFailed, CancellationToken.None).Forget();
        }

        /// <summary>
        /// 加载指定路径的场景。
        /// </summary>
        /// <param name="path">场景路径</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onFailed">失败回调</param>
        /// <param name="cancellationToken">取消令牌</param>
        private async UniTaskVoid LoadSceneWithPathAsync(
            string path,
            Action onSuccess,
            Action onFailed,
            CancellationToken cancellationToken)
        {
            LeaveLoginScene();

            SceneOperationHandle handle = Global.Instance.YooPackage.LoadSceneAsync(path);

            await AwaitSceneHandleAsync(handle, cancellationToken);

            if (handle.Status != EOperationStatus.Succeed)
            {
                Debug.LogError($"[SceneMgr] 加载场景失败: {path}, {handle.LastError}");
                onFailed?.Invoke();
                return;
            }

            onSuccess?.Invoke();
        }

        /// <summary>
        /// 等待场景加载完成。
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async UniTask AwaitSceneHandleAsync(SceneOperationHandle handle, CancellationToken cancellationToken)
        {
            if (handle.IsDone)
            {
                return;
            }

            UniTaskCompletionSource completionSource = new UniTaskCompletionSource();
            handle.Completed += _ => completionSource.TrySetResult();
            await completionSource.Task.AttachExternalCancellation(cancellationToken);
        }
    }
}
