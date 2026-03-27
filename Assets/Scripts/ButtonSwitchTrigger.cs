using UnityEngine;

public class ButtonSwitchTrigger : MonoBehaviour
{
    public GameObject greenButton;
    public GameObject lockedDoor;
    public GameObject openDoor;

    void Start()
    {
        greenButton.SetActive(false);
        lockedDoor.SetActive(true);
        openDoor.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered by: " + other.name);
        greenButton.SetActive(true);
        lockedDoor.SetActive(false);
        openDoor.SetActive(true);
        gameObject.SetActive(false);
    }
}