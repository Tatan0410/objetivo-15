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
    public string nombreCarpeta = "Objetivo15_Certificados";

    private string textoBotonOriginal = "📥 Descargar Certificado";

    void Start()
    {
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

        yield return new WaitForEndOfFrame();

        string nombreArchivo = "Certificado_" +
            (EstadisticasManager.instancia != null
                ? EstadisticasManager.instancia.nombreJugador.Replace(" ", "_")
                : "Jugador") +
            "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";

        string rutaCarpeta = Path.Combine(Application.persistentDataPath, nombreCarpeta);

        if (!Directory.Exists(rutaCarpeta))
            Directory.CreateDirectory(rutaCarpeta);

        string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

        ScreenCapture.CaptureScreenshot(rutaCompleta);

        // Esperar un frame extra para asegurar que la captura se procesó
        yield return new WaitForSeconds(0.3f);

        if (botonDescargar != null)
            botonDescargar.SetActive(true);

        MostrarConfirmacion(rutaCompleta);

        Debug.Log("Certificado guardado en: " + rutaCompleta);
    }

    void MostrarConfirmacion(string ruta)
    {
        if (panelConfirmacion == null) return;

        panelConfirmacion.SetActive(true);
        if (textoConfirmacion != null)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            textoConfirmacion.text = "¡Certificado guardado en tu dispositivo!";
#else
            textoConfirmacion.text = "¡Certificado guardado!\n" + ruta;
#endif
        }

        CancelInvoke(nameof(OcultarConfirmacion));
        Invoke(nameof(OcultarConfirmacion), 4f);
    }

    void OcultarConfirmacion()
    {
        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(false);
    }
}
