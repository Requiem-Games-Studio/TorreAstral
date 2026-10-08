using Fusion;
using UnityEngine;

public class InputAuthoritySetUp : NetworkBehaviour
{
    [SerializeField] private GameObject playerCamera;

    public override void Spawned()
    {
        // HasInputAuthority devuelve true SOLO para el jugador local que controla este objeto
        if (HasInputAuthority)
        {
            playerCamera.SetActive(true);
        }
        else
        {
            // Para los demás jugadores en mi pantalla, desactivo su cámara
            playerCamera.SetActive(false);
        }
    }
}
