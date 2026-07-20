using UnityEngine;

public class SistemaCrafteo : MonoBehaviour
{
    [Header("Prefabs de armas")]
    public GameObject prefabLanzador;

    [Header("Posicion en la mano")]
    public Vector2 offsetMano = new Vector2(0.5f, 0.4f);

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

    void EquiparArma(GameObject prefabArma)
    {
        if (prefabArma == null)
        {
            Debug.LogWarning("El prefab del arma no esta asignado en SistemaCrafteo");
            return;
        }
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null) return;

        foreach (Transform child in jugador.transform)
        {
            if (child.GetComponent<ArmaPlaceholder>() != null)
                Destroy(child.gameObject);
        }

        GameObject arma = Instantiate(prefabArma, jugador.transform);
        arma.transform.localPosition = offsetMano;
    }
}
