using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectDataManager : MonoBehaviour
{
    [SerializeField] private NetworkRunner runnerPrefab;
    [SerializeField] private string gameSceneName = "Game"; // Nombre de tu escena de juego
    public string roomSala = "MiSala";

    //public async void StartHostPrivate()
    //{
        
    //    roomSala = SaveManager.Instance.currentData.playerName;
    //    // 1. Instanciar o buscar el NetworkRunner
    //    NetworkRunner runner = FindFirstObjectByType<NetworkRunner>();
    //    if (runner == null)
    //    {
    //        runner = Instantiate(runnerPrefab);
    //    }

    //    // 2. Habilitar el procesamiento de callbacks en este Runner
    //    runner.ProvideInput = true;

    //    // 3. Iniciar el Host pasándole el índice o nombre de la escena de juego
    //    var result = await runner.StartGame(new StartGameArgs()
    //    {
    //        GameMode = GameMode.Host,
    //        SessionName = roomSala,
    //        IsOpen = false,       // Nadie puede unirse aún
    //        IsVisible = false,    // No aparece en listas de matchmaking
    //        Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(gameSceneName)), // Fusion se encarga de cargar la escena
    //        SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
    //    });

    //    if (result.Ok)
    //    {
    //        Debug.Log("Host iniciado exitosamente en modo privado.");
    //    }
    //    else
    //    {
    //        Debug.LogError($"Error al iniciar Host: {result.ShutdownReason}");
    //    }
    //}

    public async void StartHostPrivate()
    {
        roomSala = SaveManager.Instance.currentData.playerName;

        // 1. Buscar si ya existe un Runner activo
        NetworkRunner runner = FindFirstObjectByType<NetworkRunner>();

        // Si existe pero no está corriendo o fue destruido, nos aseguramos de destruirlo
        if (runner != null && !runner.IsRunning)
        {
            Destroy(runner.gameObject);
            runner = null;
        }

        // Si no hay un Runner válido, instanciamos el Prefab
        if (runner == null)
        {
            runner = Instantiate(runnerPrefab);
        }

        // 2. Habilitar inputs
        runner.ProvideInput = true;

        // 3. Validar / Agregar el SceneManager sin duplicarlo
        var sceneManager = runner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        // 4. Validar el Build Index de la escena
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(gameSceneName);
        if (sceneIndex < 0)
        {
            Debug.LogError($"[Fusion] La escena '{gameSceneName}' no está agregada en Build Settings.");
            return;
        }

        // 5. Iniciar el Host en modo privado
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = roomSala,
            IsOpen = true,       // Nadie puede unirse aún
            IsVisible = true,    // No aparece en listas de matchmaking
            Scene = SceneRef.FromIndex(sceneIndex),
            SceneManager = sceneManager
        });

        if (result.Ok)
        {
            Debug.Log($"Host privado iniciado correctamente. Sala: {roomSala}");
        }
        else
        {
            Debug.LogError($"Error al iniciar Host: {result.ShutdownReason}");
            // Si falla, destruimos el runner creado para no dejar basura en la escena
            if (runner != null)
            {
                Destroy(runner.gameObject);
            }
        }
    }
}
