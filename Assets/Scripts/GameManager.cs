using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public event EventHandler OnGameStarted{

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
