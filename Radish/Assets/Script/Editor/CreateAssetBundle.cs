using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateAssetBundle : MonoBehaviour {


    [MenuItem("BuildAssetBundle/BuildForIos")]
    static void BuildAbsForIos() {
        string outPutPath = Application.streamingAssetsPath;
        BuildPipeline.BuildAssetBundles(outPutPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}
