using UnityEngine;
using TMPro;
using System.Collections;
using System.IO;

public class CertificadoManager : MonoBehaviour
{
    [Header("UI - Textos del certificado")]
    public TMP_Text textoNombreJugador;
    public TMP_Text textoPlasticos;
    public TMP_Text textoEnemigos;
    public TMP_Text textoTiempo;

    [Header("UI - Botón de descarga")]
    public GameObject botonDescargar;
    public TMP_Text textoBotonDescargar;

    [Header("UI - Confirmación")]
    public GameObject panelConfirmacion;
    public TMP_Text textoConfirmacion;

    [Header("Configuración de captura")]
    [Tooltip("Carpeta dentro de Pictures donde se guarda el certificado en Android")]
    public string nombreCarpeta = "Objetivo15_Certificados";

    private string textoBotonOriginal = "📥 Descargar Certificado";

    void Start()
    {
        // Asegurar que el fondo quede detrás como en menuprincipal (primer hermano)
        var fondo = GameObject.Find("FondoCertificado");
        if (fondo != null)
        {
            fondo.transform.SetAsFirstSibling();
            var img = fondo.GetComponent<UnityEngine.UI.Image>();
            if (img != null) img.raycastTarget = false;
        }

        MostrarEstadisticas();

        // Snapshot de estadísticas: el certificado se puede revisar más tarde
        // (desde el mapa limpio) aunque se cierre el juego
        if (EstadisticasManager.instancia != null)
            EstadisticasManager.instancia.GuardarEstadisticas();

        // Seleccionar primer botón para navegación con mando/teclado
        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            SeleccionUI.SeleccionarPrimero(canvas.gameObject);
        else
        {
            var btn = botonDescargar != null ? botonDescargar : GameObject.Find("BotonDescargar");
            if (btn != null) SeleccionUI.SeleccionarPrimero(btn.transform.parent != null ? btn.transform.parent.gameObject : btn);
        }
    }

    public void Continuar()
    {
        SceneTransitionManager.CargarEscenaConFallback("Mapamundial");
    }

    void MostrarEstadisticas()
    {
        if (EstadisticasManager.instancia == null)
        {
            Debug.LogWarning("EstadisticasManager no encontrado");
            return;
        }

        var stats = EstadisticasManager.instancia;

        if (textoNombreJugador != null)
            textoNombreJugador.text = stats.nombreJugador;

        if (textoPlasticos != null)
            textoPlasticos.text = stats.totalPlasticosReciclados.ToString();

        if (textoEnemigos != null)
            textoEnemigos.text = stats.totalEnemigosDerrotados.ToString();

        if (textoTiempo != null)
            textoTiempo.text = stats.ObtenerTiempoFormateado();

        if (textoBotonDescargar != null)
            textoBotonOriginal = textoBotonDescargar.text;

        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(false);
    }

    // ── Botón: Descargar certificado ──────────────────────────────────────────
    public void DescargarCertificado()
    {
        StartCoroutine(CapturarYGuardar());
    }

    IEnumerator CapturarYGuardar()
    {
        // Ocultar botón y confirmación para que no salgan en la imagen
        OcultarBotones();
        yield return new WaitForEndOfFrame();

        byte[] png = CapturarPNG();
        RestaurarBotones();

        if (png == null || png.Length == 0)
        {
            MostrarConfirmacion("No se pudo capturar el certificado.");
            yield break;
        }

        GuardarPNG(png);
    }

    void OcultarBotones()
    {
        if (botonDescargar != null) botonDescargar.SetActive(false);
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
    }

    void RestaurarBotones()
    {
        if (botonDescargar != null) botonDescargar.SetActive(true);
    }

    // Captura síncrona y confiable (sin la carrera de archivos de CaptureScreenshot)
    byte[] CapturarPNG()
    {
        try
        {
            Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
            byte[] png = tex.EncodeToPNG();
            Object.Destroy(tex);
            return png;
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Certificado] Error capturando imagen: " + e);
            return null;
        }
    }

    string NombreArchivoPNG()
    {
        return "Certificado_" +
            (EstadisticasManager.instancia != null && !string.IsNullOrEmpty(EstadisticasManager.instancia.nombreJugador)
                ? EstadisticasManager.instancia.nombreJugador.Replace(" ", "_")
                : "Jugador") +
            "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
    }

    // ── Guardar en disco/galería ──────────────────────────────────────────────
    void GuardarPNG(byte[] png)
    {
        string nombreArchivo = NombreArchivoPNG();

#if UNITY_ANDROID && !UNITY_EDITOR
        // 1) MediaStore (Android 10+): inserta directo en la galería sin permisos
        if (GuardarEnGaleriaAndroid(png, nombreArchivo))
        {
            MostrarConfirmacion("¡Certificado guardado en tu galería!\n(Carpeta Pictures/" + nombreCarpeta + ")");
            return;
        }

        // 2) Fallback para Android antiguo (<10): permiso + escritura directa
        if (GuardarEnGaleriaLegacy(png, nombreArchivo))
        {
            MostrarConfirmacion("¡Certificado guardado en Pictures/" + nombreCarpeta + "!");
            return;
        }

        // 3) Último recurso: carpeta privada de la app
        try
        {
            string rutaPrivada = Path.Combine(Application.persistentDataPath, nombreArchivo);
            File.WriteAllBytes(rutaPrivada, png);
            Debug.Log("[Certificado] Guardado en carpeta privada: " + rutaPrivada);
            MostrarConfirmacion("No se pudo usar la galería.\nSe guardó en la app.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Certificado] Error guardando: " + e);
            MostrarConfirmacion("Error guardando: " + e.Message);
        }
#else
        // PC: descargas del usuario
        try
        {
            string descargas = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile),
                "Downloads");
            if (!Directory.Exists(descargas))
                descargas = Application.persistentDataPath;

            string rutaCompleta = Path.Combine(descargas, nombreArchivo);
            File.WriteAllBytes(rutaCompleta, png);
            Debug.Log("[Certificado] Guardado en: " + rutaCompleta);
            MostrarConfirmacion("¡Certificado guardado en Descargas!\n" + rutaCompleta);
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Certificado] Error guardando: " + e);
            MostrarConfirmacion("Error guardando: " + e.Message);
        }
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    // Inserta el PNG en la galería usando MediaStore (sin permisos, Android 10+).
    bool GuardarEnGaleriaAndroid(byte[] bytes, string nombreArchivo)
    {
        try
        {
            // MediaStore.Images solo funciona sin permisos desde Android 10 (API 29)
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                int sdk = version.GetStatic<int>("SDK_INT");
                if (sdk < 29)
                {
                    Debug.LogWarning("[Certificado] Android " + sdk + " < 29: usando escritura directa en Pictures.");
                    return false;
                }
            }

            if (bytes == null || bytes.Length == 0)
                return false;

            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject resolver = activity.Call<AndroidJavaObject>("getContentResolver");

                using (var values = new AndroidJavaObject("android.content.ContentValues"))
                {
                    // IMPORTANTE: ContentValues.put devuelve void -> usar Call (no Call<T>)
                    values.Call("put", "_display_name", nombreArchivo);
                    values.Call("put", "mime_type", "image/png");
                    values.Call("put", "relative_path", "Pictures/" + nombreCarpeta);

                    AndroidJavaObject coleccion;
                    using (var mediaStore = new AndroidJavaClass("android.provider.MediaStore$Images$Media"))
                        coleccion = mediaStore.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI");

                    AndroidJavaObject uri = resolver.Call<AndroidJavaObject>("insert", coleccion, values);
                    if (uri == null)
                    {
                        Debug.LogError("[Certificado] MediaStore insert devolvió null");
                        return false;
                    }

                    AndroidJavaObject stream = resolver.Call<AndroidJavaObject>("openOutputStream", uri);
                    if (stream == null)
                    {
                        Debug.LogError("[Certificado] openOutputStream devolvió null");
                        return false;
                    }

                    stream.Call("write", new object[] { bytes });
                    stream.Call("flush");
                    stream.Call("close");
                }
            }
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Certificado] Error guardando en galería: " + e);
            return false;
        }
    }

    // Android < 10: pide permiso de almacenamiento y escribe directo en Pictures/
    bool GuardarEnGaleriaLegacy(byte[] png, string nombreArchivo)
    {
        try
        {
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                int sdk = version.GetStatic<int>("SDK_INT");
                if (sdk >= 29)
                    return false; // >=10 usa MediaStore (ya falló arriba, no insistir)
            }

            if (!UnityEngine.Permission.HasUserAuthorizedPermission(UnityEngine.Permission.ExternalStorageWrite))
            {
                UnityEngine.Permission.RequestUserPermission(UnityEngine.Permission.ExternalStorageWrite);
                MostrarConfirmacion("Permite el acceso al almacenamiento\ny vuelve a tocar Descargar.");
                return false;
            }

            using (var env = new AndroidJavaClass("android.os.Environment"))
            using (var dir = env.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", "Pictures"))
            {
                string carpeta = Path.Combine(dir.Call<string>("getAbsolutePath"), nombreCarpeta);
                Directory.CreateDirectory(carpeta);
                string ruta = Path.Combine(carpeta, nombreArchivo);
                File.WriteAllBytes(ruta, png);
                Debug.Log("[Certificado] Guardado (legacy): " + ruta);
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Certificado] Error guardando (legacy): " + e);
            return false;
        }
    }
#endif

    void MostrarConfirmacion(string mensaje)
    {
        if (panelConfirmacion == null) return;

        panelConfirmacion.SetActive(true);
        if (textoConfirmacion != null)
            textoConfirmacion.text = mensaje;

        CancelInvoke(nameof(OcultarConfirmacion));
        Invoke(nameof(OcultarConfirmacion), 4f);
    }

    void OcultarConfirmacion()
    {
        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(false);
    }
}
