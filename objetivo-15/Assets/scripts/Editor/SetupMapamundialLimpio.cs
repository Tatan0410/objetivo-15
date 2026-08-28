using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class SetupMapamundialLimpio
{
    [MenuItem("Objetivo15/Configurar Mapamundial Limpio")]
    public static void Configurar()
    {
        string path = "Assets/Scenes/Mapamundial.unity";
        var scene = EditorSceneManager.OpenScene(path);

        // Eliminar FondoLimpio separado si existe (ahora se usa swap de sprite)
        var viejoLimpio = GameObject.Find("FondoLimpio");
        if (viejoLimpio != null)
        {
            Object.DestroyImmediate(viejoLimpio);
            Debug.Log("FondoLimpio separado eliminado (ahora se usa swap de sprite).");
        }

        var manager = Object.FindFirstObjectByType<MapamundialEstadoManager>();
        if (manager == null)
        {
            var go = new GameObject("MapamundialEstadoManager");
            manager = go.AddComponent<MapamundialEstadoManager>();
        }

        // Buscar fondo: primero GameObject "fondo" (SpriteRenderer)
        var goFondo = GameObject.Find("fondo");
        if (goFondo != null)
        {
            var sr = goFondo.GetComponent<SpriteRenderer>();
            var img = goFondo.GetComponent<Image>();
            if (sr != null) Debug.Log($"Fondo encontrado: {goFondo.name} SpriteRenderer sprite={sr.sprite?.name}");
            else if (img != null) { manager.imagenFondo = img; Debug.Log($"Fondo encontrado: {goFondo.name} Image sprite={img.sprite?.name}"); }
            else Debug.LogWarning("fondo sin SpriteRenderer ni Image");
        }

        // Configurar referencias de botones (incluye inactivos) y contenedor
        GameObject rejugar = null, menuPrincipal = null;
        foreach (var btn in Resources.FindObjectsOfTypeAll<UnityEngine.UI.Button>())
        {
            if (btn.gameObject.name == "BotonRejugar") rejugar = btn.gameObject;
            if (btn.gameObject.name == "BotonMenuPrincipal") menuPrincipal = btn.gameObject;
        }
        if (rejugar == null) rejugar = GameObject.Find("BotonRejugar");
        if (menuPrincipal == null) menuPrincipal = GameObject.Find("BotonMenuPrincipal");

        GameObject contNiveles = null;
        var selectorTmp = Object.FindFirstObjectByType<SelectorNivelesMapa>();
        if (selectorTmp != null && selectorTmp.nodos.Length > 0 && selectorTmp.nodos[0].boton != null)
            contNiveles = selectorTmp.nodos[0].boton.transform.parent.gameObject;

        // Asignar Image si existe, si no quedará null y runtime buscará SpriteRenderer
        var canvas = Object.FindFirstObjectByType<Canvas>();
        Image fondoImg = null;
        if (goFondo != null) fondoImg = goFondo.GetComponent<Image>();
        if (fondoImg == null && goFondo == null && canvas != null)
        {
            float maxArea = 0f;
            foreach (var img in canvas.GetComponentsInChildren<Image>(true))
            {
                if (img.gameObject.name.ToLower().Contains("panel") || img.gameObject.name.ToLower().Contains("confirmacion")) continue;
                var rt = img.GetComponent<RectTransform>();
                if (rt == null) continue;
                float area = rt.rect.width * rt.rect.height;
                if (area <= 1f) area = rt.sizeDelta.x * rt.sizeDelta.y;
                if (area > maxArea) { maxArea = area; fondoImg = img; }
            }
        }
        manager.imagenFondo = fondoImg;
        manager.botonRejugar = rejugar;
        manager.botonMenuPrincipal = menuPrincipal;
        manager.contenedorNiveles = contNiveles;
        if (fondoImg != null) Debug.Log($"Fondo Image asignado: {fondoImg.gameObject.name}");
        else if (goFondo != null) Debug.Log($"Fondo SpriteRenderer asignado: {goFondo.name}");
        else Debug.Log("Fondo no asignado, runtime buscará 'fondo'");

        EditorUtility.SetDirty(manager.gameObject);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, path);
        Debug.Log("MapamundialEstadoManager configurado (solo swap de sprite) en " + path);
    }
}
