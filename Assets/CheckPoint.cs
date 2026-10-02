using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CheckPoint : MonoBehaviour
{
    public bool active;
    public Light2D light2D;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !active)
        {
            active = true;
            light2D.enabled = true;
            Debug.Log("SaveGame");
        }
    }


}
