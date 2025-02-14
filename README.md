::WIP

A set of tools and helpers for generic project.

Tools:
* Combine selected meshes: creates a unified mesh out of any selected objects on the scene.
* Bake skinned mesh: creates a regular mesh out of any skinned mesh, preserving the pose and materials.
* Scene Composition and Editor Scene Composition assets: allows to handle multi-scene setups as single asset, both in Editor (for quick navigation) and in Runtime. Editor compositions are intended for level designers, while Runtime compositions allowing to handle multi-scene setup like a single entity.
  
Add-ons:
* Toolbar add-on system: allows to create custom UI elements on the Unity's top toolbar near the Play button.
* Scene Management add-on:
  - Adds GUI on the top toolbar for selecting and playing Scenes and Scene Compositions.
  - Allows to jump to the play mode without painfully navigating to particular scene or constructing multi-scene setup manually.

Coding features:
* Instantiate attribute and drawer: creates an instance of a class in a [SerializeReference] marked field through the dropdown in Inspector. Types derived from the field type are collected via reflection.
* WithInterface attribute: allows to restrict Object property with an interface (field type can be any derived from the UnityEngine.Object). This, the drag-and-drop operation will filter out unsupported ubjects. The filtered object selection window is yet to be implemented.
* DefaultUIElementsEditor: an inspector that replaces the default Unity's inspector to unitlize the power of UIToolkit. Classes with custom inspectors will use their own inspectors.
  If you don't want a particular class to be represented by this inspector, create an inspector class for it that is derived directly from Editor and leave no implementation.
* Inspector Extensions system: provides the ability to add anything to any Inspector without modifying or creating the Inspector class itself.
* ContextMenu extension for Inspector: any MonoBehaviour functions marked with [ContextMenu] attribute are drawn as buttons in the Inspector using DefaultUIElementsEditor
* ProjectPrefs: a system based on EditorPrefs, but with key prefix based on project name. Allows to handle Editor user settings per-project.
* StaticDictionary base class: a singleton-like point of access to various data, usually some sorts of cache for different tools. It is recommended to provide users with some way of cleanig that cache.
