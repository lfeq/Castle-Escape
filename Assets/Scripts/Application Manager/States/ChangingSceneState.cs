using Application_Manager.Events; // Assuming this namespace is relevant, though not directly used in the visible code.
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Application_Manager.States {
    /// <summary>
    /// A ScriptableObject-based state that, when entered, loads a specified scene.
    /// This is typically used within a state machine managing application flow or scene transitions.
    /// </summary>
    // The [CreateAssetMenu] attribute allows easy creation of instances of this ScriptableObject
    // directly from the Assets/Create menu in the Unity Editor.
    // "menuRootName" is presumably a constant defined elsewhere, likely in BaseState or a shared constants file.
    [CreateAssetMenu(fileName = "New Changing Scene State", menuName = BaseState.menuRootName + "ChangingSceneState")]
    public class ChangingSceneState : BaseState {
        
        /// <summary>
        /// The name of the scene to load when this state is entered.
        /// This name **must** exactly match a scene that is included in the Build Settings.
        /// If the scene name is invalid or not in Build Settings, an error will occur at runtime.
        /// </summary>
        [Tooltip("The exact name of the scene to load. Must be in Build Settings.")]
        public string sceneName;

        /// <summary>
        /// Called when this state is entered by the state machine.
        /// This method attempts to load the scene specified by the <see cref="sceneName"/> field.
        /// </summary>
        public override void OnEnter() {
            // Calls the OnEnter method of the base class (BaseState).
            // The specific behavior of base.OnEnter() depends on the BaseState implementation.
            base.OnEnter();

            // Validate the scene name before attempting to load.
            if (string.IsNullOrEmpty(sceneName)) {
                Debug.LogError("ChangingSceneState: sceneName is null or empty. Cannot load scene. Please assign a valid scene name in the ScriptableObject asset.", this);
                // Depending on the state machine design, you might want to transition to an error state here
                // or notify a manager. For now, we just prevent the LoadScene call.
                return; 
            }

            // Synchronously loads the scene. This will freeze the application until the scene is fully loaded.
            // For smoother transitions, especially with larger scenes, consider asynchronous loading.
            // Example: StartCoroutine(LoadSceneAsyncWrapper(sceneName));
            // This would require the state machine or a manager to handle the async operation's lifecycle.
            Debug.Log($"ChangingSceneState: Loading scene '{sceneName}' synchronously.", this);
            SceneManager.LoadScene(sceneName);
            // Note: Once LoadScene is called, this current scene (and this state object if it's part of it)
            // will be destroyed unless it's marked as DontDestroyOnLoad. 
            // The state machine's subsequent behavior depends on how it's structured across scene loads.
        }

        // Example of an async loading wrapper (would require this script to be a MonoBehaviour or run by one):
        // private System.Collections.IEnumerator LoadSceneAsyncWrapper(string sceneToLoad) {
        //     Debug.Log($"ChangingSceneState: Starting asynchronous load of scene '{sceneToLoad}'.");
        //     AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        //
        //     // Optionally, update loading progress here if a loading screen is used.
        //     while (!asyncLoad.isDone) {
        //         // loadingProgress = asyncLoad.progress;
        //         yield return null;
        //     }
        //     Debug.Log($"ChangingSceneState: Scene '{sceneToLoad}' loaded asynchronously.");
        //     // Here, you might transition to another state or raise an event.
        // }
    }
}