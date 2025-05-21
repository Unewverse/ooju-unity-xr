# OOJU Unity XR Setup

A Unity Editor extension for rapid XR project configuration, scene setup, and skybox management.

## Features
- **One-click XR Project Setup**
  - Installs XR Plugin Management if missing
  - Switches build target to Android and sets API Level 32 (12L)
  - Provides a step-by-step guide for enabling OpenXR and project validation
- **Quick XR Scene Setup**
  - Instantly configures your scene for VR or Mixed Reality (MR)
  - Adds ready-made prefabs for:
    - VR: Camera and hand tracking (`OOJU_XR_Setup_Hands`)
    - MR: Camera rig, passthrough, and real hands (`OOJU_XR_Camera Rig`, `OOJU_XR_Passthrough`, `OOJU_XR_Real_Hands`)
  - Removes existing cameras to avoid conflicts
- **Skybox Management**
  - Easily switch between included HDRi skyboxes: Indoor, Outdoor, Sky, City
  - Supports custom HDRi/EXR files via file picker
  - Automatically creates and assigns panoramic skybox materials
- **Ground Utility**
  - `Ground` component (runtime): Ensures objects do not fall through the ground by correcting their position on collision
- **Modular Package Structure**
  - Clean separation of Editor, Runtime, Prefabs, and Skybox resources for easy integration and maintenance

## Installation
1. Clone or add this package as a submodule:
   ```bash
   git submodule add <repo-url> Assets/OOJUXR/ooju-unity-xr
   ```
2. Add the following to your `manifest.json` dependencies:
   ```json
   "com.ooju.xrsetup": "file:Assets/OOJUXR/ooju-unity-xr"
   ```
3. Open Unity. The OOJU XR Setup window is available under `OOJU > XR Setup` in the menu.

## Dependencies
- [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.4/manual/index.html) (2.4.3 or later, for Unity 2021.3+)
- [Meta All-in-One SDK (Oculus Integration)](https://assetstore.unity.com/packages/tools/integration/meta-xr-all-in-one-sdk-269657?srsltid=AfmBOorCBiHmPpycq3_mzeal7KQk_UkFDJlyW6gh-qDmmacRkxE_YUG4) (required for Meta Quest/Quest Pro passthrough and hand tracking features)

## Skybox Resources
- HDRi images (`.exr`) and skybox materials are included in the `Skybox/` folder.
- You can add your own HDRi files and use the "Custom" option in the editor window.

## Usage
- Open the **OOJU XR Setup** window from the Unity menu: `OOJU > XR Setup`
- Use the **Project Setup** section to prepare your project for XR development
- Use the **Scene Setup** section to quickly add VR or MR camera/hand prefabs to your scene
- Use the **Skybox Setup** section to change the scene skybox or add your own
- For ground collision, add the `Ground` component to your ground object

## Notes
- For OpenXR, make sure to enable the correct Interaction Profiles for your target device in Project Settings.
- Meta All-in-One SDK (Oculus Integration) is required for passthrough and hand tracking on Meta Quest devices.
- Future updates will provide additional features for hand tracking and ground setup.

