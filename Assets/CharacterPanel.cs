using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class CharacterPanel : MonoBehaviour
{
    
    public SaveManager saveManager;

    public GameObject characterPanel;

    public int slot;

    [SerializeField] private NetworkRunner runnerPrefab;
    [SerializeField] private string gameSceneName = "Game"; // Nombre de tu escena de juego


    public void CreateNewCharacter(int slotIndex)
    {
        slot = slotIndex;
        characterPanel.SetActive(true);
    }
    
    
    public void UpdateNameString(string name)
    {
        saveManager.initialData.playerName = name;

        StartNewGame();
    }


    void StartNewGame()
    {
        Debug.Log("Slot vacío, creando nueva partida...");
        SaveManager.Instance.NewGame(slot);
        //SceneManager.LoadScene("Game");
        StartHostPrivate();
    }

    public async void StartHostPrivate()
    {
        // 1. Instanciar o buscar el NetworkRunner
        NetworkRunner runner = FindFirstObjectByType<NetworkRunner>();
        if (runner == null)
        {
            runner = Instantiate(runnerPrefab);
        }

        // 2. Habilitar el procesamiento de callbacks en este Runner
        runner.ProvideInput = true;

        // 3. Iniciar el Host pasándole el índice o nombre de la escena de juego
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = "MiRoomPrivada",
            IsOpen = false,       // Nadie puede unirse aún
            IsVisible = false,    // No aparece en listas de matchmaking
            Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(gameSceneName)), // Fusion se encarga de cargar la escena
            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            Debug.Log("Host iniciado exitosamente en modo privado.");
        }
        else
        {
            Debug.LogError($"Error al iniciar Host: {result.ShutdownReason}");
        }
    }
}
