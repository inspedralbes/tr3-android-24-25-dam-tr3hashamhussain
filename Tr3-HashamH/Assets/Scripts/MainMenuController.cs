using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    private UIDocument uiDocument;
    private bool isQuitting = false;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;

        var root = uiDocument.rootVisualElement;
        if (root == null) return;

        // Configurar botones con protección contra null
        SafeSetupButton(root, "btn-facil", "M-Facil");
        SafeSetupButton(root, "btn-normal", "M-Normal");
        SafeSetupButton(root, "btn-dificil", "M-Dificil");
        SafeSetupButton(root, "btn-custom", "Custom-Mod");
        
        // Configurar botón de salida con doble protección
        var exitButton = root.Q<Button>("btn-exit");
        if (exitButton != null)
        {
            exitButton.clicked += () => {
                if (!isQuitting)
                {
                    isQuitting = true;
                    StartCoroutine(SafeQuitGame());
                }
            };
        }
    }

    private void SafeSetupButton(VisualElement root, string buttonName, string sceneName)
    {
        var button = root.Q<Button>(buttonName);
        if (button != null)
        {
            button.clicked += () => StartCoroutine(LoadSceneWithTransition(sceneName));
        }
    }

    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        // Efecto de transición opcional
        yield return new WaitForSecondsRealtime(0.3f);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator SafeQuitGame()
    {
        // 1. Deshabilitar el UIDocument primero
        if (uiDocument != null)
        {
            uiDocument.enabled = false;
        }

        // 2. Esperar 2 frames para que Unity complete las operaciones UI
        yield return null;
        yield return null;

        // 3. Forzar recolección de basura
        System.GC.Collect();
        yield return null;

        // 4. Salir de manera segura
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        // Limpieza adicional
        if (uiDocument != null)
        {
            uiDocument.enabled = false;
        }
    }
}