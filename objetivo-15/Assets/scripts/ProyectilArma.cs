

using UnityEngine;

public class ProyectilArma : MonoBehaviour
{
    public float velocidad = 12f;
    public int danio = 1;
    public float tiempoVida = 3f;
    private Vector2 direccion;

    public void Iniciar(Vector2 dir)
    {
        direccion = dir.normalized;
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        transform.Translate(direccion * velocidad * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemigo"))
        {
            col.SendMessage("RecibirDanio", danio,
                            SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
        else if (!col.CompareTag("Player") && !col.CompareTag("Plastico"))
        {
            Destroy(gameObject);
        }
    }
}