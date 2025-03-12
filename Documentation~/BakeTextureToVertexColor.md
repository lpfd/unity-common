# Using Prefab Context Menu to Bake Texture to Vertex Color in Unity

## Overview
This guide explains how to use the Unity Editor's prefab context menu to perform a "Bake Texture To Vertex Color" operation. This operation creates a baked copy of the original prefab, where the mesh materials are replaced with a new `VertexColor` material that has the original texture colors baked into vertex colors.

## How It Works
1. The operation generates a new prefab with the `.baked.prefab` extension.
2. The original mesh textures are analyzed, and color data is transferred into the mesh's vertex colors.
3. The original materials are replaced with a new `VertexColor` material that uses only vertex colors.

## Steps to Use

1. **Select a Prefab**
   - In the Unity Editor, locate the prefab you want to bake.
   - Right-click on the prefab in the **Project** window.

2. **Run the Bake Operation**
   - In the context menu, select: `Leap Forward` and then `Bake Texture to Vertex Color`
   - The operation will process the selected prefab and generate a new baked version.

![BakeTextureToVertexColor](Images/BakeTextureToVertexColor.png)

3. **Locate the Baked Prefab**
   - The new prefab will be created in the same folder as the original, with the name format:
     ```
     OriginalName.baked.prefab
     ```
   - This new prefab contains meshes that have baked vertex colors instead of texture-based materials.

![BakeTextureToVertexColorResult](Images/BakeTextureToVertexColorResult.png)

4. **Using the Baked Prefab**
   - Drag the baked prefab into a scene to preview the results.
   - The mesh renderer now uses a `VertexColor` material, eliminating the need for the original texture.
   - This optimization can reduce draw calls and improve performance in certain rendering scenarios.

## Notes
- Ensure that the original prefab has sufficient vertex density to capture texture details accurately.
- The baking process is irreversible on the baked prefab; keep a backup of the original.
- If the baked result lacks sufficient detail, consider increasing mesh tessellation before baking.

## Conclusion
Using this method, you can optimize your prefabs by converting texture-based materials to vertex color materials, improving performance and reducing texture memory usage. This is especially useful for stylized games, mobile applications, and VR experiences.

