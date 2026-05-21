using UnityEngine;

public class ParallaxFondo : MonoBehaviour
{
    public float velocidadParallax = 0.3f;
    private Transform camara;
    private Vector3 posicionAnteriorCamara;

    void Start()
    {
        camara = Camera.main.transform;
        posicionAnteriorCamara = camara.position;
    }

    void LateUpdate()
    {
        Vector3 diferencia = camara.position - posicionAnteriorCamara;
        transform.position += new Vector3(
            diferencia.x * velocidadParallax, 0, 0);
        posicionAnteriorCamara = camara.position;
    }
}