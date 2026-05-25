using UnityEngine;

public class NubesMovimiento : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabNube;
    public int cantidadNubes = 5;
    public float velocidadMin = 0.3f;
    public float velocidadMax = 0.8f;
    public float alturaMin = 2f;
    public float alturaMax = 5f;
    public float rangoHorizontal = 20f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        for (int i = 0; i < cantidadNubes; i++)
            SpawnNube(true);
    }

    void SpawnNube(bool posicionAleatoria)
    {
        if (prefabNube == null) return;

        float x = posicionAleatoria ?
            cam.transform.position.x + Random.Range(-rangoHorizontal, rangoHorizontal) :
            cam.transform.position.x + rangoHorizontal + 5f;

        float y = cam.transform.position.y + Random.Range(alturaMin, alturaMax);

        GameObject nube = Instantiate(prefabNube,
            new Vector3(x, y, 0),
            Quaternion.identity);

        float velocidad = Random.Range(velocidadMin, velocidadMax);
        float escala = Random.Range(0.5f, 1.5f);
        nube.transform.localScale = new Vector3(escala, escala, 1);

        nube.AddComponent<NubeIndividual>().Inicializar(
            velocidad, rangoHorizontal, cam);
    }

    void Update()
    {
        // Spawnear nuevas nubes cuando se necesite
        if (transform.childCount < cantidadNubes)
            SpawnNube(false);
    }
}