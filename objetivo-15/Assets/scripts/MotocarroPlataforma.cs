using UnityEngine;

public class MotocarroPlataforma : MonoBehaviour
{
    [SerializeField] private Transform[] puntosMovimiento;
    [SerializeField] private float velocidadMovimiento;

    private int siguientePlataforma = 1;
    private bool ordenPlataformas = true;
    private Vector3 posicionAnterior;
    private bool jugadorEncima = false;
    private Transform jugador;

    private SpriteRenderer sr;
    private float escalaOriginalX;

    void Start()
    {
        posicionAnterior = transform.position;
        sr = GetComponent<SpriteRenderer>();
        escalaOriginalX = Mathf.Abs(transform.localScale.x);
    }

    void Update()
    {
        if (puntosMovimiento == null || puntosMovimiento.Length == 0) return;

        if (ordenPlataformas && siguientePlataforma + 1 >= puntosMovimiento.Length)
            ordenPlataformas = false;
        if (!ordenPlataformas && siguientePlataforma <= 0)
            ordenPlataformas = true;

        if (Vector2.Distance(transform.position, puntosMovimiento[siguientePlataforma].position) < 0.1f)
        {
            if (ordenPlataformas)
                siguientePlataforma += 1;
            else
                siguientePlataforma -= 1;
        }

        // Voltear el sprite según la dirección hacia el punto objetivo
        float direccionX = puntosMovimiento[siguientePlataforma].position.x - transform.position.x;
        if (Mathf.Abs(direccionX) > 0.01f)
        {
            float signo = direccionX > 0 ? 1f : -1f;
            transform.localScale = new Vector3(
                escalaOriginalX * signo,
                transform.localScale.y,
                transform.localScale.z);
        }

        Vector3 posAnteriorFrame = transform.position;
        transform.position = Vector2.MoveTowards(transform.position,
            puntosMovimiento[siguientePlataforma].position,
            velocidadMovimiento * Time.deltaTime);
        Vector3 delta = transform.position - posAnteriorFrame;

        if (jugadorEncima && jugador != null)
            jugador.position += delta;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        if (other.contactCount == 0) return;

        ContactPoint2D contacto = other.GetContact(0);
        if (contacto.normal.y < -0.5f)
        {
            jugadorEncima = true;
            jugador = other.transform;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        jugadorEncima = false;
        jugador = null;
    }
}