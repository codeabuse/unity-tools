using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Codeabuse.EditorTools
{
    public abstract class CustomEditorToolBase : EditorTool
    {
        [SerializeField]
        private Texture2D _toolIconLightTheme;
        [SerializeField]
        private Texture2D _toolIconDarkTheme;

        protected abstract string iconTooltip { get; }
        
        private static GUIContent _toolbarIcon;
        
        public override GUIContent toolbarIcon =>
            _toolbarIcon ??=
                new GUIContent(EditorGUIUtility.isProSkin?
                        _toolIconDarkTheme :
                        _toolIconLightTheme, 
                    iconTooltip);
    }
}