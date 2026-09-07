using UnityEngine;
using UnityEngine.UI;

public class CanecasCrafteo : MonoBehaviour
{
    [Header("Canecas (solo contadores, sin boton)")]
    public ContadorHUD contadorCanecaReciclaje;
    public ContadorHUD contadorCanecaIcopor;
    public ContadorHUD contadorCanecaFruta;

    [Header("Potenciadores - contadores de restantes")]
    public ContadorHUD[] contadorRestantePotenciador = new ContadorHUD[3];

    [Header("Potenciadores - botones (se deshabilitan al llegar a 0)")]
    public Button[] botonPotenciador = new Button[3];

    void Start()
    {
        ActualizarTodo();
    }

    public void ActualizarTodo()
    {
        ActualizarCanecas();
        ActualizarPotenciadores();
    }

    void ActualizarCanecas()
    {
        if (Inventario.instancia == null) return;

        Inventario inv = Inventario.instancia;
        if (contadorCanecaReciclaje != null)
            contadorCanecaReciclaje.Actualizar(inv.botellasPET + inv.bolsasPlasticas);
        if (contadorCanecaIcopor != null)
            contadorCanecaIcopor.Actualizar(inv.icopor);
        if (contadorCanecaFruta != null)
            contadorCanecaFruta.Actualizar(inv.manzanas + inv.bananas);
    }

    void ActualizarPotenciadores()
    {
        if (SistemaCrafteo.instancia == null) return;

        SistemaCrafteo sist = SistemaCrafteo.instancia;
        for (int i = 0; i < contadorRestantePotenciador.Length; i++)
        {
            int restantes = sist.CrafteosRestantes(i);
            if (contadorRestantePotenciador[i] != null)
                contadorRestantePotenciador[i].Actualizar(restantes);
            if (i < botonPotenciador.Length && botonPotenciador[i] != null)
                botonPotenciador[i].interactable = restantes > 0;
        }
    }
}