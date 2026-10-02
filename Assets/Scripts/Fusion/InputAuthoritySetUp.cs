using Fusion;
using UnityEngine;

public class InputAuthoritySetUp : NetworkBehaviour
{
    [SerializeField] private GameObject playerCamera;

    public PauseMenu pauseMenu;

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

        CheckPlayerRole();
    }

    public void CheckPlayerRole()
    {
        // 1. Single Player
        if (Runner.GameMode == GameMode.Single)
        {
            Debug.Log("Estás jugando en Modo Single Player.");
            pauseMenu.playerMode = "Single";
            return;
        }

        // 2. Host (Servidor que también actúa como Jugador)
        if (Runner.IsServer && Runner.IsPlayer)
        {
            Debug.Log("Eres el HOST (Servidor + Jugador).");
            pauseMenu.playerMode = "Host";
        }
        // 3. Servidor Dedicado (Servidor sin jugador local)
        else if (Runner.IsServer && !Runner.IsPlayer)
        {
            Debug.Log("Eres un Servidor Dedicado.");
        }
        // 4. Cliente
        else if (Runner.IsClient)
        {
            Debug.Log("Eres un CLIENTE.");
            pauseMenu.playerMode = "Client";
        }
    }
}
