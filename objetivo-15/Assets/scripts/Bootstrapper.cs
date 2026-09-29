using UnityEngine;

public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod]
    static void Inicializar()
    {
        Input.simulateMouseWithTouches = false;
        if (Object.FindFirstObjectByType<SceneTransitionManager>() == null)
        {
            GameObject stm = new GameObject("SceneTransitionManager");
            stm.AddComponent<SceneTransitionManager>();
        }

        if (Object.FindFirstObjectByType<GameOverManager>() == null)
        {
            GameObject prefabGO = Resources.Load<GameObject>("GameOverManager");
            if (prefabGO != null)
            {
                GameObject.Instantiate(prefabGO);
                // El prefab debe tener panelGameOverPrefab asignado en el Inspector.
                // Si no, el GameOverManager Awake() creará el canvas desde el prefab.
            }
            else
            {
                GameObject gom = new GameObject("GameOverManager");
                gom.AddComponent<GameOverManager>();
            }
        }

        if (Object.FindFirstObjectByType<VidasManager>() == null)
        {
            GameObject vm = new GameObject("VidasManager");
            vm.AddComponent<VidasManager>();
        }

        if (EstadisticasManager.instancia == null)
        {
            GameObject prefabEM = Resources.Load<GameObject>("EstadisticasManager");
            if (prefabEM != null)
                GameObject.Instantiate(prefabEM);
            else
            {
                GameObject em = new GameObject("EstadisticasManager");
                em.AddComponent<EstadisticasManager>();
            }
        }

        if (GameOverManager.instancia == null)
        {
            GameObject prefabGO = Resources.Load<GameObject>("GameOverManager");
            if (prefabGO != null)
                GameObject.Instantiate(prefabGO);
            else
            {
                GameObject gom = new GameObject("GameOverManager");
                gom.AddComponent<GameOverManager>();
            }
        }
    }
}
