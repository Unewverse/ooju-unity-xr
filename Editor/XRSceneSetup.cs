using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace OojuXRPlugin
{
    public class XRSceneSetup
    {
        public static void SetupXRScene()
        {
            // Get current scene
            Scene currentScene = SceneManager.GetActiveScene();

            // Remove all existing cameras in the scene
            // Changed to FindObjectsByType for better performance and to avoid deprecation warning
            Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach (Camera cam in cameras)
            {
                Object.DestroyImmediate(cam.gameObject);
            }

            // Path to the prefab in Assets and Packages
            string prefabPathAssets = "Assets/OOJUXR/ooju-unity-xr/Prefabs/OOJU_XR_Setup_Hands.prefab";
            string prefabPathPackages = "Packages/com.ooju.xrsetup/Prefabs/OOJU_XR_Setup_Hands.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPathAssets);
            if (prefab == null)
            {
                // Try to load from Packages if not found in Assets
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPathPackages);
            }
            if (prefab != null)
            {
                // Instantiate the prefab in the scene
                PrefabUtility.InstantiatePrefab(prefab);
            }
            else
            {
                EditorUtility.DisplayDialog("Prefab Missing", "OOJU_XR_Setup_Hands prefab not found in either Assets or Packages folder.", "OK");
                return;
            }

            // Mark scene as dirty and save
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);

            EditorUtility.DisplayDialog("Success", "OOJU XR Setup Hands prefab added to the scene!", "OK");
        }

        public static void SetupPassthroughAndRealHands()
        {
            // Get current scene
            Scene currentScene = SceneManager.GetActiveScene();

            // Remove all existing cameras in the scene
            // Changed to FindObjectsByType for better performance and to avoid deprecation warning
            Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach (Camera cam in cameras)
            {
                Object.DestroyImmediate(cam.gameObject);
            }

            // List of prefab names to add
            string[] prefabNames = {
                "OOJU_XR_Camera Rig.prefab",
                "OOJU_XR_Passthrough.prefab",
                "OOJU_XR_Real_Hands.prefab"
            };

            // Try to add each prefab from Assets first, then Packages
            foreach (string prefabName in prefabNames)
            {
                string prefabPathAssets = $"Assets/OOJUXR/ooju-unity-xr/Prefabs/{prefabName}";
                string prefabPathPackages = $"Packages/com.ooju.xrsetup/Prefabs/{prefabName}";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPathAssets);
                if (prefab == null)
                {
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPathPackages);
                }
                if (prefab != null)
                {
                    // Instantiate the prefab in the scene
                    PrefabUtility.InstantiatePrefab(prefab);
                }
                else
                {
                    EditorUtility.DisplayDialog("Prefab Missing", $"{prefabName} not found in either Assets or Packages folder.", "OK");
                    return;
                }
            }

            // Mark scene as dirty and save
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);

            EditorUtility.DisplayDialog("Success", "Passthrough & Real Hands prefabs added to the scene!", "OK");
        }
    }
} 