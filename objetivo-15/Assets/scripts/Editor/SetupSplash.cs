using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

// Crea la escena splash.unity: portadajuego.png a pantalla completa
// durante 3 segundos (made with unity sigue apareciendo antes) y
// luego carga menuprincipal. Tambien la inserta como escena 0
// en Build Settings.
public static class SetupSplash
{
    private const string RUTA_ESCENA = "Assets/Scenes/splash.unity";

    [MenuItem("Objetivo15/Agregar Fondo al Splash")]
    public static void AgregarFondoSplash()
    {
        // Abrir la escena splash si no es la activa
        if (EditorSceneManager.GetActiveScene().path != RUTA_ESCENA)
            EditorSceneManager.OpenScene(RUTA_ESCENA);

        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("[SetupSplash] No se encontro Canvas en la escena splash.");
            return;
        }

        // No duplicar si ya existe
        if (canvas.transform.Find("FondoSplash") != null)
        {
            Debug.Log("[SetupSplash] Ya existe 'FondoSplash'. No se duplico.");
            return;
        }

        var fondo = CrearFondoSplash(canvas.transform);
        fondo.transform.SetAsFirstSibling(); // siempre detras de la Portada

        if (fondo == null)
        {
            Debug.LogWarning("[SetupSplash] No se encontro Assets/sprites/cielocontaminado.png; fondo no creado.");
            return;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), RUTA_ESCENA);
        Debug.Log("[SetupSplash] FondoSplash agregado a splash.unity (detras de Portada).");

        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Listo", "Fondo agregado al splash detrás de la portada.", "OK");
    }

    // Imagen fullscreen con el cielo contaminado, detras de la portada.
    private static GameObject CrearFondoSplash(Transform canvasTransform)
    {
        var spriteFondo = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/cielocontaminado.png");
        if (spriteFondo == null) return null;

        var fondoGO = new GameObject("FondoSplash", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fondoGO.transform.SetParent(canvasTransform, false);
        var rt = fondoGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var img = fondoGO.GetComponent<Image>();
        img.sprite = spriteFondo;
        img.color = Color.white;
        img.raycastTarget = false;
        img.preserveAspect = false;
        return fondoGO;
    }

    [MenuItem("Objetivo15/Crear Escena Splash")]
    public static void CrearEscenaSplash()
    {
        string directorio = Path.GetDirectoryName(RUTA_ESCENA);
        if (!Directory.Exists(directorio))
            Directory.CreateDirectory(directorio);

        // Escena nueva vacia
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camara (fondo negro por si la imagen no cubre todo)
        var camGO = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        camGO.tag = "MainCamera";
        camGO.transform.position = new Vector3(0, 0, -10);
        var cam = camGO.GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;

        // Canvas a pantalla completa (mismos parametros que el resto del juego)
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // Fondo detras de la portada (visible si la portada no cubre todo)
        CrearFondoSplash(canvasGO.transform);

        // Imagen con la portada estirada a pantalla completa
        var spritePortada = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/portadajuego.png");
        var imgGO = new GameObject("Portada", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imgGO.transform.SetParent(canvasGO.transform, false);
        var imgRT = imgGO.GetComponent<RectTransform>();
        imgRT.anchorMin = Vector2.zero;
        imgRT.anchorMax = Vector2.one;
        imgRT.offsetMin = Vector2.zero;
        imgRT.offsetMax = Vector2.zero;
        var img = imgGO.GetComponent<Image>();
        img.sprite = spritePortada;
        img.color = Color.white;
        img.raycastTarget = false;
        img.preserveAspect = false;

        if (spritePortada == null)
            Debug.LogWarning("[SetupSplash] No se encontro Assets/portadajuego.png; la escena muestra fondo negro.");

        // Manager con el script del splash
        var mgrGO = new GameObject("SplashManager");
        mgrGO.AddComponent<SplashScreen>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, RUTA_ESCENA);
        Debug.Log("[SetupSplash] Escena splash creada en " + RUTA_ESCENA);

        // Insertar splash como escena 0 en Build Settings (sin duplicar)
        var escenas = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        escenas.RemoveAll(e => e.path == RUTA_ESCENA);
        escenas.Insert(0, new EditorBuildSettingsScene(RUTA_ESCENA, true));
        EditorBuildSettings.scenes = escenas.ToArray();

        Debug.Log("[SetupSplash] splash.unity agregada como escena 0 en Build Settings.");

        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Listo", "Escena splash creada y agregada como escena 0 en Build Settings.", "OK");
    }
}
