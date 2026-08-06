using UnityEngine;
using UnityEngine.UI;

public class MenuCrafteo : MonoBehaviour
{
    public static MenuCrafteo instancia;
    public KeyCode teclaCrafteo = KeyCode.Tab;

    [Header("Panel prefab editable")]
    public GameObject panelCrafteoPrefab;

    private GameObject panelCrafteo;
    private bool menuAbierto = false;

    void Awake() => instancia = this;

    void Start()
    {
        InstanciarPanel();
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

    public void CerrarMenu()
    {
        menuAbierto = false;
        if (panelCrafteo != null)
            panelCrafteo.SetActive(false);
        Time.timeScale = 1f;
    }

    void InstanciarPanel()
    {
        if (panelCrafteoPrefab == null)
        {
            Debug.LogError("MenuCrafteo: No se asignó panelCrafteoPrefab en el Inspector.");
            return;
        }

        panelCrafteo = Instantiate(panelCrafteoPrefab);
        panelCrafteo.name = "PanelCrafteo";

        Button btnLanzador = BuscarBoton(panelCrafteo.transform, "Btn_CrafteoLanzador");
        if (btnLanzador != null)
        {
            btnLanzador.onClick.AddListener(CraftearYCerrar);
        }
        else
        {
            Debug.LogWarning("MenuCrafteo: No se encontró el botón Btn_CrafteoLanzador en el prefab.");
        }
    }

    void CraftearYCerrar()
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