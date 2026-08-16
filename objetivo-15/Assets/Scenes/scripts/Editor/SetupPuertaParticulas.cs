using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class SetupPuertaParticulas
{
    static readonly string[] ESCENAS =
    {
        "Assets/Scenes/nivel1_colegio.unity",
        "Assets/Scenes/nivel2_hipodromo.unity",
        "Assets/Scenes/nivel3_mercado.unity",
        "Assets/Scenes/nivel4_basurero.unity",
        "Assets/Scenes/nivel5_subterraneo.unity",
        "Assets/Scenes/nivel6_empresa.unity"
    };

    [MenuItem("Tools/Puerta/1. Agregar particulas a las puertas")]
    public static void Ejecutar()
    {
        string escenaOriginal = EditorSceneManager.GetActiveScene().path;

        foreach (string ruta in ESCENAS)
        {
            Scene escena = EditorSceneManager.OpenScene(ruta, OpenSceneMode.Single);

            GameObject puerta = GameObject.Find("FinNivel");
            if (puerta == null)
            {
                Debug.LogWarning("[SetupPuertaParticulas] No se encontro 'FinNivel' en " + ruta);
                continue;
            }

            Transform hijo = puerta.transform.Find("ParticulasPuerta");
            GameObject goParticulas;
            ParticleSystem ps;

            if (hijo == null)
            {
                goParticulas = new GameObject("ParticulasPuerta");
                goParticulas.transform.SetParent(puerta.transform, false);
                ps = goParticulas.AddComponent<ParticleSystem>();
            }
            else
            {
                goParticulas = hijo.gameObject;
                ps = goParticulas.GetComponent<ParticleSystem>();
                if (ps == null)
                    ps = goParticulas.AddComponent<ParticleSystem>();
            }

            ConfigurarParticulas(ps);

            FinNivel fin = puerta.GetComponent<FinNivel>();
            if (fin != null)
            {
                fin.particulas = ps;
                EditorUtility.SetDirty(fin);
            }
            else
            {
                Debug.LogWarning("[SetupPuertaParticulas] 'FinNivel' sin componente FinNivel en " + ruta);
            }

            EditorUtility.SetDirty(goParticulas);
            EditorSceneManager.SaveScene(escena);
            Debug.Log("[SetupPuertaParticulas] Particulas doradas agregadas en " + ruta);
        }

        if (!string.IsNullOrEmpty(escenaOriginal))
            EditorSceneManager.OpenScene(escenaOriginal, OpenSceneMode.Single);
        else
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        AssetDatabase.SaveAssets();
        Debug.Log("[SetupPuertaParticulas] Listo. Brillo dorado ascendente agregado a todas las puertas.");
    }

    static void ConfigurarParticulas(ParticleSystem ps)
    {
        var so = new SerializedObject(ps);
        so.Update();

        SetBool(so, "looping", true);
        SetBool(so, "playOnAwake", true);
        SetCurva(so, "startLifetime", 1.4f);
        SetCurva(so, "startSpeed", 0.7f);
        SetCurva(so, "startSize", 0.12f);
        SetCurva(so, "gravityModifier", -0.3f);
        SetEntero(so, "maxNumParticles", 40);

        SerializedProperty colorInicial = so.FindProperty("startColor");
        if (colorInicial != null)
        {
            SetEnteroRel(colorInicial, "minMaxState", 0);
            SetColorRel(colorInicial, "minColor", new Color(1f, 0.9f, 0.3f, 1f));
            SetColorRel(colorInicial, "maxColor", new Color(1f, 0.55f, 0.05f, 1f));
        }

        SerializedProperty emision = so.FindProperty("emissionModule");
        if (emision != null)
        {
            SerializedProperty tasa = emision.FindPropertyRelative("rateOverTime");
            if (tasa != null) SetCurva(tasa, 12f);
        }

        SerializedProperty forma = so.FindProperty("shapeModule");
        if (forma != null)
        {
            SerializedProperty tipo = forma.FindPropertyRelative("shapeType");
            if (tipo != null) tipo.intValue = (int)ParticleSystemShapeType.Cone;
            SerializedProperty angulo = forma.FindPropertyRelative("angle");
            if (angulo != null) angulo.floatValue = 8f;
            SerializedProperty radio = forma.FindPropertyRelative("radius");
            if (radio != null) radio.floatValue = 0.35f;
        }

        so.ApplyModifiedProperties();

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortingOrder = 10;
        renderer.sortMode = ParticleSystemSortMode.YoungestInFront;

        Material mat = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");
        if (mat != null)
            renderer.material = mat;
    }

    static void SetBool(SerializedObject so, string nombre, bool valor)
    {
        SerializedProperty p = so.FindProperty(nombre);
        if (p != null) p.boolValue = valor;
    }

    static void SetEntero(SerializedObject so, string nombre, int valor)
    {
        SerializedProperty p = so.FindProperty(nombre);
        if (p != null) p.intValue = valor;
    }

    static void SetCurva(SerializedObject so, string nombre, float valor)
    {
        SerializedProperty p = so.FindProperty(nombre);
        if (p != null) SetCurva(p, valor);
    }

    static void SetCurva(SerializedProperty curva, float valor)
    {
        SerializedProperty estado = curva.FindPropertyRelative("minMaxState");
        if (estado != null) estado.intValue = 0;
        SerializedProperty scalar = curva.FindPropertyRelative("scalar");
        if (scalar != null) scalar.floatValue = valor;
    }

    static void SetEnteroRel(SerializedProperty padre, string nombre, int valor)
    {
        SerializedProperty p = padre.FindPropertyRelative(nombre);
        if (p != null) p.intValue = valor;
    }

    static void SetColorRel(SerializedProperty padre, string nombre, Color color)
    {
        SerializedProperty p = padre.FindPropertyRelative(nombre);
        if (p != null) p.colorValue = color;
    }
}