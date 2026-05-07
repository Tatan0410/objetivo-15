using UnityEngine;

public class AjustarFondo : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        Camera cam = Camera.main;
        float altoCam = cam.orthographicSize * 2f;
        float anchoCam = altoCam * cam.aspect;

        float altoSprite = sr.sprite.bounds.size.y;
        float anchoSprite = sr.sprite.bounds.size.x;

        transform.localScale = new Vector3(
            anchoCam / anchoSprite,
            altoCam / altoSprite,
            1f);

        transform.position = new Vector3(
            cam.transform.position.x,
            cam.transform.position.y,
            transform.position.z);
    }
}