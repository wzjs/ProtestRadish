using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class AssetBundleMgr : MonoBehaviour {

    string Win_streamingAssetsPath;

    Dictionary<string, GameObject> prefebsDict = new Dictionary<string, GameObject>();
    void Awake()
    {
        //动态下载AssetBundle的目录
        Win_streamingAssetsPath = "file://" + Application.streamingAssetsPath + "/";
    }

    //加载制定的AB包
    GameObject temp; 
    IEnumerator LoadSpecialAbs(string path) {
        WWW w = WWW.LoadFromCacheOrDownload(path, 0);
        yield return w;
        Object[] _obj = w.assetBundle.LoadAllAssets();
        foreach (var item in _obj)
        {
            temp = item as GameObject;
            Instantiate(temp);
        }
    }

    List<string> _abName = new List<string>(); //清单中获取资源包名
    List<string> _abDependName = new List<string>(); //依赖包名
    Dictionary<string, AssetBundle> _allAbs = new Dictionary<string, AssetBundle>();
    
    //加载全部包
    AssetBundle _Dep_ab; //临时依赖包对象

    bool _IsFull = false;
    IEnumerator LoadAbMainFest(string path)
    {
        WWW bundle = WWW.LoadFromCacheOrDownload(path, 0);
        yield return bundle;
        AssetBundle _main_ab = bundle.assetBundle;
        AssetBundleManifest aMList = (AssetBundleManifest)_main_ab.LoadAsset("AssetBundleManifest");
        _main_ab.Unload(false);

        _abName.Clear();
        
        for (int i = 0; i < aMList.GetAllAssetBundles().Length; i++) {
            _abName.Add(aMList.GetAllAssetBundles()[i]);
            Debug.Log(_abName[i]);
            Debug.Log(aMList.GetAllAssetBundles().Length);
            _abDependName = aMList.GetAllDependencies(aMList.GetAllAssetBundles()[i]).ToList();

            if (_abDependName.Count > 0) {
                for (int j = 0; j < _abDependName.Count; j++) {
                    if (_allAbs.ContainsKey(_abDependName[j])) {
                        continue;
                    }
                    string Depab_Path = string.Format("{0}{1}", Win_streamingAssetsPath, _abDependName[j]);
                    WWW dep_bundle = WWW.LoadFromCacheOrDownload(Depab_Path, 0);
                    yield return dep_bundle;
                    _Dep_ab = dep_bundle.assetBundle;
                    if (_Dep_ab != null) {
                        _allAbs.Add(_abDependName[j], _Dep_ab);
                    }
                }

            }
        }
        _IsFull = true;
        //asdf();
        //LoadMultiAbs();
    }

    //IEnumerator asdf() {
    //    Debug.Log("wrty");
    //    yield return null;
    //}
    public const string str = "main_menu";
    public const string str1 = "StreamingAssets";
    GameObject tempObj;


    AssetBundle _ab;
    IEnumerator LoadMultiAbs() {
        while (!_IsFull) {
            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < _abName.Count; i++) {
            if (_allAbs.ContainsKey(_abName[i])) {
                continue;
            }
            string ab_path = string.Format("{0}{1}", Win_streamingAssetsPath, _abName[i]);
            WWW bundle = WWW.LoadFromCacheOrDownload(ab_path, 0);
            yield return bundle;
            _ab = bundle.assetBundle;
            if (_ab != null) {
                _allAbs.Add(_abName[i], _ab);
            }
        }
        Debug.Log(_allAbs[str]);
        Object[] obj = _allAbs[str].LoadAllAssets();
        foreach (var item in obj)
        {
            temp = item as GameObject;
            Instantiate(temp);
        }
    }

    //TEST
    
    void test() {
        StartCoroutine(LoadAbMainFest(Win_streamingAssetsPath + str1));
        StartCoroutine(LoadMultiAbs());
    }

    private void Start()
    {
        test();
    }
}
