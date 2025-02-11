using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;

namespace Codeabuse
{
    public static class MeshTools
    {
        [MenuItem("Tools/Combine Selected Meshes")]
        public static void CombineSelectedMeshes()
        {
            var selectedObjects = 
                SelectionTools.FilterObjects(Selection.gameObjects, SelectionTools.ObjectFilter.SceneObjects).ToArray();
            if (selectedObjects.Length == 0)
            {
                Debug.Log("MeshTools: no scene objects was selected");
                return;
            }
            var combinedMeshObject = new GameObject("Combined Mesh");

            Undo.RegisterCreatedObjectUndo(combinedMeshObject, "Combine meshes");
            
            var selectionParent = selectedObjects.Length == 1
                    ? selectedObjects[0]
                    : selectedObjects.FirstOrDefault(x => x.transform.parent != null);
            
            if (selectionParent)
            {
                combinedMeshObject.transform.SetParent(selectionParent.transform);
                combinedMeshObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
            else
            {
                combinedMeshObject.transform.SetPositionAndRotation(
                        VectorTools.GetAverage(selectedObjects.Select(go => go.transform.position)), 
                        Quaternion.identity);
            }
            
            var combinedMesh = CombineMeshes(selectedObjects, combinedMeshObject.transform);
            if (combinedMesh.vertexCount == 0)
            {
                Debug.LogError("Combined mesh has 0 vertices");
                return;
            }
            
            var meshFilter = combinedMeshObject.AddComponent<MeshFilter>();
            meshFilter.mesh = combinedMesh;
            var meshRenderer = combinedMeshObject.AddComponent<MeshRenderer>();
            var subMeshCount = combinedMesh.subMeshCount;
            
            var material = GetDefaultMaterial();

            var materials = new Material[subMeshCount];
            for (var i = 0; i < subMeshCount; i++)
            {
                materials[i] = material;
            }
            
            meshRenderer.materials = materials;
            Selection.activeObject = combinedMeshObject;
            var meshPath = EditorUtility.SaveFilePanelInProject(
                    "Save Mesh", 
                    "New Mesh", 
                    "asset", 
                    "Save combined mesh");

            if (!string.IsNullOrEmpty(meshPath))
            {
                var meshName = Path.GetFileNameWithoutExtension(meshPath);
                combinedMesh.name = meshName;
                AssetDatabase.CreateAsset(combinedMesh, meshPath);
                EditorGUIUtility.PingObject(combinedMesh);
            }
            else
            {
                combinedMesh.name = "Combined Mesh";
                Debug.Log("The combined mesh is saved within the object!", combinedMeshObject);
            }
        }

        public static Material GetDefaultMaterial()
        {
            var renderPipeline = GraphicsSettings.currentRenderPipeline;
            var isSrp = renderPipeline != null;
            string renderPipelineName = null;
            if (isSrp)
            {
                renderPipelineName = renderPipeline.GetType().Name.Contains("Universal") ? 
                        "Universal Render Pipeline" : 
                        "HDRP";
            }
            var defaultMaterial = isSrp
                    ? new Material(Shader.Find($"{renderPipelineName}/Lit"))
                    : AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
            defaultMaterial.color = new(.7f, .7f, .7f);
            return defaultMaterial;
        }

        public static Mesh BakeStatic(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            var tempMesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(tempMesh);
            return tempMesh;
        }

        public static void SaveMesh(Mesh mesh, string meshName)
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Mesh", meshName, "asset", "save Mesh");
            if (!string.IsNullOrEmpty(path))
                AssetDatabase.CreateAsset(mesh, path);
        }
        
        public static Mesh CombineMeshes(IEnumerable<GameObject> targetObjects, Transform transform)
        {
            var result = new Mesh
            {
                indexFormat = IndexFormat.UInt32
            };
            var combineInstances = ListPool<CombineInstance>.Get();
            foreach (var targetObject in targetObjects)
            {
                var meshFilters = targetObject.GetComponentsInChildren<MeshFilter>();
                foreach (var meshFilter in meshFilters)
                {
                    var mesh = meshFilter.sharedMesh;
                    if (!mesh)
                        continue;
                    var meshTransform = transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;

                    for (var i = 0; i < mesh.subMeshCount; i++)
                    {
                        combineInstances.Add(new CombineInstance()
                        {
                                mesh = mesh,
                                subMeshIndex = i,
                                transform = meshTransform,
                        });
                    }
                    
                }
            }

            if (combineInstances.Count == 0)
            {
                Debug.Log("No meshes found to combine");
                return result;
            }
            Debug.Log($"Preparing to combine {combineInstances.Count} meshes");
            result.CombineMeshes(combineInstances.ToArray());
            ListPool<CombineInstance>.Release(combineInstances);
            
            return result;
        }
    }
}