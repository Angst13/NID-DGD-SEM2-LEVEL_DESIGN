using UnityEngine;

public class PlayerColorSwitcher : MonoBehaviour
{
    [Header("Player Colors")]
    public Color realWorldColor = Color.white;   
    public Color mirrorWorldColor = Color.black; 

    [Header("Transparency Settings")]
    [Range(0f, 1f)] public float realWorldAlpha = 1f;
    [Range(0f, 1f)] public float mirrorWorldAlpha = 1f;

    private SpriteRenderer sr;
    private WorldSwitcher worldSwitcher;
    private bool lastWorldState;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        worldSwitcher = Object.FindFirstObjectByType<WorldSwitcher>();

        if (worldSwitcher != null)
        {
            lastWorldState = worldSwitcher.isInRealWorld;
            UpdateVisuals();
        }
    }

    void Update()
    {
        if (worldSwitcher == null) return;

        if (worldSwitcher.isInRealWorld != lastWorldState)
        {
            lastWorldState = worldSwitcher.isInRealWorld;
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (worldSwitcher.isInRealWorld)
        {
            Color c = realWorldColor;
            c.a = realWorldAlpha;
            sr.color = c;
        }
        else
        {
            Color c = mirrorWorldColor;
            c.a = mirrorWorldAlpha;
            sr.color = c;
        }
    }
}