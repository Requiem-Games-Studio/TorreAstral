using UnityEngine;

public class SecretArea : MonoBehaviour
{
    public SpriteRenderer[] spriteElements;
    float alphaValue = 1f;

    public float disappearRate = 1f;
    bool playerEntered = false;

    public bool toggleWall = false;
    public bool reverse;

    private void Update()
    {
        if (!reverse)
        {
            if (playerEntered)
            {
                alphaValue -= Time.deltaTime * disappearRate;

                if (alphaValue <= 0)
                    alphaValue = 0;

                foreach (SpriteRenderer spriteItem in spriteElements)
                {
                    spriteItem.color = new Color(spriteItem.color.r, spriteItem.color.g, spriteItem.color.b, alphaValue);
                }
            }
            else
            {
                alphaValue += Time.deltaTime * disappearRate;

                if (alphaValue >= 1)
                    alphaValue = 1;

                foreach (SpriteRenderer spriteItem in spriteElements)
                {
                    spriteItem.color = new Color(spriteItem.color.r, spriteItem.color.g, spriteItem.color.b, alphaValue);
                }
            }
        }
        else
        {
            if (!playerEntered)
            {
                alphaValue -= Time.deltaTime * disappearRate;

                if (alphaValue <= 0)
                {
                    alphaValue = 0;
                }
                foreach (SpriteRenderer spriteItem in spriteElements)
                {
                    spriteItem.color = new Color(spriteItem.color.r, spriteItem.color.g, spriteItem.color.b, alphaValue);
                }
            }
            else
            {
                alphaValue += Time.deltaTime * disappearRate;

                if (alphaValue >= 1)
                {
                    alphaValue = 1;
                }
                foreach (SpriteRenderer spriteItem in spriteElements)
                {
                    spriteItem.color = new Color(spriteItem.color.r, spriteItem.color.g, spriteItem.color.b, alphaValue);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerEntered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && toggleWall)
        {
            playerEntered = false;
        }
    }
}
