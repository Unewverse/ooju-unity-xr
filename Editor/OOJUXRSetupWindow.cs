using UnityEditor;
using UnityEngine;
using System.IO;
using OojuXRPlugin;

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
            
            GUILayout.Space(20);

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
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("Ground Setup", EditorStyles.boldLabel);
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Set up ground for XR interactions and animations", EditorStyles.miniLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Create Ground Plane", "Create a new plane as ground"), 
                GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                CreateGroundPlane();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Set Selected as Ground", "Set selected object as ground"), 
                GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                SetSelectedAsGround();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndVertical();
        }

        private void SetSkyboxByType(string type)
        {
            string matName = "";
            string textureName = "";
            switch (type)
            {
                case "Indoor":
                    matName = "Skybox_Indoor.mat";
                    textureName = "Indoor.exr";
                    break;
                case "Outdoor":
                    matName = "Skybox_Outdoor.mat";
                    textureName = "Outdoor.exr";
                    break;
                case "Sky":
                    matName = "Skybox_Sky.mat";
                    textureName = "Sky.exr";
                    break;
                case "City":
                    matName = "Skybox_City.mat";
                    textureName = "City.exr";
                    break;
            }
            
            string assetsMatPath = "Assets/OOJUXR/ooju-unity-xr/Skybox/" + matName;
            string packagesMatPath = "Packages/com.ooju.xrsetup/Skybox/" + matName;
            string assetsTexturePath = "Assets/OOJUXR/ooju-unity-xr/Skybox/" + textureName;
            string packagesTexturePath = "Packages/com.ooju.xrsetup/Skybox/" + textureName;

            Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(assetsMatPath);
            if (skyboxMat == null)
                skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(packagesMatPath);

            if (skyboxMat == null)
            {
                // Create material if it doesn't exist
                Texture2D hdriTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetsTexturePath);
                if (hdriTexture == null)
                    hdriTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(packagesTexturePath);
                if (hdriTexture != null)
                {
                    skyboxMat = CreateSkyboxMaterial(hdriTexture, assetsMatPath);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", $"HDRi texture not found: {assetsTexturePath} or {packagesTexturePath}", "OK");
                    return;
                }
            }

            if (skyboxMat != null)
            {
                RenderSettings.skybox = skyboxMat;
                EditorUtility.DisplayDialog("Skybox Changed", $"{type} skybox has been applied.", "OK");
            }
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

        private void CreateGroundPlane()
        {
            // Create a new plane
            GameObject groundPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            groundPlane.name = "Ground";
            
            // Add necessary components
            var collider = groundPlane.GetComponent<Collider>();
            if (collider == null)
            {
                groundPlane.AddComponent<BoxCollider>();
            }
            
            // Add Ground component
            var ground = groundPlane.AddComponent<Ground>();
            
            // Position the plane
            groundPlane.transform.position = Vector3.zero;
            groundPlane.transform.rotation = Quaternion.identity;
            
            // Select the created plane
            Selection.activeGameObject = groundPlane;
            
            EditorUtility.DisplayDialog("Success", "Ground plane has been created successfully!", "OK");
        }

        private void SetSelectedAsGround()
        {
            var selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select an object first.", "OK");
                return;
            }

            // Add Ground component if not exists
            var ground = selectedObject.GetComponent<Ground>();
            if (ground == null)
            {
                ground = selectedObject.AddComponent<Ground>();
            }

            // Collider handling
            var meshFilter = selectedObject.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                // Remove existing BoxCollider if present
                var boxCollider = selectedObject.GetComponent<BoxCollider>();
                if (boxCollider != null)
                {
                    DestroyImmediate(boxCollider);
                }
                // Add MeshCollider if not present
                var meshCollider = selectedObject.GetComponent<MeshCollider>();
                if (meshCollider == null)
                {
                    meshCollider = selectedObject.AddComponent<MeshCollider>();
                }
                meshCollider.sharedMesh = meshFilter.sharedMesh;
                meshCollider.convex = false; // Ground should not be convex
            }
            else
            {
                // Keep BoxCollider if there is no mesh
                if (selectedObject.GetComponent<Collider>() == null)
                {
                    selectedObject.AddComponent<BoxCollider>();
                }
            }

            EditorUtility.DisplayDialog("Success", "Selected object has been set as ground!", "OK");
        }
    }
} 