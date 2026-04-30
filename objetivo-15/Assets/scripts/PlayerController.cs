using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 8f;
    public float fuerzaSalto = 16f;
    public float velocidadDash = 20f;
    public float duracionDash = 0.15f;
    public float cooldownDash = 1f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool estaEnSuelo;
    private bool estaDashing;
    private float temporizadorDash;
    private float temporizadorCooldown;
    private float direccionDash;
    private float temporizadorSalto;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        estaEnSuelo = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        estaEnSuelo = false;
    }

    void Update()
    {
        temporizadorSalto -= Time.deltaTime;
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

        if (Input.GetButtonDown("Jump") && estaEnSuelo)
        {
            Saltar();
        }
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
        rb.velocity = new Vector2(movimiento * velocidad, rb.velocity.y);

        if (movimiento != 0)
            transform.localScale = new Vector3(
                movimiento > 0 ? 1 : -1, 1, 1);

        if (anim != null)
        {
            anim.SetBool("corriendo", movimiento != 0);
            anim.SetBool("enSuelo", estaEnSuelo);
        }
    }

    void Saltar()
    {
        if (temporizadorSalto > 0) return;
        rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
        temporizadorSalto = 0.1f;
    }
}