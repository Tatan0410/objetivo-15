using UnityEngine;

public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod]
    static void Inicializar()
    {
        if (Object.FindFirstObjectByType<SceneTransitionManager>() == null)
        {
            GameObject stm = new GameObject("SceneTransitionManager");
            stm.AddComponent<SceneTransitionManager>();
        }

        if (Object.FindFirstObjectByType<GameOverManager>() == null)
        {
            GameObject gom = new GameObject("GameOverManager");
            gom.AddComponent<GameOverManager>();
        }
    }
}
