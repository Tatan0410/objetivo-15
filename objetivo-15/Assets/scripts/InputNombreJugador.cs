using UnityEngine;
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
