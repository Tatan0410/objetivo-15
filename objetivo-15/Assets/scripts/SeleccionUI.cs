using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class SeleccionUI
{
    public static void AsegurarEventSystem()
    {
        if (EventSystem.current != null) return;

        var existente = Object.FindFirstObjectByType<EventSystem>();
        if (existente != null) return;

        new GameObject("EventSystem",
            typeof(EventSystem),
            typeof(StandaloneInputModule));
    }

    public static void SeleccionarPrimero(GameObject raiz)
    {
        if (raiz == null || !raiz.activeInHierarchy) return;

        AsegurarEventSystem();
        var es = EventSystem.current;
        if (es == null) return;

        Selectable primero = null;
        foreach (var s in raiz.GetComponentsInChildren<Selectable>(false))
        {
            if (!s.gameObject.activeInHierarchy) continue;
            PrepararResalte(s);
            if (primero == null && s.interactable)
                primero = s;
        }

        if (primero != null)
        {
            es.SetSelectedGameObject(primero.gameObject);
            Debug.Log($"[SeleccionUI] seleccionado: {primero.name}");
        }
    }

    static void PrepararResalte(Selectable s)
    {
        Color resalte = new Color(0.35f, 0.9f, 0.45f);

        var c = s.colors;
        c.selectedColor = resalte;
        c.highlightedColor = resalte;
        c.fadeDuration = 0.05f;
        s.colors = c;

        if (s.GetComponent<ResalteBoton>() == null)
            s.gameObject.AddComponent<ResalteBoton>();
    }

    public static void LimpiarSeleccion()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
}
