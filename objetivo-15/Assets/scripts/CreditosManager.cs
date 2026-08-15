using UnityEngine;

public class CreditosManager : MonoBehaviour
{
    public void VolverAlMenu()
    {
        SceneTransitionManager.CargarEscenaConFallback("menuprincipal");
    }
}