using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Codeabuse.EditorTools
{
    /// <summary>
    /// Allows to add custom UI elements to the Editor's top toolbar (near the Play/Pause buttons).
    /// Use <see cref="IToolbarAddonBehaviour"/> interface to write your own custom logic, then create
    /// <see cref="ToolbarAddon"/> scriptable object instance and choose your class.
    /// You can use UXML or create GUI from code. 
    /// </summary>
    public static class UnityToolbarExtension
    {
        public enum ToolbarArea
        {
            Left,
            Right,
            Central
        }
        
        private const string left_toolbar_area_name = "ToolbarZoneLeftAlign";
        private const string right_toolbar_area_name = "ToolbarZoneRightAlign";
        private const string central_toolbar_area_name = "ToolbarZonePlayMode";

        private const int get_root_safe_attempts = 10;
        
        private static readonly Type toolbar_type = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static readonly FieldInfo root_visual_element_field =
            toolbar_type.GetField("m_Root", BindingFlags.Instance | BindingFlags.NonPublic);

        private static ScriptableObject _toolbar;
        private static VisualElement _toolbarRoot;
        private static VisualElement _leftToolbarArea;
        private static VisualElement _rightToolbarArea;
        private static VisualElement _centralToolbarArea;
        private static int _getRootAttempts;
        private static List<ToolbarAddon> _toolbarAddons;

        private static readonly Dictionary<IToolbarAddonBehaviour, VisualElement> addon_root_elements = new();

        public static bool TryGetAddonRoot(IToolbarAddonBehaviour toolbarAddonBehaviour, out VisualElement root) =>
            addon_root_elements.TryGetValue(toolbarAddonBehaviour, out root);

        [MenuItem("Tools/Reload Toolbar Addons", false, 0)]
        public static void ReloadAddons()
        {
            if (!_toolbar)
            {
                Debug.LogError($"{nameof(UnityToolbarExtension)}: No Toolbar object found");
                return;
            }
            foreach (var (addonBehaviour, addonRoot) in addon_root_elements)
            {
                addonBehaviour.OnDestroy();
                var addonParent = addonRoot.parent;
                addonParent.Remove(addonRoot);
            }
            
            InitializeToolbarAddons();
        }

        [InitializeOnLoadMethod]
        private static void GetToolbar()
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        private static VisualElement GetToolbarRoot()
        {
            return root_visual_element_field.GetValue(_toolbar) as VisualElement;
        }

        static void OnEditorUpdate()
        {
            if (EditorApplication.isCompiling)
                return;
            
            if (_getRootAttempts == get_root_safe_attempts)
            {
                Debug.LogError(
                        $"{nameof(UnityToolbarExtension)}: Can't find Toolbar's root Visual Element after {_getRootAttempts} attempts.");
                EditorApplication.update -= OnEditorUpdate;
                return;
            }
            
            if (!_toolbar)
            {
                _toolbar = Resources.FindObjectsOfTypeAll(toolbar_type).FirstOrDefault() as ScriptableObject;
                if (!_toolbar)
                {
                    EditorApplication.update -= OnEditorUpdate;
                    Debug.LogError($"{nameof(UnityToolbarExtension)}: unable to find UnityEditor.Toolbar object");
                    return;
                }
            }
            
            _toolbarRoot ??= GetToolbarRoot();
            
            if (_toolbarRoot is null)
            {
                _getRootAttempts++;
                return;
            }

            _getRootAttempts = 0;
            FindToolbarComponents();
            InitializeToolbarAddons();
            EditorApplication.update -= OnEditorUpdate;
        }

        private static void InitializeToolbarAddons()
        {
            _toolbarAddons = new (Resources.LoadAll<ToolbarAddon>(""));
            _toolbarAddons.Sort((x, y)=> y.Priority - x.Priority);
            foreach (var toolbarAddon in _toolbarAddons)
            {
                var toolbarAddonBehaviour = toolbarAddon.ToolbarAddonBehaviour;
                if (toolbarAddonBehaviour is null)
                {
                    Debug.LogError($"No addon behavior selected for {toolbarAddon.name}", toolbarAddon);
                    continue;
                }
                toolbarAddonBehaviour.OnAwake();
                var areaRoot = GetToolbarArea(toolbarAddon.ToolbarArea);
                var addonRoot = toolbarAddon.VisualTree ? 
                    toolbarAddon.VisualTree.Instantiate() : 
                    new VisualElement();
                addonRoot.name = toolbarAddon.name;
                if (toolbarAddon.Styles)
                    addonRoot.styleSheets.Add(toolbarAddon.Styles);

                try
                {
                    toolbarAddonBehaviour.OnCreateGUI(addonRoot);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Can't create GUI for toolbar addon '{toolbarAddon.name}':");
                    Debug.LogException(e);
                    continue;
                }
                areaRoot.Add(addonRoot);
                addon_root_elements[toolbarAddonBehaviour] = addonRoot;
            }
        }

        private static VisualElement GetToolbarArea(ToolbarArea area) => area switch
        {
            ToolbarArea.Left => _leftToolbarArea,
            ToolbarArea.Right => _rightToolbarArea,
            ToolbarArea.Central => _centralToolbarArea,
            _ => throw new ArgumentOutOfRangeException(nameof(area), area, null)
        };

        private static void FindToolbarComponents()
        {
            _leftToolbarArea = _toolbarRoot.Q(left_toolbar_area_name);
            _rightToolbarArea = _toolbarRoot.Q(right_toolbar_area_name);
            _centralToolbarArea = _toolbarRoot.Q(central_toolbar_area_name);
        }
    }
}