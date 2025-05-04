using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Codeabuse
{
    public static class MenuItems
    {
        [MenuItem("Tools/Request Script Recompilation", priority = 1000)]
        public static void RequestScriptRecompilation()
        {
            CompilationPipeline.RequestScriptCompilation();
        }

        [MenuItem("Tools/Reset PlayerPrefs")]
        public static void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}