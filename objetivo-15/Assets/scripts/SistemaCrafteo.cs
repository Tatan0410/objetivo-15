using UnityEngine;

public class SistemaCrafteo : MonoBehaviour
{
    [Header("Prefabs de armas")]
    public GameObject prefabLanzador;

    [Header("Posicion en la mano")]
    public Vector2 offsetMano = new Vector2(0.5f, 0.4f);

    [Header("Cartuchos de balas")]
    public int balasPorCartucho = 10;

    public static SistemaCrafteo instancia;

    void Awake()
    {
        if (instancia == null)
            instancia = this;
        else
            Destroy(gameObject);
    }

    public void CraftearLanzador()
    {
        if (Inventario.instancia.TieneIngredientes(3, 2, 1))
        {
            Inventario.instancia.GastarIngredientes(3, 2, 1);
            EquiparArma(prefabLanzador);
            Debug.Log("Lanzador crafteado!");
        }
        else Debug.Log("Necesitas 3 PET + 2 Bolsas + 1 Icopor");
    }

    public void CraftearCartuchoComun()
    {
        if (Inventario.instancia.TieneIngredientes(1, 1, 0))
        {
            Inventario.instancia.GastarIngredientes(1, 1, 0);
            MunicionManager.instancia.AgregarBala(TipoBala.Comun, balasPorCartucho);
            ReproducirAnimacionCrafteo();
            Debug.Log("Cartucho comun (10 balas) crafteado!");
        }
        else Debug.Log("Necesitas 1 Botella + 1 Bolsa");
    }

    public void CraftearCartuchoEpica()
    {
        if (Inventario.instancia.TieneIngredientes(1, 2, 1))
        {
            Inventario.instancia.GastarIngredientes(1, 2, 1);
            MunicionManager.instancia.AgregarBala(TipoBala.Epica, balasPorCartucho);
            ReproducirAnimacionCrafteo();
            Debug.Log("Cartucho epico (10 balas) crafteado!");
        }
        else Debug.Log("Necesitas 1 Botella + 2 Bolsas + 1 Icopor");
    }

    public void CraftearCartuchoLegendaria()
    {
        if (Inventario.instancia.TieneIngredientes(3, 4, 3))
        {
            Inventario.instancia.GastarIngredientes(3, 4, 3);
            MunicionManager.instancia.AgregarBala(TipoBala.Legendaria, balasPorCartucho);
            ReproducirAnimacionCrafteo();
            Debug.Log("Cartucho legendario (10 balas) crafteado!");
        }
        else Debug.Log("Necesitas 3 Botellas + 4 Bolsas + 3 Icopores");
    }

    void EquiparArma(GameObject prefabArma)
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null) return;

        PlayerController pc = jugador.GetComponent<PlayerController>();
        if (pc != null)
            pc.armaEquipada = TipoArma.Lanzador;

        ReproducirAnimacionCrafteo();
    }

    void ReproducirAnimacionCrafteo()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null) return;

        Animator anim = jugador.GetComponentInChildren<Animator>();
        if (anim != null)
            anim.SetTrigger("Craftear");
    }
}
