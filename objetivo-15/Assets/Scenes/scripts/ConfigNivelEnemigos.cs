using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public static class ConfigNivelEnemigos
{
    public static int VidasEnemigo()
    {
        var m = Regex.Match(SceneManager.GetActiveScene().name, @"nivel(\d+)");
        if (!m.Success) return 3;
        int nivel;
        if (!int.TryParse(m.Groups[1].Value, out nivel)) return 3;

        switch (nivel)
        {
            case 1: return 1;
            case 2: return 3;
            case 3:
            case 4: return 5;
            case 5: return 7;
            case 6: return 10;
            default: return 3;
        }
    }
}