using UnityEngine;

public class MotocarroPlataforma : MonoBehaviour
{
    [Header("Movimiento")]
    public Transform[] puntosMovimiento;
    public float velocidadMovimiento = 3f;

    [Header("Ruedas")]
    public Transform ruedaTrasera;
    public Transform ruedaDelantera;
    public float factorRotacion = 100f;

    [Header("Flip")]
    public float toleranciaFlip = 0.01f;

    private int siguientePunto = 1;
    private bool ordenAscendente = true;
    private Vector3 posicionAnterior;
    private bool jugadorEncima = false;
    private Transform jugador;
    private float escalaOriginalX;
    private bool moviendose = false;

    void Start()
    {
        posicionAnterior = transform.position;
        escalaOriginalX = Mathf.Abs(transform.localScale.x);

        if (puntosMovimiento == null || puntosMovimiento.Length == 0)
        {
            Debug.LogWarning("MotocarroPlataforma: No hay puntos de movimiento asignados en " + gameObject.name);
        }
    }

    void Update()
    {
        if (puntosMovimiento == null || puntosMovimiento.Length == 0) return;

        if (ordenAscendente && siguientePunto + 1 >= puntosMovimiento.Length)
            ordenAscendente = false;

        if (!ordenAscendente && siguientePunto <= 0)
            ordenAscendente = true;

        if (Vector2.Distance(transform.position, puntosMovimiento[siguientePunto].position) < 0.1f)
        {
            if (ordenAscendente)
                siguientePunto += 1;
            else
                siguientePunto -= 1;
        }

        Vector3 posAnteriorFrame = transform.position;
        transform.position = Vector2.MoveTowards(transform.position,
            puntosMovimiento[siguientePunto].position,
            velocidadMovimiento * Time.deltaTime);

        Vector3 delta = transform.position - posAnteriorFrame;
        moviendose = delta.magnitude > toleranciaFlip;

        if (jugadorEncima && jugador != null)
            jugador.position += delta;

        if (moviendose)
        {
            float direccion = delta.x;
            if (direccion > toleranciaFlip)
                transform.localScale = new Vector3(escalaOriginalX, transform.localScale.y, 1);
            else if (direccion < -toleranciaFlip)
                transform.localScale = new Vector3(-escalaOriginalX, transform.localScale.y, 1);

            float velocidad = delta.magnitude / Time.deltaTime;
            float anguloRotacion = -velocidad * factorRotacion * Time.deltaTime;

            if (ruedaTrasera != null)
                ruedaTrasera.Rotate(0, 0, anguloRotacion);

            if (ruedaDelantera != null)
                ruedaDelantera.Rotate(0, 0, anguloRotacion);
        }
        else
        {
            if (ruedaTrasera != null)
                ruedaTrasera.Rotate(0, 0, 0);

            if (ruedaDelantera != null)
                ruedaDelantera.Rotate(0, 0, 0);
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