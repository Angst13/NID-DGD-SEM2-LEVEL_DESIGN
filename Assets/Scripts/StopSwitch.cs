using UnityEngine;

public class StopSwitch : MonoBehaviour
{
    [SerializeField] private WorldSwitcher worldSwitcher;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entered stop switch area");
        if (other.CompareTag("Player"))
        {
            worldSwitcher.canSwitch = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Player exited stop switch area");
        if (other.CompareTag("Player"))
        {
            worldSwitcher.canSwitch = true;
        }
    }
}
