using UnityEngine;

public class ParallaxFondo : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadParallax = 0.3f;
    public bool repetirHorizontal = true;

    private Transform camara;
    private float anchoSprite;
    private Vector3 posicionInicial;
    private float posicionInicialCamara;

    void Start()
    {
        camara = Camera.main.transform;
        posicionInicial = transform.position;
        posicionInicialCamara = camara.position.x;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            anchoSprite = sr.bounds.size.x;
    }

    void LateUpdate()
    {
        float desplazamiento = (camara.position.x - posicionInicialCamara)
            * velocidadParallax;

        transform.position = new Vector3(
            posicionInicial.x + desplazamiento,
            transform.position.y,
            transform.position.z);

        // Repetir el fondo si el jugador se aleja mucho
        if (repetirHorizontal && anchoSprite > 0)
        {
            float distancia = camara.position.x - transform.position.x;
            if (Mathf.Abs(distancia) >= anchoSprite)
            {
                float offset = Mathf.Sign(distancia) * anchoSprite;
                transform.position = new Vector3(
                    transform.position.x + offset,
                    transform.position.y,
                    transform.position.z);
            }
        }
    }
}