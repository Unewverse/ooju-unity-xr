using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;

namespace OojuXRPlugin
{
    public class XRSceneSetup
    {
        public static void SetupXRScene()
        {
            // Check if XR Interaction Toolkit is installed
            if (!IsPackageInstalled("com.unity.xr.interaction.toolkit"))
            {
                EditorUtility.DisplayDialog("Package Missing", 
                    "XR Interaction Toolkit package is not installed. Please install it from Package Manager.", 
                    "OK");
                return;
            }

            // Get current scene
            Scene currentScene = SceneManager.GetActiveScene();
            
            // Create XR Origin
            GameObject xrOrigin = new GameObject("XR Origin");
            var origin = xrOrigin.AddComponent<XROrigin>();
            
            // Create Camera Offset
            GameObject cameraOffset = new GameObject("Camera Offset");
            cameraOffset.transform.parent = xrOrigin.transform;
            cameraOffset.transform.localPosition = Vector3.zero;
            
            // Create Main Camera
            GameObject mainCamera = new GameObject("Main Camera");
            mainCamera.transform.parent = cameraOffset.transform;
            mainCamera.transform.localPosition = Vector3.zero;
            Camera camera = mainCamera.AddComponent<Camera>();
            mainCamera.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRController>();
            
            // Create Event System
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            // Create XR Interaction Manager
            GameObject interactionManager = new GameObject("XR Interaction Manager");
            interactionManager.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>();
            
            // Save scene
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);
            
            EditorUtility.DisplayDialog("Success", "XR Scene setup completed successfully!", "OK");
        }

        private static bool IsPackageInstalled(string packageName)
        {
            var packageList = UnityEditor.PackageManager.Client.List();
            while (!packageList.IsCompleted)
            {
                System.Threading.Thread.Sleep(100);
            }
            
            foreach (var package in packageList.Result)
            {
                if (package.name == packageName)
                {
                    return true;
                }
            }
            return false;
        }
    }
} 