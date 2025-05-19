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
            Camera[] cameras = Object.FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
            {
                Object.DestroyImmediate(cam.gameObject);
            }

            // Path to the prefab
            string prefabPath = "Assets/OOJUXR/ooju-unity-xr/Prefabs/OOJU_XR_Setup_Hands.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
            {
                // Instantiate the prefab in the scene
                PrefabUtility.InstantiatePrefab(prefab);
            }
            else
            {
                EditorUtility.DisplayDialog("Prefab Missing", "OOJU_XR_Setup_Hands prefab not found at: " + prefabPath, "OK");
                return;
            }

            // Mark scene as dirty and save
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);

            EditorUtility.DisplayDialog("Success", "OOJU XR Setup Hands prefab added to the scene!", "OK");
        }
    }
} 