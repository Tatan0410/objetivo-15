using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InputNombreJugador : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelInputNombre;
    public TMP_InputField inputNombre;
    public Button botonConfirmar;

    void Start()
    {
        // Solo mostrar el panel si el jugador no tiene nombre guardado todavía
        bool yaTieneNombre = EstadisticasManager.TieneNombreGuardado();

        if (panelInputNombre != null)
            panelInputNombre.SetActive(!yaTieneNombre);

        if (botonConfirmar != null)
            botonConfirmar.onClick.AddListener(ConfirmarNombre);

        // Si el panel se va a mostrar, llevar la selección dentro de él para
        // que el control/teclado no pueda mover el menú de atrás
        if (panelInputNombre != null && panelInputNombre.activeSelf)
            SeleccionUI.SeleccionarPrimero(panelInputNombre);
    }

    void Update()
    {
        // Contención modal: mientras el panel de nombre esté activo, la selección
        // del EventSystem no puede salirse del panel (bloquea navegación al menú)
        if (panelInputNombre == null || !panelInputNombre.activeInHierarchy) return;

        var es = EventSystem.current;
        var sel = es != null ? es.currentSelectedGameObject : null;
        if (sel == null || !sel.transform.IsChildOf(panelInputNombre.transform))
            SeleccionUI.SeleccionarPrimero(panelInputNombre);
    }

    public void ConfirmarNombre()
    {
        string nombre = inputNombre != null ? inputNombre.text : "Jugador";

        if (EstadisticasManager.instancia != null)
            EstadisticasManager.instancia.GuardarNombreJugador(nombre);
        else
        {
            // Fallback por si el manager aún no existe en esta escena
            if (string.IsNullOrWhiteSpace(nombre)) nombre = "Jugador";
            PlayerPrefs.SetString("NombreJugador", nombre);
            PlayerPrefs.Save();
        }

        if (panelInputNombre != null)
            panelInputNombre.SetActive(false);

        var mgr = FindFirstObjectByType<CambiarUsuarioManager>();
        if (mgr != null) mgr.ActualizarTextoBoton();
    }
}
