

using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 6f;
    public float fuerzaSalto = 7.5f;
    public float velocidadDash = 15f;
    public float duracionDash = 0.15f;
    public float cooldownDash = 1f;

    [Header("Detección de suelo")]
    public Transform puntoSuelo;
    public float radioSuelo = 0.2f;
    public LayerMask capaSuelo;

    [Header("Potenciadores")]
    public float velocidadBoost = 12f;
    public float duracionVelocidad = 8f;
    public float duracionInmortalidad = 10f;

    // Referencias
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    // Estado movimiento
    private bool estaEnSuelo;
    private bool estaDashing;
    private float temporizadorDash;
    private float temporizadorCooldown;
    private float direccionDash;
    private float escalaOriginalX;
    private bool saltoPendiente = false;

    // Estado potenciadores
    private float velocidadNormal;
    private bool inmortal = false;
    private bool potenciadorActivo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        escalaOriginalX = Mathf.Abs(transform.localScale.x);
        velocidadNormal = velocidad;
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

    // ═══════════════════════════════════════
    // SISTEMA DE POTENCIADORES
    // ═══════════════════════════════════════

    public void AplicarPotenciador(TipoPotenciador tipo)
    {
        switch (tipo)
        {
            case TipoPotenciador.Velocidad:
                if (potenciadorActivo) StopCoroutine("BoostVelocidad");
                StartCoroutine(BoostVelocidad());
                break;
            case TipoPotenciador.Inmortalidad:
                if (inmortal) StopCoroutine("BoostInmortalidad");
                StartCoroutine(BoostInmortalidad());
                break;
            case TipoPotenciador.VidaExtra:
                VidasManager.instancia.AgregarVida();  // ✅ corregido
                StartCoroutine(DestelloColor(Color.red, 0.3f));
                break;
        }
    }

    IEnumerator BoostVelocidad()
    {
        potenciadorActivo = true;
        velocidad = velocidadBoost;
        if (sr) sr.color = new Color(1f, 0.9f, 0.2f);
        Debug.Log("⚡ Velocidad activada por " + duracionVelocidad + "s");

        yield return new WaitForSeconds(duracionVelocidad);

        velocidad = velocidadNormal;
        if (sr) sr.color = Color.white;
        potenciadorActivo = false;
        Debug.Log("⚡ Velocidad restaurada");
    }

    IEnumerator BoostInmortalidad()
    {
        inmortal = true;
        Debug.Log("⭐ Inmortalidad activada por " + duracionInmortalidad + "s");

        float tiempo = 0f;
        while (tiempo < duracionInmortalidad)
        {
            if (sr) sr.color = new Color(1f, 1f, 0.3f, 0.4f);
            yield return new WaitForSeconds(0.2f);
            if (sr) sr.color = Color.white;
            yield return new WaitForSeconds(0.2f);
            tiempo += 0.4f;
        }

        inmortal = false;
        if (sr) sr.color = Color.white;
        Debug.Log("⭐ Inmortalidad terminada");
    }

    IEnumerator DestelloColor(Color color, float duracion)
    {
        if (sr) sr.color = color;
        yield return new WaitForSeconds(duracion);
        if (sr) sr.color = Color.white;
    }

    public bool EsInmortal() => inmortal;

    void OnDrawGizmosSelected()
    {
        if (puntoSuelo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(puntoSuelo.position, radioSuelo);
        }
    }
}