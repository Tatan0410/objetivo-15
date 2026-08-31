using UnityEngine;

public class EstadisticasManager : MonoBehaviour
{
    public static EstadisticasManager instancia;

    [Header("Estadísticas de la partida")]
    public int totalPlasticosReciclados = 0;
    public int totalEnemigosDerrotados = 0;
    public float tiempoJugado = 0f; // en segundos

    [Header("Jugador")]
    public string nombreJugador = "Jugador";

    private bool tiempoActivo = true;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Cargar nombre guardado si existe
        if (PlayerPrefs.HasKey("NombreJugador"))
            nombreJugador = PlayerPrefs.GetString("NombreJugador");
    }

    void Update()
    {
        // Solo suma tiempo si el juego no está pausado (Time.timeScale > 0)
        // y no estamos en el menú principal o pantalla de créditos/certificado
        if (tiempoActivo && Time.timeScale > 0f)
        {
            tiempoJugado += Time.deltaTime;
        }
    }

    // ── Registro de eventos ──────────────────────────────────────────────────

    public void RegistrarPlastico()
    {
        totalPlasticosReciclados++;
    }

    public void RegistrarEnemigoDerrotado()
    {
        totalEnemigosDerrotados++;
    }

    // ── Control de tiempo ────────────────────────────────────────────────────

    public void PausarConteoTiempo()
    {
        tiempoActivo = false;
    }

    public void ReanudarConteoTiempo()
    {
        tiempoActivo = true;
    }

    // ── Nombre del jugador ───────────────────────────────────────────────────

    public void GuardarNombreJugador(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            nombre = "Jugador";

        nombreJugador = nombre;
        PlayerPrefs.SetString("NombreJugador", nombre);
        PlayerPrefs.Save();
    }

    public static bool TieneNombreGuardado()
    {
        return PlayerPrefs.HasKey("NombreJugador");
    }

    // ── Formato de tiempo ────────────────────────────────────────────────────

    public string ObtenerTiempoFormateado()
    {
        int minutos = Mathf.FloorToInt(tiempoJugado / 60f);
        int segundos = Mathf.FloorToInt(tiempoJugado % 60f);
        return string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    // ── Reiniciar (para "jugar de nuevo") ────────────────────────────────────

    public void ReiniciarEstadisticas()
    {
        totalPlasticosReciclados = 0;
        totalEnemigosDerrotados = 0;
        tiempoJugado = 0f;
        tiempoActivo = true;
    }
}
