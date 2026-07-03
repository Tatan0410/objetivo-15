using UnityEngine;

public class CajaSorpresa : MonoBehaviour
{
    [Header("Power-ups posibles")]
    public GameObject[] prefabsPowerUp;

    [Header("Tipo de caja")]
    public bool cajaEnSuelo = false;

    private bool usada = false;

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (usada) return;
        if (!col.collider.CompareTag("Player")) return;

        Activar();
    }

    void Activar()
    {
        usada = true;
        SoltarPowerUp();
        Destroy(gameObject);
    }

    void SoltarPowerUp()
    {
        if (prefabsPowerUp.Length == 0)
        {
            Debug.LogWarning("CajaSorpresa: no hay power-ups asignados");
            return;
        }

        int idx = Random.Range(0, prefabsPowerUp.Length);

        Vector3 posSpawn = transform.position + Vector3.up * 1.2f;
        Instantiate(prefabsPowerUp[idx], posSpawn, Quaternion.identity);
    }
}
