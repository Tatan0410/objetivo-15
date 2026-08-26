using UnityEngine;
using System.Collections;

public class PlasticoSpawnRunner : MonoBehaviour
{
    public void Iniciar(GameObject[] prefabs, int cantidad, Vector3 posicion, float intervalo)
    {
        if (prefabs == null || prefabs.Length == 0 || cantidad <= 0)
        {
            Destroy(gameObject);
            return;
        }
        StartCoroutine(EjecutarSpawn(prefabs, cantidad, posicion, intervalo));
    }

    IEnumerator EjecutarSpawn(GameObject[] prefabs, int cantidad, Vector3 posicion, float intervalo)
    {
        Debug.Log($"[PlasticoSpawnRunner] Iniciando spawn cantidad={cantidad} pos={posicion} escena={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        for (int i = 0; i < cantidad && i < prefabs.Length; i++)
        {
            if (prefabs[i] == null)
            {
                Debug.LogWarning($"[PlasticoSpawnRunner] prefab null en indice {i}");
                continue;
            }
            Vector3 offset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
            Vector3 spawnPos = posicion + offset;
            GameObject go = Instantiate(prefabs[i], spawnPos, Quaternion.identity);
            Debug.Log($"[PlasticoSpawnRunner] Instanciado {prefabs[i].name} en {spawnPos} -> {go.name}");
            yield return new WaitForSeconds(intervalo);
        }
        Destroy(gameObject);
    }
}
