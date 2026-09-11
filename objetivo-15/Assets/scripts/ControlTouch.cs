using UnityEngine;

public class ControlTouch : MonoBehaviour
{
    public static ControlTouch instancia;

    [Header("Controles")]
    public GameObject botonDisparo;
    public GameObject botonCambioBala;

    [Tooltip("Si es false, los controles solo se muestran en plataformas moviles.")]
    public bool forzarEnEditor = true;

    static bool izquierda;
    static bool derecha;
    static bool saltar;
    static bool disparar;
    static bool atacar;

    public static float Horizontal => (derecha ? 1f : 0f) - (izquierda ? 1f : 0f);

    void Awake()
    {
        if (instancia == null)
            instancia = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
#if UNITY_EDITOR
        // En editor se respeta forzarEnEditor para poder testear el APK en PC
        if (!forzarEnEditor && !Application.isMobilePlatform)
            gameObject.SetActive(false);
#else
        // Builds: los botones táctiles solo se muestran en móviles (PC queda limpio)
        if (!Application.isMobilePlatform)
            gameObject.SetActive(false);
#endif
        ActualizarVisibilidadArmado();
    }

    void Update()
    {
        ActualizarVisibilidadArmado();
    }

    void OnDisable()
    {
        izquierda = derecha = saltar = disparar = atacar = false;
    }

    public void ApretarIzquierda() { izquierda = true; }
    public void SoltarIzquierda() { izquierda = false; }
    public void ApretarDerecha() { derecha = true; }
    public void SoltarDerecha() { derecha = false; }

    public void ApretarSaltar() { saltar = true; }
    public void ApretarDisparo() { disparar = true; }
    public void ApretarAtaque() { atacar = true; }

    public void CambiarBala()
    {
        if (MunicionManager.instancia != null)
            MunicionManager.instancia.CambiarTipo();
    }

    public static bool ConsumirSaltar() { bool v = saltar; saltar = false; return v; }
    public static bool ConsumirDisparo() { bool v = disparar; disparar = false; return v; }
    public static bool ConsumirAtaque() { bool v = atacar; atacar = false; return v; }

    void ActualizarVisibilidadArmado()
    {
        if (botonDisparo == null && botonCambioBala == null) return;

        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        bool armado = jugador != null && jugador.GetComponent<PlayerController>() is PlayerController pc
                      && pc.armaEquipada == TipoArma.Lanzador;

        if (botonDisparo != null && botonDisparo.activeSelf != armado)
            botonDisparo.SetActive(armado);
        if (botonCambioBala != null && botonCambioBala.activeSelf != armado)
            botonCambioBala.SetActive(armado);
    }
}