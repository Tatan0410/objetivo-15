using UnityEngine;

public enum TipoPotenciador
{
    Velocidad,
    Inmortalidad,
    VidaExtra
}

public class Potenciador : MonoBehaviour
{
    [Header("Configuraci�n")]
    public TipoPotenciador tipo;
    public float tiempoVida = 10f;

    [Header("Animaci�n flotante")]
    public float velocidadFlotacion = 2f;
    public float alturaFlotacion = 0.15f;

    private Vector3 posInicial;

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        posInicial = transform.position;
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        float nuevoY = posInicial.y +
            Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;
        transform.position = new Vector3(posInicial.x, nuevoY, posInicial.z);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        PlayerController pc = col.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.AplicarPotenciador(tipo);
            Debug.Log("Potenciador aplicado: " + tipo);
        }
        else
        {
            Debug.LogWarning("No se encontr� PlayerController en el jugador");
        }

        Destroy(gameObject);
    }
}