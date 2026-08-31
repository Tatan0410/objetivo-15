using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5f;
    public float speed = 2f;

    [Header("Detección de bordes")]
    public LayerMask capaSuelo;
    public float distanciaBorde = 0.4f;
    public float alturaBorde = 0.3f;

    [Header("Obstáculos y patrulla")]
    public LayerMask capaObstaculos;
    public float distanciaObstaculo = 0.6f;
    public float distanciaPatrulla = 3f;

    [Header("Drop de plasticos")]
    public GameObject[] prefabsPlasticos;
    public int cantidadDrop = 1;
    [Range(0f, 1f)]
    public float probabilidadDrop = 0.5f;

    [Header("Vida")]
    public int vidas = 3;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool muerto = false;
    private bool congelado = false;
    private SpriteRenderer sr;
    private Color colorOriginal;
    private Color colorFlashBase;
    private bool flashDanioActivo;
    private bool yaProcesadoEsteFrame = false;
    private float direccionVisual = 1f;
    private Animator animator;
    private Vector2 puntoInicio;
    private bool moviendoDerecha = true;
    private float pausaPatrulla;
    private bool enPausa = false;

    private enum Modo { Quieto, Perseguir, Patrullar }
    private Modo modo = Modo.Quieto;

    void Start()
    {
        vidas = ConfigNivelEnemigos.VidasEnemigo();
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) colorOriginal = sr.color;
        animator = GetComponentInChildren<Animator>();
        puntoInicio = transform.position;
    }

    void Update()
    {
        if (player == null || muerto || congelado) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < detectionRadius)
        {
            bool jugadorArriba = (player.position.y - transform.position.y) > 0.5f;

            if (!jugadorArriba && TrayectoriaDespejada())
            {
                modo = Modo.Perseguir;
                Vector2 dir = (player.position - transform.position).normalized;
                movement = new Vector2(dir.x, 0f);
            }
            else
            {
                if (modo == Modo.Quieto)
                    IniciarPatrulla();
                modo = Modo.Patrullar;
            }
        }
        else
        {
            modo = Modo.Quieto;
            movement = Vector2.zero;
        }

        if (animator != null)
            animator.SetBool("atacando", movement.x != 0f || modo == Modo.Patrullar);
    }

    void FixedUpdate()
    {
        yaProcesadoEsteFrame = false;
        if (rb == null || muerto || congelado) return;

        switch (modo)
        {
            case Modo.Perseguir:
                if (movement.x != 0f)
                {
                    direccionVisual = movement.x > 0f ? 1f : -1f;
                    if (sr != null) sr.flipX = direccionVisual > 0f;

                    if (HayObstaculoAdelante(direccionVisual) || HayTrampaAdelante(direccionVisual))
                    {
                        IniciarPatrulla();
                        modo = Modo.Patrullar;
                        break;
                    }
                    if (!HaySueloAdelante(direccionVisual))
                        movement = Vector2.zero;
                }
                break;

            case Modo.Patrullar:
                Patrullar();
                break;

            case Modo.Quieto:
                movement = Vector2.zero;
                break;
        }

        if (movement.x != 0f && OtroEnemigoDelante(movement.x))
            movement = Vector2.zero;

        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    void Patrullar()
    {
        if (enPausa)
        {
            pausaPatrulla -= Time.fixedDeltaTime;
            if (pausaPatrulla <= 0f)
            {
                enPausa = false;
                Voltear();
            }
            else
            {
                movement = Vector2.zero;
                return;
            }
        }

        float dir = moviendoDerecha ? 1f : -1f;

        if (HayObstaculoAdelante(dir) || HayTrampaAdelante(dir) || !HaySueloAdelante(dir))
        {
            enPausa = true;
            pausaPatrulla = 0.3f;
            movement = Vector2.zero;
            return;
        }

        float limite = moviendoDerecha
            ? puntoInicio.x + distanciaPatrulla
            : puntoInicio.x - distanciaPatrulla;

        if ((moviendoDerecha && transform.position.x >= limite) ||
            (!moviendoDerecha && transform.position.x <= limite))
        {
            enPausa = true;
            pausaPatrulla = 0.3f;
            movement = Vector2.zero;
            return;
        }

        direccionVisual = dir;
        if (sr != null) sr.flipX = direccionVisual > 0f;
        movement = new Vector2(dir, 0f);
    }

    void IniciarPatrulla()
    {
        modo = Modo.Patrullar;
        puntoInicio = transform.position;
        enPausa = false;
    }

    void Voltear()
    {
        moviendoDerecha = !moviendoDerecha;
    }

    bool TrayectoriaDespejada()
    {
        if (player == null) return true;
        LayerMask mask = capaObstaculos != 0 ? capaObstaculos : capaSuelo;
        Vector2 origen = transform.position;
        Vector2 destino = player.position;

        if (mask != 0)
        {
            RaycastHit2D hit = Physics2D.Linecast(origen, destino, mask);
            if (hit.collider != null) return false;
        }

        RaycastHit2D[] hits = Physics2D.LinecastAll(origen, destino);
        foreach (RaycastHit2D h in hits)
        {
            if (h.collider == null || h.collider.gameObject == gameObject) continue;
            if (EsTrampa(h.collider.gameObject)) return false;
        }
        return true;
    }

    bool HaySueloAdelante(float dir)
    {
        if (capaSuelo == 0) return true;

        Vector3 origen = transform.position + new Vector3(dir * distanciaBorde, -alturaBorde, 0f);
        RaycastHit2D hit = Physics2D.Raycast(origen, Vector2.down, alturaBorde + 0.1f, capaSuelo);
        return hit.collider != null;
    }

    bool HayObstaculoAdelante(float dir)
    {
        LayerMask mask = capaObstaculos != 0 ? capaObstaculos : capaSuelo;
        if (mask == 0) return false;

        Vector2 origen = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origen, Vector2.right * dir, distanciaObstaculo, mask);
        return hit.collider != null;
    }

    static bool EsEnemigo(GameObject go)
    {
        if (go == null) return false;
        return go.GetComponent<Enemigo>() != null
            || go.GetComponent<EnemigoVolador>() != null
            || go.GetComponent<rata>() != null;
    }

    static bool EsTrampa(GameObject go)
    {
        if (go == null) return false;
        return go.GetComponent<TrampaHot>() != null
            || go.GetComponent<DañoEnemigo>() != null;
    }

    bool HayTrampaAdelante(float dir)
    {
        Collider2D cuerpo = ObtenerColliderCuerpo();
        Vector2 origen = cuerpo != null ? cuerpo.bounds.center : (Vector2)transform.position;
        Vector2 size = cuerpo != null ? cuerpo.bounds.size : new Vector2(0.8f, 0.8f);
        float distancia = (cuerpo != null ? cuerpo.bounds.extents.x : 0.4f) + 0.4f;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origen, size, 0f, Vector2.right * dir, distancia);
        foreach (RaycastHit2D h in hits)
        {
            if (h.collider == null || h.collider.gameObject == gameObject) continue;
            if (EsTrampa(h.collider.gameObject)) return true;
        }
        return false;
    }

    bool OtroEnemigoDelante(float dir)
    {
        Collider2D cuerpo = ObtenerColliderCuerpo();
        Vector2 origen = cuerpo != null ? cuerpo.bounds.center : (Vector2)transform.position;
        Vector2 size = cuerpo != null ? cuerpo.bounds.size : new Vector2(0.8f, 0.8f);
        float distancia = (cuerpo != null ? cuerpo.bounds.extents.x : 0.4f) + 0.05f;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origen, size, 0f, Vector2.right * dir, distancia);
        foreach (RaycastHit2D h in hits)
        {
            if (h.collider == null || h.collider.gameObject == gameObject) continue;
            if (h.collider.isTrigger) continue;
            if (EsEnemigo(h.collider.gameObject)) return true;
        }
        return false;
    }

    void SoltarPlasticos()
    {
        string escena = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"[Enemigo] SoltarPlasticos en '{escena}' | drop={probabilidadDrop} cantidad={cantidadDrop} prefabs={(prefabsPlasticos!=null?prefabsPlasticos.Length:0)} pos={transform.position}");

        if (Random.value > probabilidadDrop)
        {
            Debug.Log($"[Enemigo] Drop falló por probabilidad ({probabilidadDrop}) en '{escena}'");
            return;
        }

        GameObject[] fuente = prefabsPlasticos;
        if (fuente == null || fuente.Length == 0)
        {
            Debug.LogWarning($"[Enemigo] prefabsPlasticos vacío en '{escena}' ({gameObject.name}) - usando fallback Resources");
            fuente = new GameObject[]
            {
                Resources.Load<GameObject>("botella"),
                Resources.Load<GameObject>("bolsaplastica"),
                Resources.Load<GameObject>("icopor")
            };
        }

        int validos = 0;
        for (int i = 0; i < cantidadDrop; i++)
        {
            GameObject p = fuente[Random.Range(0, fuente.Length)];
            if (p != null) validos++;
        }
        if (validos == 0)
        {
            Debug.LogWarning($"[Enemigo] Ningún prefab válido para drop en '{escena}' ({gameObject.name})");
            return;
        }

        GameObject[] selecciones = new GameObject[validos];
        int idx = 0;
        while (idx < validos)
        {
            GameObject p = fuente[Random.Range(0, fuente.Length)];
            if (p != null) selecciones[idx++] = p;
        }

        Vector3 posSpawn = transform.position + Vector3.up * 0.8f;
        for (int k = 0; k < 4; k++)
        {
            if (Physics2D.OverlapCircle(posSpawn, 0.2f) == null) break;
            posSpawn += Vector3.up * 0.4f;
        }
        Debug.Log($"[Enemigo] Spawneando {validos} plasticos en '{escena}' pos={posSpawn} overlap={Physics2D.OverlapCircle(posSpawn,0.2f) != null}");

        GameObject runner = new GameObject("PlasticoSpawnRunnerEnemigo");
        runner.AddComponent<PlasticoSpawnRunner>().Iniciar(
            selecciones,
            validos,
            posSpawn,
            0.5f
        );
    }

    void Morir()
    {
        if (muerto) return;
        muerto = true;
        if (EstadisticasManager.instancia != null)
            EstadisticasManager.instancia.RegistrarEnemigoDerrotado();

        if (animator != null)
            animator.SetBool("muerto", true);

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;

        if (sr != null) sr.color = Color.gray;
        SoltarPlasticos();
        Destroy(gameObject, 1f);
    }

    public void RecibirDanio(int cantidad)
    {
        if (muerto) return;
        vidas -= cantidad;
        if (vidas <= 0)
        {
            Morir();
            return;
        }
        IniciarFlashDanio();
    }

    void IniciarFlashDanio()
    {
        if (sr == null) return;
        if (!flashDanioActivo)
            colorFlashBase = sr.color;
        flashDanioActivo = true;
        sr.color = new Color(1f, 0f, 0f, colorFlashBase.a);
        CancelInvoke("DetenerFlashDanio");
        Invoke("DetenerFlashDanio", 0.2f);
    }

    void DetenerFlashDanio()
    {
        if (sr != null && !muerto) sr.color = colorFlashBase;
        flashDanioActivo = false;
    }

    public void Congelar()
    {
        if (muerto) return;
        congelado = true;
        if (rb != null) rb.velocity = Vector2.zero;
        if (sr != null)
        {
            colorOriginal = sr.color;
            sr.color = new Color(0.5f, 0.8f, 1f);
        }
        Invoke("Restaurar", 5f);
    }

    void Restaurar()
    {
        congelado = false;
        if (sr != null) sr.color = colorOriginal;
    }

    private Collider2D ObtenerColliderCuerpo()
    {
        foreach (var c in GetComponents<Collider2D>())
            if (!c.isTrigger) return c;
        return null;
    }

    private void ManejarContactoJugador(Collider2D colJugador, GameObject jugador)
    {
        if (muerto || congelado || yaProcesadoEsteFrame) return;

        PlayerController pc = jugador.GetComponent<PlayerController>();
        if (pc != null && pc.EsInmortal())
        {
            Morir();
            yaProcesadoEsteFrame = true;
            return;
        }

        Rigidbody2D rbJug = jugador.GetComponent<Rigidbody2D>();
        if (rbJug == null) return;

        Collider2D cuerpo = ObtenerColliderCuerpo();
        float pieJugador = colJugador.bounds.min.y;
        float cabezaEnemigo;
        if (cuerpo != null)
            cabezaEnemigo = cuerpo.bounds.max.y;
        else
        {
            Collider2D trigger = null;
            foreach (var c in GetComponents<Collider2D>())
                if (c.isTrigger) { trigger = c; break; }
            if (trigger != null)
                cabezaEnemigo = trigger.bounds.max.y;
            else if (sr != null)
                cabezaEnemigo = sr.bounds.max.y;
            else
                return;
        }

        yaProcesadoEsteFrame = true;

        string fuente = cuerpo != null ? "cuerpo" : "trigger/sr";
        Debug.Log($"[{gameObject.name}] pieJugador={pieJugador:F3} cabezaEnemigo={cabezaEnemigo:F3} diferencia={pieJugador - cabezaEnemigo:F3} fuente={fuente}");

        if (pieJugador >= cabezaEnemigo - 0.02f && rbJug.velocity.y <= 0.1f)
        {
            rbJug.velocity = new Vector2(rbJug.velocity.x, 6f);
            RecibirDanio(1);
        }
        else
        {
            MuerteJugador muerte = jugador.GetComponent<MuerteJugador>();
            if (muerte != null)
                muerte.MorirPorEnemigo();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (muerto || congelado) return;
        if (!col.gameObject.CompareTag("Player")) return;
        ManejarContactoJugador(col.otherCollider, col.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (muerto || congelado) return;
        if (!other.CompareTag("Player")) return;
        ManejarContactoJugador(other, other.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (capaSuelo != 0)
        {
            Gizmos.color = Color.green;
            Vector3 origen = transform.position + new Vector3(distanciaBorde, -alturaBorde, 0f);
            Gizmos.DrawLine(origen, origen + Vector3.down * (alturaBorde + 0.1f));
            origen = transform.position + new Vector3(-distanciaBorde, -alturaBorde, 0f);
            Gizmos.DrawLine(origen, origen + Vector3.down * (alturaBorde + 0.1f));
        }

        LayerMask maskGizmo = capaObstaculos != 0 ? capaObstaculos : capaSuelo;
        if (maskGizmo != 0 && player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, player.position);
        }

        if (maskGizmo != 0)
        {
            Gizmos.color = Color.magenta;
            Vector3 der = transform.position + Vector3.right * distanciaObstaculo;
            Gizmos.DrawLine(transform.position, der);
            Vector3 izq = transform.position + Vector3.left * distanciaObstaculo;
            Gizmos.DrawLine(transform.position, izq);
        }
    }
}