using UnityEngine;
#if INPUT_SYSTEM_ENABLED
using UnityEngine.InputSystem;
#endif
using UnityEngine.Serialization;

namespace Codeabuse.SceneManagement
{
    public class SceneCompositionLoader : MonoBehaviour
    {
#if INPUT_SYSTEM_ENABLED
        [SerializeField]
        private InputActionProperty _resetAction;
#endif

        [FormerlySerializedAs("_currentComposition")]
        [SerializeField]
        private SceneComposition _firstComposition;
        
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            _firstComposition.Load();
        }

        private void OnEnable()
        {
#if INPUT_SYSTEM_ENABLED
            if (_resetSceneSetup.action is null)
                return;
            _resetAction.action.Enable();
            
            _resetAction.action.performed += OnReset;
#endif
        }

        private void OnDisable()
        {
#if INPUT_SYSTEM_ENABLED
            _resetAction.action.Disable();
#endif
        }

#if INPUT_SYSTEM_ENABLED
        private void OnReset(InputAction.CallbackContext obj)
        {
            _firstComposition.Load();
        }
#endif
    }
}