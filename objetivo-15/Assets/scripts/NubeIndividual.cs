using UnityEngine;

public class NubeIndividual : MonoBehaviour
{
    private float velocidad;
    private float rangoHorizontal;
    private Camera cam;
    private float alturaFija;

    public void Inicializar(float vel, float rango, Camera camara)
    {
        velocidad = vel;
        rangoHorizontal = rango;
        cam = camara;
        // Guardar altura fija relativa a la cámara al momento de spawnear
        alturaFija = transform.position.y;
    }

    void Update()
    {
        transform.position += Vector3.right * velocidad * Time.deltaTime;

        if (transform.position.x > cam.transform.position.x + rangoHorizontal + 5f)
        {
            transform.position = new Vector3(
                cam.transform.position.x - rangoHorizontal - 5f,
                cam.transform.position.y + Random.Range(2f, 5f),
                0);
        }
    }
}