using UnityEditor;

public class BuildAssetBundles
{
    [MenuItem("Assets/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/04. Images/[Object]/[Main]/Race/Atlas", BuildAssetBundleOptions.None, BuildTarget.Android);
    }
	
}
