using Unity.VisualScripting;
using UnityEngine;

public class HandScript : MonoBehaviour
{

    public Equipment equipment;

    public GameObject objectTaken;
    public Transform hand,leftHand;
    public CapsuleCollider2D triggerCollider;
    public bool taken;

    //Controlado desde PlayerAnimatorController
    public bool active,left;

    public float trowSpeed;

    public ItemObject itemObject;

    public void TakeObject(GameObject obect)
    {
        Debug.Log("Objecto Tomado!!!");
        objectTaken = obect;
        objectTaken.layer = 8;
        itemObject = objectTaken.GetComponent<ItemObject>();
        if(itemObject != null )
        {
            itemObject.equipment = equipment;
        }
    }


    private void Update()
    {
        if(taken)
        {
            if(left)
            {
                objectTaken.transform.position = leftHand.position;
            }
            else
            {
                objectTaken.transform.position = hand.position;
            }          
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active)
        {
            if (objectTaken == null && collision.CompareTag("Object") && !taken)
            {
                taken = true;
                TakeObject(collision.gameObject);
            }
        }
    }

    public void ActiveHand(bool activen)
    {
        active = activen;

        triggerCollider.enabled = active;

    }

    public void TrowObject()
    {     
        Debug.Log("Trow object");
        taken = false;
        Rigidbody2D rig = objectTaken.GetComponent<Rigidbody2D>();
        if (left)
        {
            rig.AddForce(transform.up * 10 * trowSpeed);
            rig.AddForce(transform.right * -100 * trowSpeed);
        }
        else
        {
            rig.AddForce(transform.up * 10 * trowSpeed);
            rig.AddForce(transform.right * 100 * trowSpeed);
        }
        objectTaken.layer = 3;
        objectTaken = null;
        itemObject = null;
    }

    public void AddOrDrop()
    {       
        if (itemObject != null)
        {
            itemObject.AddItemToPlayer();
        }
        else
        {
            objectTaken.layer = 3;
        }
        
        taken = false;
        objectTaken = null;
        itemObject = null;
    }

}
