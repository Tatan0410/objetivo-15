using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Animator animator;

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

    [Header("Límites del nivel")]
    public bool usarLimiteIzquierdo = true;

    [Header("Disparo")]
    public GameObject prefabProyectil;
    public KeyCode teclaDisparo = KeyCode.F;
    public float cooldownDisparo = 0.5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private bool estaEnSuelo;
    private bool estaDashing;
    private float temporizadorDash;
    private float temporizadorCooldown;
    private float temporizadorCooldownDisparo;
    private float direccionDash;
    private float escalaOriginalX;
    private bool saltoPendiente = false;
    private bool controlActivo = true;

    private float velocidadNormal;
    private bool inmortal = false;
    private bool potenciadorActivo = false;

    [Header("Respawn")]
    public bool respawnPendiente = false;
    public Vector3 posicionRespawn;

    [Header("Caída lenta (Minecraft)")]
    public bool caidaLentaActiva = false;
    public float gravedadNormal = 1f;
    public float gravedadCaidaLenta = 0.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (animator == null)
            animator = GetComponent<Animator>();

        escalaOriginalX = Mathf.Abs(transform.localScale.x);
        velocidadNormal = velocidad;
        gravedadNormal = rb.gravityScale;

        if (!CompareTag("Player"))
            gameObject.tag = "Player";
    }

    void Update()
    {
        estaEnSuelo = Physics2D.OverlapCircle(
            puntoSuelo.position, radioSuelo, capaSuelo);

        if (!controlActivo) return;

        temporizadorCooldown -= Time.deltaTime;
        temporizadorCooldownDisparo -= Time.deltaTime;

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
        if (Input.GetKeyDown(KeyCode.W) && estaEnSuelo)
            saltoPendiente = true;

        if ((Input.GetKeyDown(teclaDisparo) || Input.GetMouseButtonDown(0)) &&
            temporizadorCooldownDisparo <= 0)
        {
            TipoArma arma = GetArmaEquipada();
            if (arma == TipoArma.Lanzador || arma == TipoArma.Red || arma == TipoArma.LanzaTubos)
            {
                if (MunicionManager.instancia != null &&
                    !MunicionManager.instancia.ConsumirMunicion())
                {
                    // Sin municion, no dispara
                }
                else
                {
                    Disparar(arma);
                    temporizadorCooldownDisparo = cooldownDisparo;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (respawnPendiente)
        {
            respawnPendiente = false;

            Vector3 destino = posicionRespawn;
            Debug.Log("Respawn en: " + destino);

            rb.velocity = Vector2.zero;
            rb.position = destino;
            transform.position = destino;

            rb.gravityScale = gravedadCaidaLenta;
            caidaLentaActiva = true;

            if (Camera.main != null)
            {
                CamaraSeguidora cam = Camera.main.GetComponent<CamaraSeguidora>();
                if (cam != null)
                {
                    Vector3 posCam = destino;
                    posCam.x += cam.offset.x;
                    posCam.y += cam.offset.y;
                    posCam.z = Camera.main.transform.position.z;
                    Camera.main.transform.position = posCam;
                }
            }

            return;
        }

        if (caidaLentaActiva)
        {
            if (estaEnSuelo)
            {
                caidaLentaActiva = false;
                rb.gravityScale = gravedadNormal;
                controlActivo = true;
            }
            else
            {
                if (animator != null)
                {
                    animator.SetBool("corriendo", false);
                    animator.SetBool("enSuelo", false);
                }
                return;
            }
        }

        if (!controlActivo)
        {
            rb.velocity = Vector2.zero;

            if (animator != null)
            {
                animator.SetBool("corriendo", false);
                animator.SetBool("enSuelo", estaEnSuelo);
            }
            return;
        }

        if (usarLimiteIzquierdo && Camera.main != null)
        {
            float limiteIzquierdo = Camera.main.transform.position.x -
                Camera.main.orthographicSize * Camera.main.aspect;
            if (transform.position.x < limiteIzquierdo)
                transform.position = new Vector3(
                    limiteIzquierdo,
                    transform.position.y,
                    transform.position.z);
        }

        if (estaDashing)
        {
            rb.velocity = new Vector2(
                direccionDash * velocidadDash, rb.velocity.y);
            temporizadorDash -= Time.fixedDeltaTime;
            if (temporizadorDash <= 0) estaDashing = false;

            if (animator != null)
                animator.SetBool("corriendo", true);
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

        if (animator != null)
        {
            animator.SetBool("corriendo", movimiento != 0);
            animator.SetBool("enSuelo", estaEnSuelo);
            animator.SetFloat("velocidadVertical", rb.velocity.y);
        }
    }

    public void DesactivarControl()
    {
        controlActivo = false;
        estaDashing = false;
    }

    public void ActivarControl()
    {
        controlActivo = true;
    }

    TipoArma GetArmaEquipada()
    {
        foreach (Transform child in transform)
        {
            ArmaPlaceholder arma = child.GetComponent<ArmaPlaceholder>();
            if (arma != null)
                return arma.tipo;
        }
        return TipoArma.Escudo;
    }

    void Disparar(TipoArma arma)
    {
        if (prefabProyectil == null)
        {
            Debug.LogWarning("prefabProyectil no asignado en PlayerController");
            return;
        }

        float dirX = transform.localScale.x > 0 ? 1f : -1f;
        Vector3 spawnPos = transform.position + new Vector3(dirX * 0.6f, 0, 0);
        GameObject proy = Instantiate(prefabProyectil, spawnPos, Quaternion.identity);

        TipoProyectil tipo = TipoProyectil.Lanzador;
        if (arma == TipoArma.Red) tipo = TipoProyectil.Red;
        else if (arma == TipoArma.LanzaTubos) tipo = TipoProyectil.LanzaTubos;

        proy.GetComponent<ProyectilArma>().Iniciar(new Vector2(dirX, 0), tipo);
    }

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
                VidasManager.instancia.AgregarVida();
                StartCoroutine(DestelloColor(Color.red, 0.3f));
                break;
        }
    }

    IEnumerator BoostVelocidad()
    {
        potenciadorActivo = true;
        velocidad = velocidadBoost;
        if (sr) sr.color = new Color(1f, 0.9f, 0.2f);
        yield return new WaitForSeconds(duracionVelocidad);
        velocidad = velocidadNormal;
        if (sr) sr.color = Color.white;
        potenciadorActivo = false;
    }

    IEnumerator BoostInmortalidad()
    {
        inmortal = true;
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