using UnityEngine;
using UnityEngine.UI;

public class MenuCrafteo : MonoBehaviour
{
    public static MenuCrafteo instancia;
    public KeyCode teclaCrafteo = KeyCode.Tab;

    [Header("Panel fijo en escena (editable)")]
    public GameObject panelCrafteo;

    private bool menuAbierto = false;

    void Awake() => instancia = this;

    void Start()
    {
        ConectarBotonesPanel();
        if (panelCrafteo != null)
            panelCrafteo.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaCrafteo) || Input.GetButtonDown("Craft"))
            ToggleMenu();
        else if (menuAbierto && Input.GetButtonDown("Cancel"))
            CerrarMenu();
    }

    static bool HayPausaActiva()
    {
        return MenuPausa.instancia != null && MenuPausa.JuegoPausado;
    }

    void ActualizarTimeScale()
    {
        Time.timeScale = (menuAbierto || HayPausaActiva()) ? 0f : 1f;
    }

    void ToggleMenu()
    {
        if (panelCrafteo == null) return;
        menuAbierto = !menuAbierto;
        panelCrafteo.SetActive(menuAbierto);
        if (menuAbierto)
            SeleccionUI.SeleccionarPrimero(panelCrafteo);
        else
            SeleccionUI.LimpiarSeleccion();
        ActualizarTimeScale();
    }

    public void AbrirCrafteo()
    {
        if (panelCrafteo == null) return;
        menuAbierto = true;
        panelCrafteo.SetActive(true);
        SeleccionUI.SeleccionarPrimero(panelCrafteo);
        ActualizarTimeScale();
    }

    public void CerrarMenu()
    {
        menuAbierto = false;
        if (panelCrafteo != null)
            panelCrafteo.SetActive(false);
        SeleccionUI.LimpiarSeleccion();
        ActualizarTimeScale();
    }

    void ConectarBotonesPanel()
    {
        if (panelCrafteo == null)
        {
            Debug.LogError("MenuCrafteo: No se asignó panelCrafteo en el Inspector.");
            return;
        }

        Button btnLanzador = BuscarBoton(panelCrafteo.transform, "Btn_CrafteoLanzador");
        if (btnLanzador != null)
        {
            btnLanzador.onClick.AddListener(CraftearYCerrar);
        }
        else
        {
            Debug.LogWarning("MenuCrafteo: No se encontró el botón Btn_CrafteoLanzador en el panel.");
        }

        Button btnCerrar = BuscarBoton(panelCrafteo.transform, "BtnCerrarCrafteo");
        if (btnCerrar != null)
        {
            btnCerrar.onClick.AddListener(CerrarMenu);
        }
        else
        {
            Debug.LogWarning("MenuCrafteo: No se encontró el botón BtnCerrarCrafteo en el panel.");
        }

        ConectarBotonBalas(panelCrafteo.transform, "Btn_CraftearBalaComun", () =>
            SistemaCrafteo.instancia.CraftearCartuchoComun());
        ConectarBotonBalas(panelCrafteo.transform, "Btn_CraftearBalaEpica", () =>
            SistemaCrafteo.instancia.CraftearCartuchoEpica());
        ConectarBotonBalas(panelCrafteo.transform, "Btn_CraftearBalaLegendaria", () =>
            SistemaCrafteo.instancia.CraftearCartuchoLegendaria());
    }

    void ConectarBotonBalas(Transform panel, string nombre, UnityEngine.Events.UnityAction accion)
    {
        Button btn = BuscarBoton(panel, nombre);
        if (btn != null)
            btn.onClick.AddListener(accion);
        else
            Debug.LogWarning($"MenuCrafteo: No se encontró el botón {nombre} en el panel.");
    }

    public void CraftearYCerrar()
    {
        if (SistemaCrafteo.instancia != null)
            SistemaCrafteo.instancia.CraftearLanzador();
        CerrarMenu();
    }

    public void CraftearBalaComun()
    {
        if (SistemaCrafteo.instancia != null)
            SistemaCrafteo.instancia.CraftearCartuchoComun();
    }

    public void CraftearBalaEpica()
    {
        if (SistemaCrafteo.instancia != null)
            SistemaCrafteo.instancia.CraftearCartuchoEpica();
    }

    public void CraftearBalaLegendaria()
    {
        if (SistemaCrafteo.instancia != null)
            SistemaCrafteo.instancia.CraftearCartuchoLegendaria();
    }

    Transform BuscarTransformEn(Transform parent, string nombre)
    {
        Transform t = parent.Find(nombre);
        if (t == null)
        {
            foreach (Transform child in parent)
            {
                t = BuscarTransformEn(child, nombre);
                if (t != null) break;
            }
        }
        return t;
    }

    Button BuscarBoton(Transform parent, string nombre)
    {
        Transform t = BuscarTransformEn(parent, nombre);
        return t?.GetComponent<Button>();
    }
}