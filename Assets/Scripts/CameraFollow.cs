using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;              // Referencia al jugador
    public float smoothSpeed = 5f;        // Velocidad de seguimiento
    public float cursorInfluence = 2f;    // Intensidad del efecto del cursor
    public Vector2 screenThreshold = new Vector2(0.3f, 0.3f); // Área que activa el desplazamiento

    [Header("Camera Bounds")]
    public Collider2D cameraBounds;       // Collider que define los límites

    private Vector3 offset;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Posición base de la cámara
        Vector3 desiredPosition = player.position + offset;

        // Influencia del cursor
        Vector3 mousePos = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 cursorOffset = new Vector2(
            (mousePos.x - screenCenter.x) / screenCenter.x,
            (mousePos.y - screenCenter.y) / screenCenter.y
        );

        if (Mathf.Abs(cursorOffset.x) > screenThreshold.x || Mathf.Abs(cursorOffset.y) > screenThreshold.y)
        {
            desiredPosition += new Vector3(cursorOffset.x, cursorOffset.y, 0) * cursorInfluence;
        }

        // Aplicar bounds si existen
        if (cameraBounds != null)
        {
            Bounds bounds = cameraBounds.bounds;

            float camHeight = cam.orthographicSize * 2f;
            float camWidth = camHeight * cam.aspect;

            float minX = bounds.min.x + camWidth / 2f;
            float maxX = bounds.max.x - camWidth / 2f;
            float minY = bounds.min.y + camHeight / 2f;
            float maxY = bounds.max.y - camHeight / 2f;

            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        // Movimiento suave
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

    }
}
