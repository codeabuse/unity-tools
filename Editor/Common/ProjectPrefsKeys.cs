using UnityEngine;

namespace Codeabuse
{
    public static class ProjectPrefsKeys
    {
        public const string EDITOR_PLAY_MODE_KEY = "_editor_play_mode";
        
        private static readonly string project_prefix = $"{Application.productName}+{Application.unityVersion}";
        
        internal static string ToProjectSpecificKey(string key) => $"{project_prefix}.{key}";
    }
}