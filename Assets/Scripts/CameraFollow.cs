using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public float smoothSpeed = 5f; // Velocidad de seguimiento
    public float cursorInfluence = 2f; // Intensidad del efecto del cursor
    public Vector2 screenThreshold = new Vector2(0.3f, 0.3f); // Área cerca de los bordes que activa el desplazamiento

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;

        // Obtener la posición del cursor en relación con la pantalla (normalizada de -1 a 1)
        Vector3 mousePos = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 cursorOffset = new Vector2(
            (mousePos.x - screenCenter.x) / screenCenter.x,
            (mousePos.y - screenCenter.y) / screenCenter.y
        );

        // Aplicar influencia solo si el cursor está cerca de los bordes
        if (Mathf.Abs(cursorOffset.x) > screenThreshold.x || Mathf.Abs(cursorOffset.y) > screenThreshold.y)
        {
            desiredPosition += new Vector3(cursorOffset.x, cursorOffset.y, 0) * cursorInfluence;
        }

        // Movimiento suave
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
