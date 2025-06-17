
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    [Header("Button References")]
    public Button continueButton;
    public Button newGameButton;

    [Header("Scene Names")]
    public string newGameStartScene = "_SceneLong_Spr"; // The scene to load when starting a brand new game.

    private string saveFilePath;

    void Start()
    {
        saveFilePath = Application.persistentDataPath + "/gameSave_simplified_v2.json";

        if (continueButton != null)
        {
            continueButton.interactable = File.Exists(saveFilePath);
            continueButton.onClick.AddListener(OnContinueGameClicked);
        }

        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(OnNewGameClicked);
        }
    }

    public void OnNewGameClicked()
    {
        Debug.Log("[MainMenu] New Game button clicked. Resetting player data.");
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.StartNewGameReset();
        }
        // Always load the designated first scene for a new game.
        SceneManager.LoadScene(newGameStartScene);
    }

    public void OnContinueGameClicked()
    {
        Debug.Log("[MainMenu] Continue Game button clicked. Loading saved data.");
        if (PlayerDataManager.Instance != null)
        {
            // CRITICAL: Load the scene that was saved in the data file.
            string sceneToLoad = PlayerDataManager.Instance.sceneToLoadOnContinue;
            Debug.Log($"[MainMenu] Attempting to load last saved scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            // This case should ideally not happen if PDM is set up correctly.
            Debug.LogError("[MainMenu] PlayerDataManager.Instance not found! Cannot continue. Loading default scene as fallback.");
            SceneManager.LoadScene(newGameStartScene);
        }
    }

    public void OnQuitGameButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}