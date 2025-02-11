using UnityEngine;
using UnityEngine.UIElements;

namespace Codeabuse.EditorTools
{
    
    public interface IToolbarAddonBehaviour
    {
        void OnAwake();
        /// <summary>
        /// Called after instantiating VisualTreeAsset if it is present, otherwise creates an empty VisualElement
        /// that can be populated in this method.
        /// </summary>
        /// <param name="rootElement"></param>
        void OnCreateGUI(VisualElement rootElement);
        void OnDestroy();
    }
    
    [CreateAssetMenu(fileName = "Toolbar Addon", menuName = "Editor Tools/Toolbar Addon")]
    public class ToolbarAddon : ScriptableObject
    {
        [SerializeField]
        private VisualTreeAsset _visualTree;

        [SerializeField]
        private StyleSheet _styles;

        [SerializeField]
        private UnityToolbarExtension.ToolbarArea _toolbarArea;

        [SerializeField]
        private int _priority;

        [SerializeReference, Instantiate(typeof(IToolbarAddonBehaviour))]
        private IToolbarAddonBehaviour _toolbarAddonBehaviour;

        public VisualTreeAsset VisualTree => _visualTree;

        public StyleSheet Styles => _styles;
        public UnityToolbarExtension.ToolbarArea ToolbarArea => _toolbarArea;
        public IToolbarAddonBehaviour ToolbarAddonBehaviour => _toolbarAddonBehaviour;
        public int Priority => _priority;
    }
}