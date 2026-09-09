using UnityEngine;

// Datos serializables de un elemento del HUD (JsonUtility no soporta diccionarios).
[System.Serializable]
public class HudElementoData
{
    public string id;
    public Vector2 posicion;
    public Color color;
}

[System.Serializable]
public class HudConfigData
{
    public HudElementoData[] elementos;
}

// Guarda/carga el layout personalizado del HUD en PlayerPrefs, por jugador.
// Al cambiar de usuario, CambiarUsuarioManager hace PlayerPrefs.DeleteAll(),
// por lo que el layout se limpia automaticamente al iniciar sesion otro jugador.
public static class HudConfig
{
    const string CLAVE_BASE = "HudLayout_";

    static string Clave()
    {
        string nombre = EstadisticasManager.instancia != null
            ? EstadisticasManager.instancia.nombreJugador
            : PlayerPrefs.GetString("NombreJugador", "Jugador");
        return CLAVE_BASE + nombre;
    }

    public static bool TieneGuardado()
    {
        return PlayerPrefs.HasKey(Clave());
    }

    public static HudElementoData[] Cargar()
    {
        if (!TieneGuardado()) return null;
        string json = PlayerPrefs.GetString(Clave());
        try
        {
            var data = JsonUtility.FromJson<HudConfigData>(json);
            return data != null ? data.elementos : null;
        }
        catch
        {
            return null;
        }
    }

    public static void Guardar(HudElementoData[] elementos)
    {
        var data = new HudConfigData { elementos = elementos };
        PlayerPrefs.SetString(Clave(), JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public static void Borrar()
    {
        if (PlayerPrefs.HasKey(Clave()))
            PlayerPrefs.DeleteKey(Clave());
    }
}