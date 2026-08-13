using Fusion;
using Fusion.Sockets;
using Photon.Client.StructWrapping;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FusionInput : MonoBehaviour, INetworkRunnerCallbacks
{
    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new();

        data.movement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"));

        // Solo el estado actual
        data.buttons.Set(InputButtons.Jump, Input.GetKey(KeyCode.Space));
        data.buttons.Set(InputButtons.Attack, Input.GetMouseButton(0));
        data.buttons.Set(InputButtons.Block, Input.GetMouseButton(1));
        data.buttons.Set(InputButtons.Run, Input.GetKey(KeyCode.LeftShift));
        data.buttons.Set(InputButtons.Interact, Input.GetKey(KeyCode.E));

        input.Set(data);

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ReadOnlySpan<byte> data)
    {
        
    }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    // Resto de callbacks...
}