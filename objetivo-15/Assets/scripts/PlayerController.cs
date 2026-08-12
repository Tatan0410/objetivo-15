using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Animator animator;

    [Header("Movimiento")]
    public float velocidad = 6f;
    public float fuerzaSalto = 7.5f;

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
    public float distanciaMuzzle = 0.6f;
    public float alturaMuzzle = -0.1f;
    public TipoArma armaEquipada = TipoArma.Ninguna;

    [Header("Sprites de bala por tipo")]
    public Sprite spriteBalaComun;
    public Sprite spriteBalaRara;
    public Sprite spriteBalaEpica;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private bool estaEnSuelo;
    private float temporizadorCooldownDisparo;
    private float escalaOriginalX;
    private bool saltoPendiente = false;
    private bool controlActivo = true;

    private float velocidadNormal;
    private bool inmortal = false;
    private bool potenciadorActivo = false;
    private float coyoteTime = 0.12f;
    private float coyoteTimeContador = 0f;

    [Header("Partículas")]
    public ParticleSystem particulas;
    private float escalaXAnterior;

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
        escalaXAnterior = transform.localScale.x;
        velocidadNormal = velocidad;
        gravedadNormal = rb.gravityScale;

        if (!CompareTag("Player"))
            gameObject.tag = "Player";
    }

    void Update()
    {
        estaEnSuelo = Physics2D.OverlapCircle(puntoSuelo.position, radioSuelo, capaSuelo)
               || Physics2D.Raycast(puntoSuelo.position + Vector3.left * 0.15f, Vector2.down, radioSuelo + 0.05f, capaSuelo)
               || Physics2D.Raycast(puntoSuelo.position + Vector3.right * 0.15f, Vector2.down, radioSuelo + 0.05f, capaSuelo);

        if (!controlActivo) return;

        temporizadorCooldownDisparo -= Time.deltaTime;

        if (estaEnSuelo)
            coyoteTimeContador = coyoteTime;
        else
            coyoteTimeContador -= Time.deltaTime;

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && coyoteTimeContador > 0f)
        {
            saltoPendiente = true;
            coyoteTimeContador = 0f;
        }

        if (Input.GetKeyDown(teclaDisparo) &&
            temporizadorCooldownDisparo <= 0)
        {
            TipoArma arma = GetArmaEquipada();
            if (arma == TipoArma.Ninguna) return;

            TipoBala? tipo = MunicionManager.instancia != null
                ? MunicionManager.instancia.ConsumirBalaSeleccionada()
                : null;

            if (tipo != null)
            {
                Disparar(arma, tipo.Value);
                temporizadorCooldownDisparo = cooldownDisparo;

                // Animación de disparo: SOLO al disparar
                if (animator != null)
                    animator.SetTrigger("Disparar");
            }
        }
    }

    void FixedUpdate()
    {
        if (respawnPendiente)
        {
            respawnPendiente = false;

            Vector3 destino = posicionRespawn;
            Debug.Log("[PlayerController] Respawn destino=" + destino);

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

        float movimiento = Input.GetAxisRaw("Horizontal");

        if (saltoPendiente)
        {
            rb.velocity = new Vector2(movimiento * velocidad, fuerzaSalto);
            saltoPendiente = false;
            if (particulas != null)
                particulas.Play();
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

        if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(escalaXAnterior))
        {
            escalaXAnterior = transform.localScale.x;
            if (particulas != null)
                particulas.Play();
        }

        if (animator != null)
        {
            animator.SetBool("corriendo", movimiento != 0);
            animator.SetBool("enSuelo", estaEnSuelo);
            float velocidadVerticalAnim = estaEnSuelo ? 0f : rb.velocity.y;
            animator.SetFloat("velocidadVertical", velocidadVerticalAnim);
        }
    }

    public void DesactivarControl()
    {
        controlActivo = false;
    }

    public void ActivarControl()
    {
        controlActivo = true;
    }

    TipoArma GetArmaEquipada()
    {
        return armaEquipada;
    }

    void Disparar(TipoArma arma, TipoBala tipoBala)
    {
        if (prefabProyectil == null)
        {
            Debug.LogWarning("prefabProyectil no asignado en PlayerController");
            return;
        }

        int danio = tipoBala == TipoBala.Comun ? 1
                  : tipoBala == TipoBala.Rara  ? 5
                  : 10;

        float dirX = transform.localScale.x > 0 ? 1f : -1f;
        Vector3 spawnPos = transform.position + new Vector3(dirX * distanciaMuzzle, alturaMuzzle, 0f);
        GameObject proy = Instantiate(prefabProyectil, spawnPos, Quaternion.identity);
        Debug.Log($"[PlayerController] Disparo desde {spawnPos} direccion={dirX} tipo={tipoBala} danio={danio}");

        proy.GetComponent<ProyectilArma>().Iniciar(
            new Vector2(dirX, 0),
            tipoBala,
            danio,
            SpriteDeBala(tipoBala));
    }

    Sprite SpriteDeBala(TipoBala tipo)
    {
        switch (tipo)
        {
            case TipoBala.Rara:  return spriteBalaRara;
            case TipoBala.Epica: return spriteBalaEpica;
            default:             return spriteBalaComun;
        }
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