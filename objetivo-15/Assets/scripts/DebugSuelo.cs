using UnityEngine;

public class DebugSuelo : MonoBehaviour
{
    public Transform puntoSuelo;
    public float radio = 0.2f;
    public LayerMask capa;

    void Update()
    {
        bool detecta = Physics2D.OverlapCircle(puntoSuelo.position, radio, capa);
        Debug.Log("En suelo: " + detecta + " | Espacio presionado: " + Input.GetButtonDown("Jump"));
    }
}