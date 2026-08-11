using UnityEngine;

public class MunicionManager : MonoBehaviour
{
    public static MunicionManager instancia;

    [Header("Configuracion")]
    public int maximoPorTipo = 99;

    [Header("Conteo por tipo de bala")]
    public int balasComunes = 15;
    public int balasRaras = 0;
    public int balasEpicas = 0;

    [Header("Tipo seleccionado")]
    public TipoBala tipoSeleccionado = TipoBala.Comun;

    [Header("UI")]
    public ContadorHUD contadorMunicion;

    public event System.Action OnCambio;

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
    }

    void Start()
    {
        ActualizarUI();
    }

    public void AsignarContador(ContadorHUD contador)
    {
        contadorMunicion = contador;
        ActualizarUI();
    }

    public int Obtener(TipoBala tipo)
    {
        switch (tipo)
        {
            case TipoBala.Comun: return balasComunes;
            case TipoBala.Rara: return balasRaras;
            case TipoBala.Epica: return balasEpicas;
            default: return 0;
        }
    }

    void Setear(TipoBala tipo, int valor)
    {
        valor = Mathf.Clamp(valor, 0, maximoPorTipo);
        switch (tipo)
        {
            case TipoBala.Comun: balasComunes = valor; break;
            case TipoBala.Rara: balasRaras = valor; break;
            case TipoBala.Epica: balasEpicas = valor; break;
        }
    }

    public void AgregarBala(TipoBala tipo, int cantidad)
    {
        Setear(tipo, Obtener(tipo) + cantidad);
        ActualizarUI();
        OnCambio?.Invoke();
    }

    public TipoBala? ConsumirBalaSeleccionada()
    {
        if (Obtener(tipoSeleccionado) <= 0)
            return null;

        Setear(tipoSeleccionado, Obtener(tipoSeleccionado) - 1);
        ActualizarUI();
        OnCambio?.Invoke();
        return tipoSeleccionado;
    }

    public void CambiarTipo()
    {
        int idx = ((int)tipoSeleccionado + 1) % 3;
        tipoSeleccionado = (TipoBala)idx;
        ActualizarUI();
        OnCambio?.Invoke();
    }

    public void EstablecerTipo(TipoBala tipo)
    {
        tipoSeleccionado = tipo;
        ActualizarUI();
        OnCambio?.Invoke();
    }

    public void ActualizarUI()
    {
        if (contadorMunicion != null)
            contadorMunicion.Actualizar(Obtener(tipoSeleccionado));
    }
}
