using System.Collections.Generic;
using System.Linq;
using Codeabuse.SceneManagement;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Codeabuse.EditorTools
{
    [UsedImplicitly]
    public class PlayModeSceneSelectAddon : IToolbarAddonBehaviour
    {
        private const string last_played_setup_id_key = "PlayModeSceneIndex";
        private const string ask_to_save_changes_key = "PlayModeSceneSelect_AskToSaveChanges";
        private const string play_button_name = "play-button";
        private const string load_button_name = "load-button";
        private const string save_current_setup_button_name = "save-current-setup-button";
        
        
        private VisualElement _rootVisualElement;
        private Button _playButton;
        private Button _loadButton;
        private Button _saveSetupButton;
        private DropdownField _sceneSelectDropdown;

        private Scene[] _openedScenes;
        private SceneSetup[] _storedSceneSetup;
        private bool _hasJumpedToPlaymode;
        private bool _askToSaveChanges;

        private readonly List<ISceneSetup> _sceneSetups = new();
        private ISceneSetup _lastPlayedSetup;

        private int selectedSceneSetupIndex
        {
            get => _sceneSelectDropdown.index;
            set => _sceneSelectDropdown.index = value;
        }

        void IToolbarAddonBehaviour.OnAwake()
        {
            UpdateSceneSetups();
            EditorBuildSettings.sceneListChanged -= OnSceneListChanged;
            EditorBuildSettings.sceneListChanged += OnSceneListChanged;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }

        private void OnSceneListChanged()
        {
            UpdateSceneSetups();
            UpdateDropdownChoices();
        }

        private void UpdateSceneSetups()
        {
            _sceneSetups.Clear();
            _sceneSetups.AddRange(EditorBuildSettings.scenes
                   .Select(ebss => new BuildSceneSetup(ebss)));
            
            _sceneSetups.AddRange(EditorSceneCompositionManager.GetCompositions());

            SubscribeToSetupUpdates();
        }

        private void SubscribeToSetupUpdates()
        {
            foreach (var sceneSetup in _sceneSetups)
            {
                sceneSetup.OnEditorUpdated -= SceneSetupUpdated;
                sceneSetup.OnEditorUpdated += SceneSetupUpdated;
            }
        }

        private void UnsubscribeFromSetupUpdates()
        {
            foreach (var sceneSetup in _sceneSetups)
            {
                if (sceneSetup is null)
                    continue;
                sceneSetup.OnEditorUpdated -= SceneSetupUpdated;
            }
        }

        private void SceneSetupUpdated(ISceneSetup sceneSetup)
        {
            if (sceneSetup is Object unityObject && unityObject == false)
            {
                sceneSetup.OnEditorUpdated -= SceneSetupUpdated;
                _sceneSetups.Remove(sceneSetup);
                return;
            }
            
            UpdateDropdownChoices();
        }

        private void UpdateDropdownChoices()
        {
            CleanupSetups();
            _sceneSelectDropdown.choices = _sceneSetups.Select(s => s.name).ToList();
        }

        private void CleanupSetups()
        {
            for (var i = _sceneSetups.Count - 1; i >= 0; i--)
            {
                var setup = _sceneSetups[i];
                if (setup is BuildSceneSetup)
                    continue;
                
                if (!(setup as Object))
                    _sceneSetups.RemoveAt(i);
            }
        }

        private void OnPlayModeStateChange(PlayModeStateChange playModeState)
        {
            var enableControl = playModeState is PlayModeStateChange.EnteredEditMode;
            _rootVisualElement.SetEnabled(enableControl);
            
            if (enableControl && _hasJumpedToPlaymode)
            {
                RestorePreviousSetup();
            }

            if (playModeState is PlayModeStateChange.EnteredEditMode)
            {
                UpdateSceneSetups();
            }
        }

        void IToolbarAddonBehaviour.OnCreateGUI(VisualElement rootElement)
        {
            _rootVisualElement = rootElement;
            _playButton = rootElement.Q<Button>(play_button_name);
            _loadButton = rootElement.Q<Button>(load_button_name);
            _saveSetupButton = rootElement.Q<Button>(save_current_setup_button_name);
            _playButton.clicked += OnPlayButtonClicked;
            _loadButton.clicked += OnLoadButtonClicked;
            _saveSetupButton.clicked += SaveCurrentSetupClicked;
            _sceneSelectDropdown = rootElement.Q<DropdownField>();
            _sceneSelectDropdown.RegisterCallback<PointerDownEvent>(OnDropdownClicked);
            UpdateDropdownChoices();
            var askToSaveChangesToggle = rootElement.Q<Toggle>();
            askToSaveChangesToggle.RegisterValueChangedCallback(OnSaveChangesToggle);
            askToSaveChangesToggle.value = ProjectPrefs.GetBool(ask_to_save_changes_key, true);
            LoadLastPlayedSetup().Match(
                    sceneSetup =>
                    {
                        selectedSceneSetupIndex = _sceneSetups.IndexOf(sceneSetup);
                        _lastPlayedSetup = sceneSetup;
                    },
                    () =>
                    {
                        selectedSceneSetupIndex = 0;
                    });
            _rootVisualElement.SetEnabled(!EditorApplication.isPlaying && _sceneSetups.Count > 0);
        }

        private void OnDropdownClicked(PointerDownEvent evt)
        {
            if (evt.button is not 1)
                return;

            if (_sceneSetups.Count > selectedSceneSetupIndex)
            {
                EditorGUIUtility.PingObject(_sceneSetups[selectedSceneSetupIndex].GetUnderlyingObject());
            }
        }

        private void OnSaveChangesToggle(ChangeEvent<bool> evt)
        {
            ProjectPrefs.SetBool(ask_to_save_changes_key, _askToSaveChanges = evt.newValue);
        }

        private void OnPlayButtonClicked()
        {
            RememberCurrentSetup();
            _lastPlayedSetup = _sceneSetups[selectedSceneSetupIndex];
            SaveLastPlayedSetup();
            _lastPlayedSetup.Load();
            EditorApplication.EnterPlaymode();
            _hasJumpedToPlaymode = true;
        }

        private void OnLoadButtonClicked()
        {
            if (!CanLoadSceneSetup())
                return;
            var index = _sceneSetups.IndexOf(_lastPlayedSetup);
            if (index == -1)
                index = 0;
            _sceneSetups[selectedSceneSetupIndex].Load();
            selectedSceneSetupIndex = index;
        }

        private void SaveCurrentSetupClicked()
        {
            var saveWindow = EditorWindow.GetWindow<SaveSceneSetupModalWindow>(true);
            saveWindow.ShowLoadedScenes(EditorSceneManager.GetSceneManagerSetup().Select(ss => ss.path));
            saveWindow.OnSaveDialogClosed += HandleSaveSceneSetup;
        }

        private void HandleSaveSceneSetup(Option<string> option)
        {
            option.Match(
                    name =>
                    {
                        EditorSceneCompositionManager.GetOrCreate(name).Match(
                                setup =>
                                {
                                    setup.Save();
                                    UpdateSceneSetups();
                                    UpdateDropdownChoices();
                                    EditorGUIUtility.PingObject(setup as Object);
                                });
                    });
        }

        private bool CanLoadSceneSetup()
        {
            if (!_askToSaveChanges)
                return true;
            
            var sceneCount = EditorSceneManager.sceneCount;
            if (sceneCount == 0) 
                return true;
            
            _openedScenes = new Scene[sceneCount];
            
            for (var i = 0; i < sceneCount; i++)
            {
                _openedScenes[i] = EditorSceneManager.GetSceneAt(i);
            }

            if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(_openedScenes)) 
                return true;

            EditorUtility.DisplayDialog("Jump to scene", 
                $"Jump to `{_sceneSelectDropdown.value}` scene is cancelled", "Ok");
            return false;
        }

        private void RememberCurrentSetup()
        {
            _storedSceneSetup = EditorSceneManager.GetSceneManagerSetup();
        }

        private void RestorePreviousSetup()
        {
            if (_storedSceneSetup is null)
                return;
            _hasJumpedToPlaymode = false;
            EditorSceneManager.RestoreSceneManagerSetup(_storedSceneSetup);
        }

        private void SaveLastPlayedSetup()
        {
            ProjectPrefs.SetInt(last_played_setup_id_key, _lastPlayedSetup.Id);
            Debug.Log($"Saved last played scene setup {_lastPlayedSetup.name}");
        }

        private Option<ISceneSetup> LoadLastPlayedSetup()
        {
            var lastPlayedSetupId= ProjectPrefs.GetInt(last_played_setup_id_key);
            return _sceneSetups.Find(x => x.Id == lastPlayedSetupId) is { } sceneSetup? 
                    new (sceneSetup) : 
                    Option<ISceneSetup>.None;
        }

        void IToolbarAddonBehaviour.OnDestroy()
        {
            UnsubscribeFromSetupUpdates();
            _openedScenes = null;
            _storedSceneSetup = null;
            _sceneSetups.Clear();
            _rootVisualElement = null;
            _playButton = null;
            _loadButton = null;
            _saveSetupButton = null;
            _sceneSelectDropdown = null;
        }
    }
}