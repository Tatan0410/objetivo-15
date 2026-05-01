using UnityEngine;
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 8f;
    public float fuerzaSalto = 16f;
    public float velocidadDash = 20f;
    public float duracionDash = 0.15f;
    public float cooldownDash = 1f;

    [Header("Detección de suelo")]
    public Transform puntoSuelo;
    public float radioSuelo = 0.2f;
    public LayerMask capaSuelo;

    private Rigidbody2D rb;
    private Animator anim;
    private bool estaEnSuelo;
    private bool estaDashing;
    private float temporizadorDash;
    private float temporizadorCooldown;
    private float direccionDash;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        estaEnSuelo = Physics2D.OverlapCircle(puntoSuelo.position, radioSuelo, capaSuelo);

        temporizadorCooldown -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && temporizadorCooldown <= 0 && !estaDashing)
        {
            estaDashing = true;
            temporizadorDash = duracionDash;
            temporizadorCooldown = cooldownDash;
            direccionDash = Input.GetAxisRaw("Horizontal");
            if (direccionDash == 0) direccionDash = 1;
        }

        if (Input.GetButtonDown("Jump") && estaEnSuelo)
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
    }

    void FixedUpdate()
    {
        if (estaDashing)
        {
            rb.velocity = new Vector2(direccionDash * velocidadDash, rb.velocity.y);
            temporizadorDash -= Time.fixedDeltaTime;
            if (temporizadorDash <= 0) estaDashing = false;
            return;
        }

        float movimiento = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(movimiento * velocidad, rb.velocity.y);

        if (movimiento != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = movimiento > 0 ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
            transform.localScale = escala;
        }

        if (anim != null)
        {
            anim.SetBool("corriendo", movimiento != 0);
            anim.SetBool("enSuelo", estaEnSuelo);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (puntoSuelo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(puntoSuelo.position, radioSuelo);
        }
    }
}