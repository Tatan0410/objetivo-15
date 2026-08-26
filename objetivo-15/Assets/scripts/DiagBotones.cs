using UnityEngine;

public static class DiagBotones
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Iniciar()
    {
        var go = new GameObject("DiagBotones");
        go.AddComponent<Lector>();
        Object.DontDestroyOnLoad(go);
    }

    class Lector : MonoBehaviour
    {
        void Update()
        {
            for (int i = 0; i <= 19; i++)
            {
                var kc = KeyCode.JoystickButton0 + i;
                if (Input.GetKeyDown(kc))
                    Debug.Log($"[DiagBotones] JoystickButton{i}");
            }
        }
    }
}
