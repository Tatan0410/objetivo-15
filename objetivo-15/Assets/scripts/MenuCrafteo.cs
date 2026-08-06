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
        if (Input.GetKeyDown(teclaCrafteo))
            ToggleMenu();
    }

    void ToggleMenu()
    {
        if (panelCrafteo == null) return;
        menuAbierto = !menuAbierto;
        panelCrafteo.SetActive(menuAbierto);
        Time.timeScale = menuAbierto ? 0f : 1f;
    }

    public void AbrirCrafteo()
    {
        if (panelCrafteo == null) return;
        menuAbierto = true;
        panelCrafteo.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CerrarMenu()
    {
        menuAbierto = false;
        if (panelCrafteo != null)
            panelCrafteo.SetActive(false);
        Time.timeScale = 1f;
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
    }

    public void CraftearYCerrar()
    {
        if (SistemaCrafteo.instancia != null)
            SistemaCrafteo.instancia.CraftearLanzador();
        CerrarMenu();
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