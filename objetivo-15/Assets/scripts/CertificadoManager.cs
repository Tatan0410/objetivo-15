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
    public GameObject panelConfirmacion; // opcional: "¡Guardado!"
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
        if (SceneTransitionManager.instancia != null)
            SceneTransitionManager.instancia.CargarEscena("Mapamundial");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("Mapamundial");
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
        // Ocultar el botón de descarga y la UI de confirmación antes de capturar,
        // para que no aparezcan en la imagen final
        if (botonDescargar != null)
            botonDescargar.SetActive(false);

        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(false);

        yield return new WaitForEndOfFrame();

        string nombreArchivo = "Certificado_" +
            (EstadisticasManager.instancia != null
                ? EstadisticasManager.instancia.nombreJugador.Replace(" ", "_")
                : "Jugador") +
            "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";

        string rutaCarpeta = ObtenerRutaCaptura();

        if (!Directory.Exists(rutaCarpeta))
            Directory.CreateDirectory(rutaCarpeta);

        string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

        ScreenCapture.CaptureScreenshot(rutaCompleta);

        // Esperar a que la captura se escriba en disco (en Android es asincrona)
        float espera = 0f;
        while (!File.Exists(rutaCompleta) && espera < 3f)
        {
            espera += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (botonDescargar != null)
            botonDescargar.SetActive(true);

        string mensaje = ProcesarSegunPlataforma(rutaCompleta, nombreArchivo);
        MostrarConfirmacion(mensaje);
    }

    // Decide donde capturar segun la plataforma:
    //  - PC: directo en la carpeta Descargas del usuario.
    //  - Android: archivo temporal (luego se mueve a la galeria).
    string ObtenerRutaCaptura()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return Path.Combine(Application.persistentDataPath, "capturas_tmp");
#else
        string descargas = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile),
            "Downloads");

        if (Directory.Exists(descargas))
            return descargas;

        return Path.Combine(Application.persistentDataPath, nombreCarpeta);
#endif
    }

    string ProcesarSegunPlataforma(string rutaCompleta, string nombreArchivo)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (GuardarEnGaleriaAndroid(rutaCompleta, nombreArchivo))
        {
            // El temporal ya no se necesita, la copia vive en la galeria
            try { File.Delete(rutaCompleta); } catch { }
            return "¡Certificado guardado en tu galería!\n(Carpeta Pictures/" + nombreCarpeta + ")";
        }

        return "No se pudo guardar en la galería.\n(Tu versión de Android es muy antigua)";
#else
        Debug.Log("Certificado guardado en: " + rutaCompleta);
        return "¡Certificado guardado en Descargas!\n" + rutaCompleta;
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    // Inserta el PNG en la galeria usando MediaStore (sin permisos, Android 10+).
    bool GuardarEnGaleriaAndroid(string rutaArchivo, string nombreArchivo)
    {
        try
        {
            // MediaStore.Images solo funciona sin permisos desde Android 10 (API 29)
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                int sdk = version.GetStatic<int>("SDK_INT");
                if (sdk < 29)
                {
                    Debug.LogWarning("[Certificado] Android " + sdk + " < 29: no se puede guardar en galería sin permisos.");
                    return false;
                }
            }

            byte[] bytes = File.ReadAllBytes(rutaArchivo);
            if (bytes == null || bytes.Length == 0)
                return false;

            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject resolver = activity.Call<AndroidJavaObject>("getContentResolver");

                using (var values = new AndroidJavaObject("android.content.ContentValues"))
                {
                    values.Call<AndroidJavaObject>("put", "_display_name", nombreArchivo);
                    values.Call<AndroidJavaObject>("put", "mime_type", "image/png");
                    values.Call<AndroidJavaObject>("put", "relative_path", "Pictures/" + nombreCarpeta);

                    AndroidJavaObject coleccion;
                    using (var mediaStore = new AndroidJavaClass("android.provider.MediaStore$Images$Media"))
                        coleccion = mediaStore.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI");

                    AndroidJavaObject uri = resolver.Call<AndroidJavaObject>("insert", coleccion, values);
                    if (uri == null)
                        return false;

                    AndroidJavaObject stream = resolver.Call<AndroidJavaObject>("openOutputStream", uri);
                    if (stream == null)
                        return false;

                    stream.Call("write", new object[] { bytes });
                    stream.Call("flush");
                    stream.Call("close");
                }
            }
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[Certificado] Error guardando en galería: " + e.Message);
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
