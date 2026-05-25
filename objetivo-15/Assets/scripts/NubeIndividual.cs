using UnityEngine;

public class NubeIndividual : MonoBehaviour
{
    private float velocidad;
    private float rangoHorizontal;
    private Camera cam;

    public void Inicializar(float vel, float rango, Camera camara)
    {
        velocidad = vel;
        rangoHorizontal = rango;
        cam = camara;
    }

    void Update()
    {
        // Mover hacia la derecha
        transform.position += Vector3.right * velocidad * Time.deltaTime;

        // Si salió del rango visible reaparecer al otro lado
        if (transform.position.x > cam.transform.position.x + rangoHorizontal + 5f)
        {
            float y = cam.transform.position.y + Random.Range(2f, 5f);
            transform.position = new Vector3(
                cam.transform.position.x - rangoHorizontal - 5f,
                y, 0);
        }
    }
}