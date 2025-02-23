using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Codeabuse
{
    internal static class PackageSetup
    {
        private const string package_id = "net.codeabuse.unity-tools";
        private const string setup_done_key = "setup_done";
        private const string unitask_dotween_support_symbol = "UNITASK_DOTWEEN_SUPPORT";

        private static ListRequest _packagesRequest;
        
        [InitializeOnLoadMethod]
        private static void CheckDependencies()
        {
            _packagesRequest = Client.List();
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            switch (_packagesRequest.Status)
            {
                case StatusCode.InProgress:
                    return;
                case StatusCode.Success:
                    break;
                case StatusCode.Failure:
                    EditorApplication.update -= OnEditorUpdate;
                    Debug.LogError("Unity Tools: Can't determine Package Version - Package Manager List error");
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            EditorApplication.update -= OnEditorUpdate;
            
            string packageVersion = "0";
            foreach (var package in _packagesRequest.Result)
            {
                if (package.name != package_id)
                    continue;
                packageVersion = package.version;
                break;
            }

            _packagesRequest = null;

            if (Version.TryParse(ProjectPrefs.GetString(setup_done_key, "0"), out var lastVersion) &&
                Version.TryParse(packageVersion, out var currentVersion))
            {
                if (currentVersion.CompareTo(lastVersion) > 0)
                {
                    ProjectPrefs.SetString(setup_done_key, packageVersion);
                    PromptUserWithPackageInfo();
                }
            }

            CheckScriptingDefineSymbols();
        }

        private static void PromptUserWithPackageInfo()
        {
            // TODO: integrate more dependency cheks and show dedicated PackageInfoWindow with nicely formatted URLs.
#if (!UNITASK_ENABLED)
            {
                EditorUtility.DisplayDialog("Unity Tools package setup",
                        "UniTask integration is disabled. \n To enable it, install UniTask package via Package Manager" +
                        " using Git URL (see https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package for instructions).",
                        "Ok");
            }
#endif
        }

        private static void CheckScriptingDefineSymbols()
        {
#if UNITASK_ENABLED && DOTWEEN && !UNITASK_DOTWEEN_SUPPORT

            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var symbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget)
                   .Split(',', ' ')
                   .ToList();
            
            if (string.IsNullOrEmpty(symbols.FirstOrDefault(x => x == unitask_dotween_support_symbol)))
            {
                symbols.Add(unitask_dotween_support_symbol);
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, symbols.ToArray());
            }
#endif
        }
    }
}