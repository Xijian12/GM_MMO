using GM;
using Manager;
using System.Collections;
using System.Threading;
using UnityEngine;
using YooAsset;

namespace Test
{
    /**
 	* Title:
 	* Desciption:
 	**/
    public class Test : MonoBehaviour
    {
        private ResourcePackage _package;

        public ResourcePackage YooPackage { get => _package; }

        private void Awake()
        {
            // 初始化YooAsset
            InitYooAsset();
        }

        private void InitYooAsset()
        {
            // 初始化资源系统
            YooAssets.Initialize();

            // 创建默认的资源包
            _package = YooAssets.CreatePackage("DefaultPackage");

            // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
            YooAssets.SetDefaultPackage(_package);
            // 初始化资源包
            StartCoroutine(InitPackage());
        }

        private IEnumerator InitPackage()
        {
            EditorSimulateModeParameters editorParameters = new EditorSimulateModeParameters();
            editorParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
            InitializationOperation operation = _package.InitializeAsync(editorParameters);

            gameObject.AddComponent<Global>();

            //等待初始化完成..
            yield break;

        }

    }
}
