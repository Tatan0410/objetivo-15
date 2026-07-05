using UnityEngine;

public class MunicionManager : MonoBehaviour
{
    public static MunicionManager instancia;

    [Header("Configuracion")]
    public int municionActual = 15;
    public int municionMaxima = 30;

    [Header("UI")]
    // TODO: reemplazar icono placeholder con sprite real
    public ContadorHUD contadorMunicion;

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

    public void AgregarMunicion(int cantidad)
    {
        municionActual = Mathf.Min(municionActual + cantidad, municionMaxima);
        ActualizarUI();
    }

    public bool ConsumirMunicion()
    {
        if (municionActual <= 0)
            return false;

        municionActual--;
        ActualizarUI();
        return true;
    }

    void ActualizarUI()
    {
        if (contadorMunicion != null)
            contadorMunicion.Actualizar(municionActual);
    }
}
