using UnityEngine;

public class SistemaCrafteo : MonoBehaviour
{
    [Header("Prefabs de armas")]
    public GameObject prefabLanzador;
    public GameObject prefabRed;
    public GameObject prefabEscudo;
    public GameObject prefabLanzaTubos;

    public static SistemaCrafteo instancia;

    void Awake()
    {
        if (instancia == null)
            instancia = this;
        else
            Destroy(gameObject);
    }

    // Llama estas funciones desde la UI del crafteo
    public void CraftearLanzador()
    {
        if (Inventario.instancia.TieneIngredientes(3, 0, 0, 0))
        {
            Inventario.instancia.GastarIngredientes(3, 0, 0, 0);
            EquiparArma(prefabLanzador);
            Debug.Log("¡Lanzador crafteado!");
        }
        else Debug.Log("Necesitas 3 botellas PET");
    }

    public void CraftearRed()
    {
        if (Inventario.instancia.TieneIngredientes(0, 5, 0, 0))
        {
            Inventario.instancia.GastarIngredientes(0, 5, 0, 0);
            EquiparArma(prefabRed);
            Debug.Log("¡Red crafteada!");
        }
        else Debug.Log("Necesitas 5 bolsas");
    }

    public void CraftearEscudo()
    {
        if (Inventario.instancia.TieneIngredientes(1, 0, 2, 0))
        {
            Inventario.instancia.GastarIngredientes(1, 0, 2, 0);
            EquiparArma(prefabEscudo);
            Debug.Log("¡Escudo crafteado!");
        }
        else Debug.Log("Necesitas 2 tarros y 1 botella");
    }

    public void CraftearLanzaTubos()
    {
        if (Inventario.instancia.TieneIngredientes(0, 0, 0, 3))
        {
            Inventario.instancia.GastarIngredientes(0, 0, 0, 3);
            EquiparArma(prefabLanzaTubos);
            Debug.Log("¡LanzaTubos crafteado!");
        }
        else Debug.Log("Necesitas 3 tubos PVC");
    }

    void EquiparArma(GameObject prefabArma)
    {
        if (prefabArma == null) return;
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
            Instantiate(prefabArma, jugador.transform.position,
                Quaternion.identity, jugador.transform);
    }
}