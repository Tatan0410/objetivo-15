using UnityEngine;

public class SpikeHead : DañoEnemigo
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rango = 4f;
    [SerializeField] private float checkDelay = 0.4f;

    private float checkTimer;
    private Vector3 destino;
    private bool atacking;
    private bool regresando;
    private Vector3 posicionInicial;
    private Rigidbody2D rb;

    private void Start()
    {
        posicionInicial = transform.position;
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX
                           | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Update()
    {
        if (atacking)
        {
            transform.Translate(destino.normalized * Time.deltaTime * speed);

            if (destino.y < 0f)
            {
                if (TocaSuelo())
                {
                    atacking = false;
                    regresando = true;
                }
            }
            else if (!JugadorEnDireccion(destino))
            {
                atacking = false;
                regresando = true;
            }
        }
        else if (regresando)
        {
            Vector3 dir = posicionInicial - transform.position;
            transform.Translate(dir.normalized * Time.deltaTime * speed);

            if (Vector3.Distance(transform.position, posicionInicial) < 0.05f)
            {
                transform.position = posicionInicial;
                regresando = false;
                checkTimer = 0f;
            }
        }
        else
        {
            checkTimer += Time.deltaTime;
            if (checkTimer > checkDelay)
                CheckForPlayer();
        }
    }

    private void CheckForPlayer()
    {
        checkTimer = 0f;

        Vector2[] direcciones = { Vector2.up, Vector2.down };
        foreach (Vector2 dir in direcciones)
        {
            Debug.DrawRay(transform.position, dir * rango, Color.red);

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, rango);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null || hit.collider.gameObject == gameObject) continue;
                if (hit.collider.CompareTag("Player"))
                {
                    destino = dir;
                    atacking = true;
                    return;
                }
                else
                    break;
            }
        }
    }

    private bool JugadorEnDireccion(Vector3 dir)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, rango);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null || hit.collider.gameObject == gameObject) continue;
            return hit.collider.CompareTag("Player");
        }
        return false;
    }

    private bool TocaSuelo()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 0.1f);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null || hit.collider.gameObject == gameObject) continue;
            if (hit.collider.CompareTag("Player")) continue;
            return true;
        }
        return false;
    }
}