using UnityEngine;

public class CajaSorpresa : MonoBehaviour
{
    [Header("Power-ups posibles")]
    public GameObject[] prefabsPowerUp;

    [Header("Tipo de caja")]
    public bool cajaEnSuelo = false;

    [Header("Animaci�n")]
    public Animator animator;
    public float duracionAnimacion = 0.5f; // Ajusta seg�n la duraci�n real del clip

    private bool usada = false;
    private bool esperandoPowerUp = false;

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

        if (animator == null)
        {
            SoltarPowerUp();
            Destroy(gameObject);
            return;
        }

        esperandoPowerUp = true;
        animator.SetTrigger("golpeado");

        Collider2D colPropio = GetComponent<Collider2D>();
        if (colPropio != null) colPropio.enabled = false;
    }

    void Update()
    {
        if (!esperandoPowerUp) return;

        AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(0);
        if (st.IsName("cajarota") && st.normalizedTime >= 1f)
        {
            esperandoPowerUp = false;
            SoltarPowerUp();
            Destroy(gameObject);
        }
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