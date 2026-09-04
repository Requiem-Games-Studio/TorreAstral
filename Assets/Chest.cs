using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject[] items;

    [Range(0f, 3f)]
    public int dificult;


    public void OpenChest()
    {
        Debug.Log("OpenChest");
        gameObject.tag = "Untagged";
        //Spaw item in the Chest
    }

}
