using UnityEngine;

public class RockHead : DañoEnemigo
{
    [SerializeField] private float velocidadBajada = 8f;
    [SerializeField] private float velocidadSubida = 2.5f;
    [SerializeField] private float tiempoEspera = 0.6f;
    [SerializeField] private float distancia = 3f;

    private enum Estado { Bajando, Esperando, Subiendo }

    private Vector3 puntoA;
    private Vector3 puntoB;
    private Estado estado = Estado.Bajando;
    private float temporizador;
    private Rigidbody2D rb;

    private void Start()
    {
        puntoA = transform.position;
        puntoB = puntoA - Vector3.up * distancia;
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
        switch (estado)
        {
            case Estado.Bajando:
                transform.position = Vector3.MoveTowards(transform.position, puntoB, velocidadBajada * Time.deltaTime);
                if (Vector3.Distance(transform.position, puntoB) < 0.01f)
                {
                    temporizador = tiempoEspera;
                    estado = Estado.Esperando;
                }
                break;

            case Estado.Esperando:
                temporizador -= Time.deltaTime;
                if (temporizador <= 0f)
                    estado = Estado.Subiendo;
                break;

            case Estado.Subiendo:
                transform.position = Vector3.MoveTowards(transform.position, puntoA, velocidadSubida * Time.deltaTime);
                if (Vector3.Distance(transform.position, puntoA) < 0.01f)
                    estado = Estado.Bajando;
                break;
        }
    }
}