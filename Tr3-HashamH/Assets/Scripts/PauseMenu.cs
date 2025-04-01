using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(UIDocument))]
public class PauseMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private float fadeDuration = 0.3f;

    private UIDocument uiDocument;
    private VisualElement root;
    private VisualElement pauseMenu;
    private bool isPaused = false;
    private bool isInitialized = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeUI();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnregisterButtonEvents();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reinicializamos la UI cuando se carga una nueva escena
        isInitialized = false;
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (isInitialized) return;

        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component missing!");
            return;
        }

        // Forzar reinicio del UIDocument
        uiDocument.enabled = false;
        uiDocument.enabled = true;

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Root VisualElement not found!");
            return;
        }

        pauseMenu = root.Q<VisualElement>("PauseMenu");
        if (pauseMenu == null)
        {
            Debug.LogError("PauseMenu element not found!");
            return;
        }

        // Configuración inicial
        pauseMenu.style.display = DisplayStyle.None;
        pauseMenu.style.opacity = 0;
        pauseMenu.pickingMode = PickingMode.Position;

        // Registrar eventos
        RegisterButtonEvents();

        isInitialized = true;
        Debug.Log("UI initialized successfully");
    }

    private void RegisterButtonEvents()
    {
        var resumeButton = pauseMenu.Q<Button>("ResumeButton");
        if (resumeButton != null)
        {
            resumeButton.clicked += ResumeGame;
            Debug.Log("Resume button registered");
        }

        var optionsButton = pauseMenu.Q<Button>("OptionsButton");
        if (optionsButton != null)
        {
            optionsButton.clicked += ShowOptions;
            Debug.Log("Options button registered");
        }

        var mainMenuButton = pauseMenu.Q<Button>("MainMenuButton");
        if (mainMenuButton != null)
        {
            mainMenuButton.clicked += GoToMainMenu;
            Debug.Log("Main menu button registered");
        }
    }

    private void UnregisterButtonEvents()
    {
        if (!isInitialized || pauseMenu == null) return;

        var resumeButton = pauseMenu.Q<Button>("ResumeButton");
        if (resumeButton != null) resumeButton.clicked -= ResumeGame;

        var optionsButton = pauseMenu.Q<Button>("OptionsButton");
        if (optionsButton != null) optionsButton.clicked -= ShowOptions;

        var mainMenuButton = pauseMenu.Q<Button>("MainMenuButton");
        if (mainMenuButton != null) mainMenuButton.clicked -= GoToMainMenu;
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused) PauseGame();
        else ResumeGame();
    }

    private void PauseGame()
    {
        if (!isInitialized) return;

        Time.timeScale = 0f;
        ShowPauseMenu();
        AudioListener.pause = true;
        Debug.Log("Game paused");
    }

    private void ResumeGame()
    {
        if (!isInitialized) return;

        Time.timeScale = 1f;
        HidePauseMenu();
        AudioListener.pause = false;
        Debug.Log("Game resumed");
    }

    private void ShowPauseMenu()
    {
        if (!isInitialized) return;

        pauseMenu.style.display = DisplayStyle.Flex;
        StartCoroutine(FadeElement(pauseMenu, 0f, 1f, fadeDuration));
        Debug.Log("Showing pause menu");
    }

    private void HidePauseMenu()
    {
        if (!isInitialized) return;

        StartCoroutine(FadeElement(pauseMenu, 1f, 0f, fadeDuration, () => 
        {
            pauseMenu.style.display = DisplayStyle.None;
            Debug.Log("Hiding pause menu");
        }));
    }

    private IEnumerator FadeElement(VisualElement element, float start, float end, float duration, System.Action onComplete = null)
    {
        float elapsed = 0f;
        element.style.opacity = start;

        while (elapsed < duration)
        {
            element.style.opacity = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        element.style.opacity = end;
        onComplete?.Invoke();
    }

    private void ShowOptions()
    {
        Debug.Log("Options menu opened");
        // Implementa tu lógica de opciones aquí
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Returning to main menu");
    }

    private void OnDestroy()
    {
        UnregisterButtonEvents();
    }
}