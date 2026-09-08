using UnityEngine;

public class SafeArea : MonoBehaviour
{
    void Start()
    {
        Aplicar();
    }

    void Update()
    {
        Rect nueva = Screen.safeArea;
        if (nueva != areaPrevia)
        {
            areaPrevia = nueva;
            Aplicar();
        }
    }

    Rect areaPrevia;

    void Aplicar()
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt == null) return;

        Rect safe = Screen.safeArea;
        if (Screen.width <= 0 || Screen.height <= 0) return;

        Vector2 min = safe.position;
        Vector2 max = safe.position + safe.size;
        min.x /= Screen.width;
        min.y /= Screen.height;
        max.x /= Screen.width;
        max.y /= Screen.height;

        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}