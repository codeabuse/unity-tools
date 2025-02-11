using UnityEditor;

namespace Codeabuse
{
    public static class PackageSetup
    {
        [InitializeOnLoadMethod]
        private static void CheckDependencies()
        {
            #if (!UNITASK_ENABLED)
            {
                EditorUtility.DisplayDialog("Unity Tools package setup",
                        "UniTask integration is disabled. \n To enable it, install UniTask package via Package Manager" +
                        " using Git URL (see https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package for instructions).",
                        "Ok");
            }
            #endif
        }
    }
}