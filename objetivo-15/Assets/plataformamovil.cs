using UnityEngine;

public class plataformamovil : MonoBehaviour
{
    [SerializeField] private Transform[] puntosMovimiento;
    [SerializeField] private float velocidadMovimiento;
    private int siguientePlataforma = 1;
    private bool ordenPlataformas = true;

    private Vector3 posicionAnterior;
    private bool jugadorEncima = false;
    private Transform jugador;

    void Start()
    {
        posicionAnterior = transform.position;
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

        Vector3 posAnteriorFrame = transform.position;
        transform.position = Vector2.MoveTowards(transform.position,
            puntosMovimiento[siguientePlataforma].position,
            velocidadMovimiento * Time.deltaTime);

        Vector3 delta = transform.position - posAnteriorFrame;
        if (jugadorEncima && jugador != null)
        {
            Rigidbody2D rbJug = jugador.GetComponent<Rigidbody2D>();
            if (rbJug != null)
                rbJug.MovePosition(rbJug.position + (Vector2)delta);
            else
                jugador.position += delta;
        }
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
