using UnityEngine;

public class ContadorMunicionSeleccionado : MonoBehaviour
{
    private ContadorHUD contador;
    private bool suscrito = false;

    void OnEnable()
    {
        Suscribirse();
    }

    void OnDisable()
    {
        Desuscribirse();
    }

    void Suscribirse()
    {
        if (suscrito || MunicionManager.instancia == null) return;
        MunicionManager.instancia.OnCambio += Refrescar;
        suscrito = true;
        Refrescar();
    }

    void Desuscribirse()
    {
        if (MunicionManager.instancia != null && suscrito)
        {
            MunicionManager.instancia.OnCambio -= Refrescar;
            suscrito = false;
        }
    }

    void Refrescar()
    {
        MunicionManager m = MunicionManager.instancia;
        if (m == null) return;
        if (contador == null) contador = GetComponent<ContadorHUD>();
        if (contador != null)
            contador.Actualizar(m.Obtener(m.tipoSeleccionado));
    }
}