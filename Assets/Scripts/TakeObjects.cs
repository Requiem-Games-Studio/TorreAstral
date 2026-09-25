using UnityEngine;

public class TakeObjects : MonoBehaviour
{
    public bool handRight;
    public Transform HandR;

    public bool takenObject;

    public Transform objectToUse;

    private void Start()
    {
        takenObject = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Object"))
        {
            collision.GetComponent("Object").SendMessage("Detection");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Object"))
        {
            collision.GetComponent("Object").SendMessage("NoDetection");
        }
    }

    private void Update()
    {
        this.transform.position = HandR.position;
        this.transform.rotation = HandR.rotation;
    }

    public void PlayUseObjetAnimation()
    {
        objectToUse = this.gameObject.transform.GetChild(0);
        //PlayTargetAnimation("Body_UseObject");
    }

    public void UseObjet()
    {
        objectToUse.GetComponent("Object").SendMessage("ObjectAction");
    }

    public void DropTakenObject()
    {
        if (takenObject)
        {
            objectToUse = this.gameObject.transform.GetChild(0);
            objectToUse.GetComponent("Object").SendMessage("DropObjectAction");
        }
    }

}
