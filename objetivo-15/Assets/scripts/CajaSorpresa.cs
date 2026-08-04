using UnityEngine;

public class CajaSorpresa : MonoBehaviour
{
    [Header("Power-ups posibles")]
    public GameObject[] prefabsPowerUp;

    [Header("Tipo de caja")]
    public bool cajaEnSuelo = false;

    [Header("Animación")]
    public Animator animator;
    public float duracionAnimacion = 0.5f; // Ajusta según la duración real del clip

    private bool usada = false;

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = false;

        if (animator == null)
            animator = GetComponent<Animator>();
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

        if (animator != null)
            animator.SetTrigger("golpeado");

        Collider2D colPropio = GetComponent<Collider2D>();
        if (colPropio != null) colPropio.enabled = false;

        SoltarPowerUp();
        Destroy(gameObject, duracionAnimacion);
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