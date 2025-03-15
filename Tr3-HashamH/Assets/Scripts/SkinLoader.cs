using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SkinLoader : MonoBehaviour
{
    public string skinUrl = "http://localhost:3000/current-skin"; // Endpoint para obtener la skin
    public SpriteRenderer birdSpriteRenderer; // Referencia al SpriteRenderer del pájaro
    public Sprite defaultSkin; // Skin por defecto (opcional)

    void Start()
    {
        StartCoroutine(LoadSkin());
    }

    IEnumerator LoadSkin()
    {
        // Obtener la URL de la skin desde el servidor
        using (UnityWebRequest webRequest = UnityWebRequest.Get(skinUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al obtener la skin: " + webRequest.error);
                ApplyDefaultSkin(); // Aplicar skin por defecto en caso de error
            }
            else
            {
                // Parsear la respuesta JSON
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Respuesta del servidor: " + jsonResponse); // Log para depuración
                SkinData skinData = JsonUtility.FromJson<SkinData>(jsonResponse);

                // Verificar si hay una skin disponible
                if (string.IsNullOrEmpty(skinData.imageUrl))
                {
                    Debug.Log("No hay skins disponibles. Usando skin por defecto.");
                    ApplyDefaultSkin(); // Aplicar skin por defecto
                }
                else
                {
                    // Cargar la imagen desde la URL
                    yield return StartCoroutine(LoadImage(skinData.imageUrl));
                }
            }
        }
    }

    IEnumerator LoadImage(string imageUrl)
    {
        Debug.Log("Cargando imagen desde: " + imageUrl); // Log para depuración
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al cargar la imagen: " + webRequest.error);
                ApplyDefaultSkin(); // Aplicar skin por defecto en caso de error
            }
            else
            {
                // Convertir la textura en un sprite
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                // Aplicar el sprite al pájaro
                if (birdSpriteRenderer != null)
                {
                    birdSpriteRenderer.sprite = sprite;
                }
                else
                {
                    Debug.LogError("No se asignó un SpriteRenderer al SkinLoader.");
                }
            }
        }
    }

    void ApplyDefaultSkin()
    {
        if (defaultSkin != null && birdSpriteRenderer != null)
        {
            birdSpriteRenderer.sprite = defaultSkin; // Aplicar skin por defecto
        }
        else
        {
            Debug.LogWarning("No se asignó una skin por defecto o un SpriteRenderer.");
        }
    }

    [System.Serializable]
    private class SkinData
    {
        public string imageUrl;
    }
}