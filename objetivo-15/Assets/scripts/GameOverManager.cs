using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instancia;

    [Header("Panel prefab editable")]
    public GameObject panelGameOverPrefab;

    private GameObject canvasGameOver;
    private string escenaActual;

    void Awake()
    {
        if (instancia != null) { Destroy(gameObject); return; }
        instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        BuscarOCrearCanvas();
        if (canvasGameOver != null)
            canvasGameOver.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnEscenaCargada;
    }

    void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnEscenaCargada;
    }

    void OnEscenaCargada(UnityEngine.SceneManagement.Scene escena, UnityEngine.SceneManagement.LoadSceneMode modo)
    {
        if (escena.name.StartsWith("nivel") && canvasGameOver != null)
            canvasGameOver.SetActive(false);
        if (escena.name.StartsWith("nivel"))
            UnityEngine.Time.timeScale = 1f;
    }

    void BuscarOCrearCanvas()
    {
        Transform existing = transform.Find("CanvasGameOver");
        if (existing != null)
        {
            canvasGameOver = existing.gameObject;
            DontDestroyOnLoad(canvasGameOver);
            return;
        }

        if (panelGameOverPrefab != null)
        {
            canvasGameOver = Instantiate(panelGameOverPrefab);
            canvasGameOver.name = "CanvasGameOver";
            canvasGameOver.transform.SetParent(null);
            DontDestroyOnLoad(canvasGameOver);

            Button btnReintentar = BuscarBoton(canvasGameOver.transform, "Btn_Reintentar");
            if (btnReintentar != null) btnReintentar.onClick.AddListener(Reintentar);

            Button btnMenu = BuscarBoton(canvasGameOver.transform, "Btn_Menú Principal");
            if (btnMenu != null) btnMenu.onClick.AddListener(IrAlMenu);

            return;
        }

        Debug.LogError("GameOverManager: No se encontró el prefab GameOverPanel. Asigna panelGameOverPrefab en el Inspector.");
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

    public void MostrarGameOver()
    {
        escenaActual = SceneManager.GetActiveScene().name;
        canvasGameOver.SetActive(true);
        SeleccionUI.SeleccionarPrimero(canvasGameOver);
        Time.timeScale = 0f;
    }

    public void Reintentar()
    {
        canvasGameOver.SetActive(false);
        SeleccionUI.LimpiarSeleccion();
        Time.timeScale = 1f;
        SceneTransitionManager.CargarEscenaConFallback(escenaActual);
    }

    public void IrAlMenu()
    {
        canvasGameOver.SetActive(false);
        SeleccionUI.LimpiarSeleccion();
        Time.timeScale = 1f;
        SceneTransitionManager.CargarEscenaConFallback("menuprincipal");
    }
}
