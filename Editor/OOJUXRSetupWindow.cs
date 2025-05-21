using UnityEditor;
using UnityEngine;
using System.IO;
using OojuXRPlugin;
using System.Linq;
using System;

namespace OojuXRPlugin
{
    public class OOJUXRSetupWindow : EditorWindow
    {
        private UIStyles styles;
        private int selectedSkybox = 0;
        private string[] skyboxOptions = { "Indoor", "Outdoor", "Sky", "City", "Custom" };

        [MenuItem("OOJU/XR Setup")]
        public static void ShowWindow()
        {
            GetWindow<OOJUXRSetupWindow>("OOJU XR Setup");
        }

        private void OnEnable()
        {
            styles = new UIStyles();
            selectedSkybox = EditorPrefs.GetInt("OOJU_SelectedSkybox", 0);
        }

        private void OnGUI()
        {
            if (styles != null && !styles.IsInitialized)
            {
                styles.Initialize();
            }
            DrawXRSetupTab();
        }

        private void DrawXRSetupTab()
        {
            float contentWidth = position.width - 40f; // 20px margin on each side
            float buttonWidth = Mathf.Min(300f, contentWidth * 0.8f); // Maximum 300px, or 80% of content width
            
            EditorGUILayout.BeginVertical();
            GUILayout.Space(20);

            // Add project setup section at the top
            DrawXRProjectSetupSection(buttonWidth);

            // Scene Setup Section
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("Scene Setup", EditorStyles.boldLabel);
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Quickly set up your scene for VR or Mixed Reality (MR) with camera and hand tracking.", EditorStyles.miniLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Set VR Scene", "Set up a VR scene with camera and hand tracking prefabs"), 
                GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                XRSceneSetup.SetupXRScene();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Set MR Scene", "Set up a Mixed Reality scene with passthrough and real hands prefabs"),
                GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                XRSceneSetup.SetupPassthroughAndRealHands();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
            
            // XR Features Section
            /*
            // [To be developed soon]
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("XR Features", EditorStyles.boldLabel);
            GUILayout.Space(5);
            EditorGUILayout.LabelField("This feature will be developed soon.", EditorStyles.miniLabel);
            GUILayout.Space(10);

            // Add Teleport button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Add Teleport", "Add teleportation feature to the scene"), GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                // To be developed soon
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            // Add Grab Interaction button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Add Grab Interaction", "Add grab interaction feature to the scene"), GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                // To be developed soon
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            // Add Ray Interaction button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Add Ray Interaction", "Add ray interaction feature to the scene"), GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                // To be developed soon
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
            */

            // Skybox Setup Section
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("Skybox Setup", EditorStyles.boldLabel);
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Change the scene skybox to a preset HDRi", EditorStyles.miniLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int newSelectedSkybox = EditorGUILayout.Popup(selectedSkybox, skyboxOptions, GUILayout.Width(buttonWidth - 110));
            if (newSelectedSkybox != selectedSkybox)
            {
                EditorPrefs.SetInt("OOJU_SelectedSkybox", newSelectedSkybox);
                selectedSkybox = newSelectedSkybox;
            }

            if (GUILayout.Button("Apply", GUILayout.Width(100), GUILayout.Height(24)))
            {
                if (selectedSkybox == 4) // Custom option (last index)
                {
                    string path = EditorUtility.OpenFilePanelWithFilters(
                        "Select HDRi Texture",
                        "",
                        new string[] { "HDR/EXR files", "hdr,exr", "All files", "*" }
                    );
                    if (!string.IsNullOrEmpty(path))
                    {
                        // Check if it's a project internal path
                        string projectPath = path;
                        if (path.StartsWith(Application.dataPath))
                        {
                            // Convert to Assets path if it's a project internal path
                            projectPath = "Assets" + path.Substring(Application.dataPath.Length);
                        }
                        else
                        {
                            // Copy if it's an external file
                            string fileName = Path.GetFileName(path);
                            string destPath = "Assets/Skybox/" + fileName;
                            
                            try
                            {
                                // Create target directory if it doesn't exist
                                string directory = Path.GetDirectoryName(destPath);
                                if (!Directory.Exists(directory))
                                {
                                    Directory.CreateDirectory(directory);
                                }
                                
                                // Copy file
                                File.Copy(path, Application.dataPath + "/../" + destPath, true);
                                AssetDatabase.Refresh();
                                projectPath = destPath;
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError($"Error copying file: {e.Message}");
                                EditorUtility.DisplayDialog("Error", "Failed to copy HDRi file to project.", "OK");
                                return;
                            }
                        }
                        SetCustomSkybox(projectPath);
                    }
                }
                else
                {
                    SetSkyboxByType(skyboxOptions[selectedSkybox]);
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(20);
            
            // Ground Setup Section
            /*
            // [To be developed soon]
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Ground Setup", EditorStyles.boldLabel);
            GUILayout.Space(5);
            EditorGUILayout.LabelField("This feature will be developed soon.", EditorStyles.miniLabel);
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Create Ground Plane", "Create a new plane as ground"), 
                GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                // To be developed soon
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Set Selected as Ground", "Set selected object as ground"), 
                GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                // To be developed soon
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
            */
            
            EditorGUILayout.EndVertical();
        }

        private void SetSkyboxByType(string type)
        {
            // Always use Assets/OOJU/XR/Skybox/ for skybox assets
            string assetsSkyboxDir = "Assets/OOJU/XR/Skybox/";
            string matName = $"Skybox_{type}.mat";
            string textureName = $"{type}.exr";
            string packagesTexturePath = $"Packages/com.ooju.xrsetup/Skybox/{textureName}";
            string assetsTexturePath = assetsSkyboxDir + textureName;
            string assetsMatPath = assetsSkyboxDir + matName;

            // Ensure the directory exists
            if (!System.IO.Directory.Exists(assetsSkyboxDir))
                System.IO.Directory.CreateDirectory(assetsSkyboxDir);

            // Always copy the texture from the package to Assets (overwrite every time)
            if (System.IO.File.Exists(packagesTexturePath))
            {
                System.IO.File.Copy(packagesTexturePath, assetsTexturePath, true);
                AssetDatabase.ImportAsset(assetsTexturePath);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", $"HDRi texture not found in package: {packagesTexturePath}", "OK");
                return;
            }

            AssetDatabase.Refresh();

            // Load the HDRI texture
            Texture2D hdriTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetsTexturePath);
            if (hdriTexture == null)
            {
                EditorUtility.DisplayDialog("Error", $"HDRi texture not found: {assetsTexturePath}", "OK");
                return;
            }

            // Create or update the skybox material
            Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(assetsMatPath);
            if (skyboxMat == null)
            {
                // Create a new material with Skybox/Panoramic shader
                skyboxMat = new Material(Shader.Find("Skybox/Panoramic"));
                skyboxMat.SetTexture("_MainTex", hdriTexture);
                skyboxMat.SetFloat("_Exposure", 1.0f);
                skyboxMat.SetFloat("_Rotation", 0f);
                skyboxMat.SetInt("_Mapping", 1); // LatLong mapping
                AssetDatabase.CreateAsset(skyboxMat, assetsMatPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                // Update the texture if the material already exists
                skyboxMat.SetTexture("_MainTex", hdriTexture);
                EditorUtility.SetDirty(skyboxMat);
                AssetDatabase.SaveAssets();
            }

            // Apply the skybox material to the scene
            RenderSettings.skybox = skyboxMat;
            EditorUtility.DisplayDialog("Skybox Changed", $"{type} skybox has been applied.", "OK");
        }

        private Material CreateSkyboxMaterial(Texture2D hdriTexture, string savePath)
        {
            // Create material
            Material skyboxMat = new Material(Shader.Find("Skybox/Panoramic"));
            
            // Set HDRi texture
            skyboxMat.SetTexture("_MainTex", hdriTexture);
            skyboxMat.SetFloat("_Exposure", 1.0f);
            skyboxMat.SetFloat("_Rotation", 0f);
            skyboxMat.SetInt("_Mapping", 1); // LatLong mapping
            
            // Save material
            string directory = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            AssetDatabase.CreateAsset(skyboxMat, savePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            return skyboxMat;
        }

        private void SetCustomSkybox(string texturePath)
        {
            // Refresh asset database
            AssetDatabase.Refresh();
            
            Texture2D hdriTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (hdriTexture == null)
            {
                EditorUtility.DisplayDialog("Error", $"Could not load HDRi texture at path: {texturePath}", "OK");
                return;
            }

            // Create material name from file name
            string fileName = Path.GetFileNameWithoutExtension(texturePath);
            string matPath = "Assets/OOJUXR/ooju-unity-xr/Skybox/Skybox_" + fileName + ".mat";

            // Check and create material save directory
            string directory = Path.GetDirectoryName(matPath);
            if (!Directory.Exists(Application.dataPath + "/../" + directory))
            {
                Directory.CreateDirectory(Application.dataPath + "/../" + directory);
            }

            // Check if material already exists
            Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            
            if (skyboxMat == null)
            {
                skyboxMat = CreateSkyboxMaterial(hdriTexture, matPath);
            }
            else
            {
                // Update texture if material exists
                skyboxMat.SetTexture("_MainTex", hdriTexture);
                EditorUtility.SetDirty(skyboxMat);
                AssetDatabase.SaveAssets();
            }

            if (skyboxMat != null)
            {
                RenderSettings.skybox = skyboxMat;
                EditorUtility.DisplayDialog("Skybox Changed", "Custom skybox has been applied.", "OK");
            }
        }

        // Project setup and validation section
        private void DrawXRProjectSetupSection(float buttonWidth)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Project Setup", EditorStyles.boldLabel);
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Configure your project for XR development", EditorStyles.miniLabel);
            GUILayout.Space(10);

            // Check and install XR Plugin Management
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Check XR Dependencies", "Check and install XR Plugin Management package"), GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                CheckAndInstallXRPluginManagement();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            // Switch build target to Android & Set Android API Level (merged)
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Set Build Settings for XR", "Switch build target to Android and set API Level 32 (12L)"), GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                SwitchToAndroidBuildTargetAndSetApiLevel();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            // Guide for enabling OpenXR and Project Validation (merged)
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Guide for XR Setup", "Show instructions for OpenXR and Project Validation"), GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                ShowXRSetupGuide();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
        }

        // Check and install XR Plugin Management package
        private void CheckAndInstallXRPluginManagement()
        {
            var listRequest = UnityEditor.PackageManager.Client.List(true, false);
            while (!listRequest.IsCompleted) { }
            bool installed = listRequest.Result.Any(p => p.name == "com.unity.xr.management");
            if (installed)
            {
                EditorUtility.DisplayDialog("XR Plugin Management", "XR Plugin Management package is already installed.", "OK");
            }
            else
            {
                if (EditorUtility.DisplayDialog("XR Plugin Management Required", "XR Plugin Management package is not installed. Would you like to install it now?", "Install", "Cancel"))
                {
                    UnityEditor.PackageManager.Client.Add("com.unity.xr.management");
                    EditorUtility.DisplayDialog("XR Plugin Management", "Installation started. Please check the Package Manager window.", "OK");
                }
            }
        }

        // Switch build target to Android and set API Level to 32 (Android 12L)
        private void SwitchToAndroidBuildTargetAndSetApiLevel()
        {
            // Switch build target to Android
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                EditorUtility.DisplayDialog("Build Target Switched", "Build target switched to Android.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Build Target", "Build target is already Android.", "OK");
            }

            // Set Android API Level to 32 (Android 12L)
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel32;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel32;
            EditorUtility.DisplayDialog("API Level Set", "Android min/target API Level set to 32 (Android 12L).", "OK");
        }

        // Show guide for enabling OpenXR and Project Validation (merged)
        private void ShowXRSetupGuide()
        {
            EditorUtility.DisplayDialog(
                "XR Setup Guide",
                "1. Go to Project Settings > XR Plug-in Management.\n" +
                "2. In both Windows and Android tabs, check 'OpenXR'.\n" +
                "3. In OpenXR settings, under 'Enabled Interaction Profiles', add the profiles that match your target device (e.g., Oculus Touch Controller Profile, HP Reverb G2 Controller Profile, etc.).\n" +
                "4. Go to the 'Project Validation' tab and click 'Fix All' to resolve all issues.",
                "OK");
        }
    }
} 