using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class VerificarCambiarUsuario
{
    [MenuItem("Objetivo15/Verificar CambiarUsuario")]
    public static void Verificar()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/menuprincipal.unity");
        string[] nombres = { "BotonCambiarUsuario", "BotonConfirmarCambiar", "BotonCancelarCambiar", "BotonConfirmarSiCambiar", "BotonConfirmarNoCambiar" };
        foreach (var nombre in nombres)
        {
            GameObject go = null;
            foreach (var b in Resources.FindObjectsOfTypeAll<Button>())
                if (b.gameObject.name == nombre) { go = b.gameObject; break; }
            if (go == null) go = GameObject.Find(nombre);
            if (go == null)
            {
                Debug.LogError($"[Verificar] {nombre}: NO ENCONTRADO");
                continue;
            }
            var btn = go.GetComponent<Button>();
            if (btn == null) { Debug.LogError($"[Verificar] {nombre}: sin Button"); continue; }
            int count = btn.onClick.GetPersistentEventCount();
            string metodo = count > 0 ? btn.onClick.GetPersistentMethodName(0) : "N/A";
            string target = count > 0 && btn.onClick.GetPersistentTarget(0) != null ? btn.onClick.GetPersistentTarget(0).GetType().Name : "null";
            if (count == 1) Debug.Log($"[Verificar] {nombre}: OK 1 persistente -> {target}.{metodo}");
            else Debug.LogError($"[Verificar] {nombre}: ERROR esperado 1 persistente, actual {count} -> {metodo}");
            // Verificar texto si es BotonCambiarUsuario
            if (nombre == "BotonCambiarUsuario")
            {
                var txt = go.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (txt != null) Debug.Log($"[Verificar] Texto BotonCambiarUsuario: '{txt.text}' (esperado 'Jugador' sin emoji)");
            }
        }
        Debug.Log("[Verificar] Fin verificación");
    }
}
