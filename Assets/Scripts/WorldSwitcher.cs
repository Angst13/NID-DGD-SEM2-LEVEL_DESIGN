using UnityEngine;

public class WorldSwitcher : MonoBehaviour
{
    [Header("World Objects")]
    public GameObject realWorld;
    public GameObject mirrorWorld;

    [Header("Switch Settings")]
    public float switchCooldown = 0.0f;  // prevent rapid switching

    public bool isInRealWorld = true;
    private float lastSwitchTime = 0f;

    public bool canSwitch = true;

    void Start()
    {
        // Make sure we start in real world
        realWorld.SetActive(true);
        mirrorWorld.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && CanSwitch())
        {
            if (canSwitch)
            {
                SwitchWorld();
            }
        }
    }

    void SwitchWorld()
    {
        isInRealWorld = !isInRealWorld;
        lastSwitchTime = Time.time;

        realWorld.SetActive(isInRealWorld);
        mirrorWorld.SetActive(!isInRealWorld);

        Debug.Log("Switched to: " + (isInRealWorld ? "Real World" : "Mirror World"));
    }

    bool CanSwitch()
    {
        return Time.time - lastSwitchTime >= switchCooldown;
    }
}