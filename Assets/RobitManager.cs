using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RobitManager : MonoBehaviour
{

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }
        else
        {
            string mode = NetworkManager.Singleton.IsHost ? "host" :
            NetworkManager.Singleton.IsServer ? "server" : "client";
            GUILayout.Label("Mode: " + mode);
        }

        GUILayout.EndArea();
    }
}
