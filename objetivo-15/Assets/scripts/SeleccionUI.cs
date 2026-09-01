using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public static class SeleccionUI
{
    public static void AsegurarEventSystem()
    {
        ConfigurarModuloExistente();

        if (EventSystem.current != null) return;

        var existente = Object.FindFirstObjectByType<EventSystem>();
        if (existente != null)
        {
            ConfigurarModulo(existente.GetComponent<StandaloneInputModule>());
            return;
        }

        var go = new GameObject("EventSystem",
            typeof(EventSystem),
            typeof(StandaloneInputModule));
        ConfigurarModulo(go.GetComponent<StandaloneInputModule>());
    }

    static void ConfigurarModuloExistente()
    {
        if (EventSystem.current == null) return;
        ConfigurarModulo(EventSystem.current.GetComponent<StandaloneInputModule>());
    }

    static void ConfigurarModulo(StandaloneInputModule modulo)
    {
        if (modulo == null) return;
        modulo.horizontalAxis = "UIHorizontal";
        modulo.verticalAxis = "UIVertical";
        modulo.submitButton = "Submit";
        modulo.cancelButton = "Cancel";
    }

    public static void SeleccionarPrimero(GameObject raiz)
    {
        if (raiz == null || !raiz.activeInHierarchy) return;

        AsegurarEventSystem();
        ConfigurarModuloExistente();
        var es = EventSystem.current;
        if (es == null) return;

        // Reunir todos los Selectables interactables (los no interactables se saltan
        // para que no rompan la cadena de navegacion).
        List<Selectable> interactables = new List<Selectable>();
        foreach (var s in raiz.GetComponentsInChildren<Selectable>(false))
        {
            if (!s.gameObject.activeInHierarchy) continue;
            PrepararResalte(s);
            if (s.interactable)
                interactables.Add(s);
        }

        // Construir navegacion explicita por vecino mas cercano en cada direccion.
        // Esto hace que D-pad/flechas funcionen de forma predecible en cualquier
        // layout (listas, grids, mapas con nodos dispersos), sin depender de la
        // navegacion automatica de Unity.
        ConfigurarNavegacionExplicita(interactables);

        Selectable primero = null;
        foreach (var s in interactables)
            if (s.isActiveAndEnabled) { primero = s; break; }

        if (primero != null)
        {
            es.SetSelectedGameObject(primero.gameObject);
            Debug.Log($"[SeleccionUI] seleccionado: {primero.name}");
        }
    }

    // Prepara la navegacion (sin seleccionar) entre todos los Selectables interactables
    // bajo la raiz. Sirve para casos como el mapa de niveles, donde se quiere navegar
    // entre todos los nodos pero seleccionar uno concreto distinto del primero.
    public static void PrepararNavegacion(GameObject raiz)
    {
        if (raiz == null) return;

        List<Selectable> interactables = new List<Selectable>();
        foreach (var s in raiz.GetComponentsInChildren<Selectable>(false))
        {
            if (!s.gameObject.activeInHierarchy) continue;
            if (s.interactable)
                interactables.Add(s);
        }
        ConfigurarNavegacionExplicita(interactables);
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

    // ───────────────────── Navegación explícita ─────────────────────

    static void ConfigurarNavegacionExplicita(List<Selectable> botones)
    {
        if (botones == null || botones.Count <= 1) return;

        // Forzar el layout para que GetWorldCorners devuelva posiciones ya calculadas
        // (importante en paneles con LayoutGroup, cuyo layout se aplica despues de Start).
        Canvas.ForceUpdateCanvases();

        var centros = new Dictionary<Selectable, Vector2>();
        foreach (var b in botones)
            centros[b] = ObtenerCentro(b);

        foreach (var b in botones)
        {
            Vector2 c = centros[b];
            var nav = b.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = MasCercanoEnDireccion(botones, centros, c, Vector2.up);
            nav.selectOnDown = MasCercanoEnDireccion(botones, centros, c, Vector2.down);
            nav.selectOnLeft = MasCercanoEnDireccion(botones, centros, c, Vector2.left);
            nav.selectOnRight = MasCercanoEnDireccion(botones, centros, c, Vector2.right);
            b.navigation = nav;
        }
    }

    static Vector2 ObtenerCentro(Selectable s)
    {
        var rt = s.transform as RectTransform;
        if (rt != null)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return (corners[0] + corners[2]) * 0.5f; // centro = promedio esquina inf-izq y sup-der
        }
        return s.transform.position;
    }

    static Selectable MasCercanoEnDireccion(
        List<Selectable> botones,
        Dictionary<Selectable, Vector2> centros,
        Vector2 origen,
        Vector2 dir)
    {
        const float umbralAngulo = 0.3f; // el candidato debe estar mayormente en esa direccion
        Selectable mejor = null;
        float mejorDist = float.MaxValue;

        foreach (var b in botones)
        {
            Vector2 delta = centros[b] - origen;
            if (delta.sqrMagnitude < 0.0001f) continue; // es el mismo

            Vector2 norm = delta.normalized;
            float dot = Vector2.Dot(norm, dir.normalized);
            if (dot < umbralAngulo) continue;

            float dist = delta.sqrMagnitude;
            if (dist < mejorDist)
            {
                mejorDist = dist;
                mejor = b;
            }
        }
        return mejor;
    }
}