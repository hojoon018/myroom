using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OatAotOat.ProceduralDigitalAsset
{
    public static class ProceduralBakingUtility
    {
        public static Shader GetSafeShader()
        {
            Shader shader = Shader.Find("Standard");
            if (shader == null) { shader = Shader.Find("Universal Render Pipeline/Lit"); }
            if (shader == null) { shader = Shader.Find("HDRP/Lit"); }
            return shader;
        }

        public static void BakeAsset(string folderName, GameObject generatorRoot, string prefabName,
                                     Color primaryColor, Color secondaryColor,
                                     System.Action<GameObject, Material, Material> applyMaterialsAction,
                                     System.Action<GameObject> organizeHierarchyAction)
        {
            string baseFolder = "Assets/Prefabs";
            string finalFolder = $"Assets/Prefabs/{folderName}";
            string materialFolder = finalFolder + "/Materials";
            string dataFolder = finalFolder + "/BakedData";

            if (!AssetDatabase.IsValidFolder(baseFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            if (!AssetDatabase.IsValidFolder(finalFolder))
            {
                AssetDatabase.CreateFolder(baseFolder, folderName);
            }

            if (!AssetDatabase.IsValidFolder(materialFolder))
            {
                AssetDatabase.CreateFolder(finalFolder, "Materials");
            }

            if (!AssetDatabase.IsValidFolder(dataFolder))
            {
                AssetDatabase.CreateFolder(finalFolder, "BakedData");
            }

            string finalName = string.IsNullOrEmpty(prefabName) ? "GeneratedAsset" : prefabName;
            string fullPath = $"{finalFolder}/{finalName}.prefab";
            int counter = 1;

            while (AssetDatabase.LoadAssetAtPath<GameObject>(fullPath) != null)
            {
                finalName = $"{prefabName}.{counter:D3}";
                fullPath = $"{finalFolder}/{finalName}.prefab";
                counter++;
            }

            GameObject clone = Object.Instantiate(generatorRoot);
            clone.name = finalName;

            Dictionary<Mesh, Mesh> savedMeshMap = new();
            MeshFilter[] allFilters = clone.GetComponentsInChildren<MeshFilter>(true);

            foreach (MeshFilter filter in allFilters)
            {
                if (filter.sharedMesh != null && (filter.sharedMesh.name.Contains("Sheared") || filter.sharedMesh.name.Contains("Procedural_Polygon")))
                {
                    Mesh originalMesh = filter.sharedMesh;
                    if (!savedMeshMap.ContainsKey(originalMesh))
                    {
                        Mesh newSavedMesh = Object.Instantiate(originalMesh);
                        newSavedMesh.name = originalMesh.name + "_Baked";
                        AssetDatabase.CreateAsset(newSavedMesh, $"{dataFolder}/{finalName}_{originalMesh.name}_Baked_{savedMeshMap.Count}.asset");
                        savedMeshMap[originalMesh] = newSavedMesh;
                    }
                    filter.sharedMesh = savedMeshMap[originalMesh];

                    if (filter.TryGetComponent(out MeshCollider mc))
                    {
                        mc.sharedMesh = savedMeshMap[originalMesh];
                    }
                }
            }

            Shader safeShader = GetSafeShader();

            Material primaryMat = new(safeShader);
            primaryMat.SetColor("_BaseColor", primaryColor);
            primaryMat.SetColor("_Color", primaryColor);

            Material secondaryMat = new(safeShader);
            secondaryMat.SetColor("_BaseColor", secondaryColor);
            secondaryMat.SetColor("_Color", secondaryColor);

            AssetDatabase.CreateAsset(primaryMat, $"{materialFolder}/{finalName}_Primary.mat");
            AssetDatabase.CreateAsset(secondaryMat, $"{materialFolder}/{finalName}_Secondary.mat");

            applyMaterialsAction?.Invoke(clone, primaryMat, secondaryMat);

            organizeHierarchyAction?.Invoke(clone);

            foreach (var comp in clone.GetComponents<MonoBehaviour>())
            {
                Object.DestroyImmediate(comp);
            }

            GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(clone, fullPath);
            Object.DestroyImmediate(clone);

            Debug.Log("Successfully baked procedural asset to: " + fullPath);
            EditorGUIUtility.PingObject(newPrefab);
        }
    }
}
