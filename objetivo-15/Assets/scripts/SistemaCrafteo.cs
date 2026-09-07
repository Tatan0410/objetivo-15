using UnityEngine;

public class SistemaCrafteo : MonoBehaviour
{
    [Header("Prefabs de armas")]
    public GameObject prefabLanzador;

    [Header("Posicion en la mano")]
    public Vector2 offsetMano = new Vector2(0.5f, 0.4f);

    [Header("Cartuchos de balas")]
    public int balasPorCartucho = 10;

    [System.Serializable]
    public class CostoPotenciador
    {
        public int manzanas = 2;
        public int bananas = 2;
    }

    [Header("Potenciadores (solo manzanas + bananas)")]
    public int maxCrafteosPotenciador = 3;
    public CostoPotenciador costoVelocidad = new CostoPotenciador { manzanas = 2, bananas = 2 };
    public CostoPotenciador costoInmortalidad = new CostoPotenciador { manzanas = 3, bananas = 3 };
    public CostoPotenciador costoVidaExtra = new CostoPotenciador { manzanas = 4, bananas = 4 };

    public static SistemaCrafteo instancia;

    private readonly int[] crafteosPotenciador = new int[3];

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

    public void CraftearPotenciadorVelocidad()
    {
        CraftearPotenciador(0, TipoPotenciador.Velocidad, costoVelocidad);
    }

    public void CraftearPotenciadorInmortalidad()
    {
        CraftearPotenciador(1, TipoPotenciador.Inmortalidad, costoInmortalidad);
    }

    public void CraftearPotenciadorVidas()
    {
        CraftearPotenciador(2, TipoPotenciador.VidaExtra, costoVidaExtra);
    }

    public int CrafteosRestantes(int indice)
    {
        if (indice < 0 || indice >= crafteosPotenciador.Length) return 0;
        return Mathf.Max(0, maxCrafteosPotenciador - crafteosPotenciador[indice]);
    }

    public bool PuedeCraftearPotenciador(int indice, CostoPotenciador costo)
    {
        if (crafteosPotenciador[indice] >= maxCrafteosPotenciador) return false;
        if (Inventario.instancia == null) return false;
        return Inventario.instancia.TieneFrutas(costo.manzanas, costo.bananas);
    }

    void CraftearPotenciador(int indice, TipoPotenciador tipo, CostoPotenciador costo)
    {
        if (Inventario.instancia == null) return;

        if (crafteosPotenciador[indice] >= maxCrafteosPotenciador)
        {
            Debug.Log("Limite alcanzado: ya crafteaste este potenciador " + maxCrafteosPotenciador + " veces.");
            return;
        }

        if (!Inventario.instancia.TieneFrutas(costo.manzanas, costo.bananas))
        {
            Debug.Log("Necesitas " + costo.manzanas + " manzanas + " + costo.bananas + " bananas.");
            return;
        }

        Inventario.instancia.GastarFrutas(costo.manzanas, costo.bananas);
        crafteosPotenciador[indice]++;

        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        PlayerController pc = jugador != null ? jugador.GetComponent<PlayerController>() : null;
        if (pc != null)
        {
            pc.AplicarPotenciador(tipo);
            Debug.Log("Potenciador " + tipo + " aplicado! Restantes: " + CrafteosRestantes(indice));
        }

        ReproducirAnimacionCrafteo();

        NotificarActualizacion();
    }

    void NotificarActualizacion()
    {
        var canecas = FindFirstObjectByType<CanecasCrafteo>();
        if (canecas != null)
            canecas.ActualizarTodo();
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
