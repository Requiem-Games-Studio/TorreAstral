using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class Lever : MonoBehaviour
{

    public GameObject lever,platform,pointUP,pointDown;
    
    public Animator anim;
    public bool up;

    public float speed = 2f;
    private Coroutine moveCoroutine;

    public Vector3 upPosition,downPosition;

    private void Start()
    {
        upPosition = pointUP.transform.position;
        downPosition = pointDown.transform.position;
    }


    public void SwitchLever()
    {
        up = !up;
        anim.SetBool("Up", up);

        UpdatePlatform();
    }

    public void UpdatePlatform()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        Vector3 targetPosition = up ? upPosition : downPosition;

        moveCoroutine = StartCoroutine(MovePlatform(targetPosition));
    }

    private IEnumerator MovePlatform(Vector3 targetPosition)
    {
        while (Vector3.Distance(platform.transform.position, targetPosition) > 0.01f)
        {
            platform.transform.position = Vector3.MoveTowards(
                platform.transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            yield return null;
        }

        platform.transform.position = targetPosition;
        moveCoroutine = null;
    }

}
