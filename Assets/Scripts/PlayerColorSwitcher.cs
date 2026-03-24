using UnityEngine;

public class PlayerColorSwitcher : MonoBehaviour
{
    [Header("Player Colors")]
    public Color realWorldColor = Color.white;   // change in Inspector
    public Color mirrorWorldColor = Color.black; // change in Inspector

    private SpriteRenderer sr;
    private WorldSwitcher worldSwitcher;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        worldSwitcher = FindObjectOfType<WorldSwitcher>();
    }

    void Update()
    {
        if (worldSwitcher == null) return;

        if (worldSwitcher.isInRealWorld)
            sr.color = realWorldColor;
        else
            sr.color = mirrorWorldColor;
    }
}