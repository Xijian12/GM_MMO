using UnityEngine;
using YooAsset;

/**
 * Title:
 * Desciption:
 **/
public class Global : MonoBehaviour
{
    public static Global Instance { get; private set; }
    private ResourcePackage _package;
    public ResourcePackage Yoopackage{get=>_package;}

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);

        _package = YooAssets.GetPackage("DefaultPackage");
    }
}
