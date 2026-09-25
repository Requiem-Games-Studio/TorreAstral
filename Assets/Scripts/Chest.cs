using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject[] items;

    [Range(0f, 3f)]
    public int dificult;

    public Animator animator;


    public void OpenChest()
    {
        Debug.Log("OpenChest");
        gameObject.tag = "Untagged";
        animator.SetBool("Open", true);
        //Spaw item in the Chest
        SpawnItem();
    }

    public void SpawnItem()
    {
        int randomIndex = Random.Range(0, items.Length);

        Instantiate(
            items[randomIndex],
            transform.position,
            Quaternion.identity
        );
    }

}
