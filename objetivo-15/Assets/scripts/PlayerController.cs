using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 8f;
    public float fuerzaSalto = 12f;
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
    private float escalaOriginalX;
    private bool saltoPendiente = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        escalaOriginalX = Mathf.Abs(transform.localScale.x);
    }

    void Update()
    {
        estaEnSuelo = Physics2D.OverlapCircle(
            puntoSuelo.position, radioSuelo, capaSuelo);

        temporizadorCooldown -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) &&
            temporizadorCooldown <= 0 && !estaDashing)
        {
            estaDashing = true;
            temporizadorDash = duracionDash;
            temporizadorCooldown = cooldownDash;
            direccionDash = Input.GetAxisRaw("Horizontal");
            if (direccionDash == 0) direccionDash = 1;
        }

        // Guardar el salto para aplicarlo en FixedUpdate
        if (Input.GetKeyDown(KeyCode.Space) && estaEnSuelo)
            saltoPendiente = true;
    }

    void FixedUpdate()
    {
        if (estaDashing)
        {
            rb.velocity = new Vector2(
                direccionDash * velocidadDash, rb.velocity.y);
            temporizadorDash -= Time.fixedDeltaTime;
            if (temporizadorDash <= 0)
                estaDashing = false;
            return;
        }

        float movimiento = Input.GetAxisRaw("Horizontal");

        // Aplicar salto pendiente antes de sobreescribir velocidad
        if (saltoPendiente)
        {
            rb.velocity = new Vector2(movimiento * velocidad, fuerzaSalto);
            saltoPendiente = false;
        }
        else
        {
            rb.velocity = new Vector2(movimiento * velocidad, rb.velocity.y);
        }

        if (movimiento > 0)
            transform.localScale = new Vector3(
                escalaOriginalX, transform.localScale.y, 1);
        else if (movimiento < 0)
            transform.localScale = new Vector3(
                -escalaOriginalX, transform.localScale.y, 1);

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