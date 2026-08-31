using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CambiarUsuarioManager : MonoBehaviour
{
    [Header("Botón principal (muestra nombre actual)")]
    public Button botonCambiarUsuario;
    public TMP_Text textoBotonNombre;

    [Header("Panel input nuevo nombre")]
    public GameObject panelInputCambiar;
    public TMP_InputField inputNuevoNombre;
    public Button botonConfirmarNombre;
    public Button botonCancelarInput;

    [Header("Panel confirmación cambio")]
    public GameObject panelConfirmacionCambiar;
    public TMP_Text textoConfirmacionCambiar;
    public Button botonSi;
    public Button botonNo;

    private string nombrePendiente;

    void OnEnable() => ActualizarTextoBoton();
    void Start() => ActualizarTextoBoton();

    public void ActualizarTextoBoton()
    {
        string nombre = EstadisticasManager.instancia != null
            ? EstadisticasManager.instancia.nombreJugador
            : PlayerPrefs.GetString("NombreJugador", "Jugador");
        if (textoBotonNombre != null)
            textoBotonNombre.text = nombre;
    }

    public void AbrirPanelCambiarNombre()
    {
        if (panelInputCambiar == null) return;
        panelInputCambiar.SetActive(true);
        if (inputNuevoNombre != null)
        {
            inputNuevoNombre.text = "";
            inputNuevoNombre.Select();
            inputNuevoNombre.ActivateInputField();
        }
        SeleccionUI.SeleccionarPrimero(panelInputCambiar);
    }

    public void CerrarPanelCambiarNombre()
    {
        if (panelInputCambiar != null)
            panelInputCambiar.SetActive(false);
        SeleccionUI.LimpiarSeleccion();
        if (botonCambiarUsuario != null)
            SeleccionUI.SeleccionarPrimero(botonCambiarUsuario.gameObject.transform.parent != null ? botonCambiarUsuario.transform.parent.gameObject : botonCambiarUsuario.gameObject);
        else
            ActualizarTextoBoton();
    }

    public void ConfirmarNuevoNombre()
    {
        nombrePendiente = inputNuevoNombre != null ? inputNuevoNombre.text : "Jugador";
        if (string.IsNullOrWhiteSpace(nombrePendiente)) nombrePendiente = "Jugador";

        if (panelInputCambiar != null)
            panelInputCambiar.SetActive(false);

        AbrirConfirmacionCambiar();
    }

    void AbrirConfirmacionCambiar()
    {
        if (panelConfirmacionCambiar == null) return;
        panelConfirmacionCambiar.SetActive(true);
        SeleccionUI.SeleccionarPrimero(panelConfirmacionCambiar);
        ConfigurarNavegacionConfirmacion(panelConfirmacionCambiar);
    }

    void ConfigurarNavegacionConfirmacion(GameObject panel)
    {
        var botones = panel.GetComponentsInChildren<Button>(true);
        if (botones.Length >= 2)
        {
            var nav0 = botones[0].navigation;
            var nav1 = botones[1].navigation;
            nav0.mode = Navigation.Mode.Explicit;
            nav1.mode = Navigation.Mode.Explicit;
            nav0.selectOnRight = botones[1];
            nav0.selectOnLeft = botones[1];
            nav1.selectOnRight = botones[0];
            nav1.selectOnLeft = botones[0];
            botones[0].navigation = nav0;
            botones[1].navigation = nav1;
        }
    }

    void Update()
    {
        if (panelConfirmacionCambiar != null && panelConfirmacionCambiar.activeSelf && Input.GetButtonDown("Cancel"))
        {
            CerrarConfirmacionCambiar();
            return;
        }
        if (panelInputCambiar != null && panelInputCambiar.activeSelf && Input.GetButtonDown("Cancel"))
        {
            CerrarPanelCambiarNombre();
        }
    }

    public void CerrarConfirmacionCambiar()
    {
        if (panelConfirmacionCambiar != null)
            panelConfirmacionCambiar.SetActive(false);
        SeleccionUI.LimpiarSeleccion();
        if (panelInputCambiar != null && panelInputCambiar.activeSelf)
            SeleccionUI.SeleccionarPrimero(panelInputCambiar);
        else if (botonCambiarUsuario != null)
            SeleccionUI.SeleccionarPrimero(botonCambiarUsuario.gameObject.transform.parent != null ? botonCambiarUsuario.transform.parent.gameObject : botonCambiarUsuario.gameObject);
    }

    public void SiCambiarUsuario()
    {
        float vol = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        bool teniaVol = PlayerPrefs.HasKey("VolumenMusica");
        PlayerPrefs.DeleteAll();
        if (teniaVol) PlayerPrefs.SetFloat("VolumenMusica", vol);
        PlayerPrefs.SetString("NombreJugador", nombrePendiente);
        PlayerPrefs.Save();

        if (EstadisticasManager.instancia != null)
        {
            EstadisticasManager.instancia.GuardarNombreJugador(nombrePendiente);
            EstadisticasManager.instancia.ReiniciarEstadisticas();
        }

        if (panelConfirmacionCambiar != null)
            panelConfirmacionCambiar.SetActive(false);
        if (panelInputCambiar != null)
            panelInputCambiar.SetActive(false);

        SeleccionUI.LimpiarSeleccion();
        ActualizarTextoBoton();

        var selector = FindFirstObjectByType<SelectorNivelesMapa>();
        if (selector != null) selector.RefrescarBotones();

        var manager = FindFirstObjectByType<MapamundialEstadoManager>();
        if (manager != null) manager.AplicarMapa();

        if (botonCambiarUsuario != null)
            SeleccionUI.SeleccionarPrimero(botonCambiarUsuario.gameObject.transform.parent != null ? botonCambiarUsuario.transform.parent.gameObject : botonCambiarUsuario.gameObject);
    }
}
