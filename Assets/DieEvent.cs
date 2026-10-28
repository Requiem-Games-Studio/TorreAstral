using UnityEngine;

public class DieEvent : MonoBehaviour
{
    public GameObject parentObject;
    public GameObject particleDeath;

    public void PlayParticle()
    {
        particleDeath.SetActive(true);
    }

    public void DestroyObject()
    {
        Destroy(parentObject);
    }
}
