using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class MirrorPortal : NetworkBehaviour
{

    public Canvas canvas;


    private void Start()
    {
        if (Runner.IsServer && Runner.IsPlayer)
        {
            Debug.Log("Eres el HOST (Servidor + Jugador).");
            canvas.worldCamera = GameObject.FindGameObjectWithTag("PlayerCamera")?.GetComponent<Camera>();
        }
    }
}
