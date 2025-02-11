using UnityEditor;
using UnityEditor.Compilation;

namespace Codeabuse
{
    public static class MenuItems
    {
        [MenuItem("Tools/Request Script Recompilation", priority = 1000)]
        public static void RequestScriptRecompilation()
        {
            CompilationPipeline.RequestScriptCompilation();
        }
    }
}