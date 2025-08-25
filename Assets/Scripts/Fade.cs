using UnityEngine;

public class Fade : MonoBehaviour
{
    public PathFollower pathFollower;
    public int triggerIndex = 2;
    public float fadeSpeed = 1f;
    public string areaID = "Area1"; //  name for this area

    private SpriteRenderer[] spriteRenderers;
    private bool isFading = false;

    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // If this area was already unlocked, delete 
        if (PlayerPrefs.GetInt(areaID, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!isFading && pathFollower != null && pathFollower.CurrentIndex == triggerIndex)
        {
            isFading = true;
        }

        if (isFading && spriteRenderers != null && spriteRenderers.Length > 0)
        {
            foreach (var sr in spriteRenderers)
            {
                if (sr != null)
                {
                    Color color = sr.color;
                    color.a -= fadeSpeed * Time.deltaTime;
                    sr.color = color;
                }
            }

            if (spriteRenderers[0].color.a <= 0f)
            {
                // Save that this area is unlocked
                PlayerPrefs.SetInt(areaID, 1);
                PlayerPrefs.Save();

                Destroy(gameObject);
            }
        }
    }
}