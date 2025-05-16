# OOJU Unity XR Setup

A Unity Editor extension for quick XR scene setup, skybox management, and ground configuration.

## Features
- One-click XR-ready scene setup
- Easy skybox switching with included HDRi resources
- Ground creation and assignment for XR interactions
- Modular package structure for easy integration

## Installation
1. Clone or add this package as a submodule:
   ```
   git submodule add <repo-url> Assets/OOJUXR/ooju-unity-xr
   ```
2. Add the following to your `manifest.json` dependencies:
   ```json
   "com.ooju.xrsetup": "file:Assets/OOJUXR/ooju-unity-xr"
   ```
3. Open Unity. The OOJU XR Setup window is available under `OOJU > XR Setup` in the menu.

## Dependencies
- [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.4/manual/index.html) (2.4.3 or later, for Unity 2021.3)

## Skybox Resources
- HDRi images and skybox materials are included in `Skybox/`.
- You can add your own HDRi files and use the "Custom" option in the window.

## Usage
- Use the editor window to set up XR scenes, change skyboxes, and configure ground objects easily.
- For hand tracking, future updates will provide additional features.

