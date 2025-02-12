using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Codeabuse
{
    public static class EditorHelpers
    {
        public static bool EnsureAssetsFolderCreated(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (!path.StartsWith("Assets/"))
                path = "Assets/" + path;
            
            if (AssetDatabase.IsValidFolder(path)) 
                return true;
            
            var folders = path.Split('/');
            var index = 0;
            string parent = null;
            string child = null;
            StringBuilder childPath = new();
            StringBuilder parentPath = new();
            while (index + 1 < folders.Length)
            {
                parent = folders[index];
                child = folders[++index];
                parentPath.Append(parent).Append('/');
                childPath.Clear().Append(parentPath).Append('/').Append(child);
                if (AssetDatabase.IsValidFolder(childPath.ToString()))
                    continue;
                parent = parentPath.Remove(parentPath.Length -1, 1).ToString();
                AssetDatabase.CreateFolder(parent, child);
                Debug.Log($"Folder created: {parentPath}{child}");
            }

            return true;
        }

        public static bool TryGetFirstAssetPath(string assetType, out string path)
        {
            var guid = AssetDatabase.FindAssets($"t:{assetType}");
            path = guid.Length == 0 ? null : AssetDatabase.GUIDToAssetPath(guid[0]);
            return path is not null;
        }

        public static bool TryGetAllAssetsPaths(string fileName, out IEnumerable<string> paths)
        {
            var guids = AssetDatabase.FindAssets($"t:{fileName}");
            paths = guids.Select(AssetDatabase.GUIDToAssetPath);
            return paths.Any();
        }
    }
}