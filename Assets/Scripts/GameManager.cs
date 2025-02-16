using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public enum PlayerType
    {
        None,
        HostP,
        ClientP
    }
    public static GameManager Instance { get; private set; }
    private PlayerType currentPlayablePlayerType;
    private PlayerType localPlayerType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    // Which player index has the turn
    private NetworkVariable<int> currentTurnPlayer = new NetworkVariable<int>(0);

    private void Awake()
    {
        // Standard Singleton pattern
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        if(NetworkManager.Singleton.LocalClientId==0)
        {
            localPlayerType=PlayerType.HostP;
        }
        else
        {
            localPlayerType = PlayerType.ClientP;
        }
        if (IsServer)
        {
            currentPlayablePlayerType = PlayerType.HostP;
        }
    }

    //after play switch playertype
    public void changePlayerTurn(PlayerType playerType)
    {
        switch(currentPlayablePlayerType) {
            default:
                case PlayerType.HostP:
                    currentPlayablePlayerType=PlayerType.ClientP; break;
                case PlayerType.ClientP:
                    currentPlayablePlayerType = PlayerType.HostP; break;
        }
    }
    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType; 
    }
}
