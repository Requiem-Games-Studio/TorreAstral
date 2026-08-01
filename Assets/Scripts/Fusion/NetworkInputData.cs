using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 movement;
    public NetworkButtons buttons;
}

public enum InputButtons
{
    Jump,
    Attack,
    Block,
    Run,
}