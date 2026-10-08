using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionPanel : NetworkBehaviour
{
    [SerializeField] private TMP_InputField roomInputField;

    [SerializeField] private NetworkRunner runnerPrefab;

    // --- ACCIÓN DEL HOST EN EL ESPEJO ---
    public void MakeRoomPublicOrPrivate()
    {
        if (Runner == null || !Runner.IsServer) return;

        string roomName = roomInputField != null ? roomInputField.text.Trim() : "";

        Runner.SessionInfo.IsOpen = true;

        if (string.IsNullOrEmpty(roomName))
        {
            // Abierto para cualquier jugador aleatorio
            Runner.SessionInfo.IsVisible = true;
            Debug.Log("Espejo activado: Sala PÚBLICA (Matchmaking aleatorio).");
        }
        else
        {
            // Oculto, solo accesible con el nombre
            Runner.SessionInfo.IsVisible = false;
            Debug.Log($"Espejo activado: Sala PRIVADA con clave: {Runner.SessionInfo.Name}.");
        }

        //mirrorCanvas.SetActive(false); // Cerrar UI del espejo
    }

     //--- ACCIÓN DEL INVITADO EN EL ESPEJO ---
    public async void JoinWorldAsCoop()
    {
        if (Runner != null)
            await Runner.Shutdown();

        SceneManager.LoadScene("Game 1");
    }

}
