using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    public static GameController instance;
    public NetworkVariable<int> numberOfPlayers=new NetworkVariable<int>();
    public NetworkVariable<int> numberOfPlayersAlive=new NetworkVariable<int>();
    public NetworkVariable<MapLogic> mapLogic = new NetworkVariable<MapLogic>();


    zoneColors[] zoneColors;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            if (IsServer)
            {
                numberOfPlayers.Value++;
                numberOfPlayersAlive.Value++;
                mapLogic.Value.SetMap(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0.5f, 10, 0, 5);
                Debug.Log("Called on server");
                for (ulong i = 0; i < (ulong)NetworkManager.Singleton.ConnectedClients.Count; i++)
                {

                    NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerStatsController>().zoneAsigned.Value = (zoneColors)i;
                }
            }
            if (IsClient && IsOwner)
            {
                OnPlayerEnterServerRpc();
                SetMapLogicClientServerRpc(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0.5f, 10, 0, 5);
                SetNumberOfPlayerListServerRpc(clientId);
            }

        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
        {
            if (IsServer)
            {
                numberOfPlayers.Value--;
                numberOfPlayersAlive.Value--;
                mapLogic.Value.SetMap(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0.5f, 10, 0, 5);
                for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
                {
                    NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerStatsController>().zoneAsigned.Value = (zoneColors)i;
                }
            }
            else if (IsClient && IsOwner)
            {
                OnPlayerOutServerRpc();
                SetMapLogicClientServerRpc(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0.5f, 10, 0, 5);
                SetNumberOfPlayerListServerRpc(clientId);

            }

        };
    }
    
    public override void OnNetworkSpawn()
    {



    }

    void Start()
    {
        
 

        


    }

    void Update()
    {

    }

    public void CreatePlayerZones()
    {
        zoneColors = new zoneColors[numberOfPlayers.Value];
        for (int i = 0; i < zoneColors.Length; i++)
        {
            zoneColors[i] = (zoneColors)i;
        }
    }
    #region ServerRpc

    [ServerRpc]
    public void SetMapLogicClientServerRpc(int numberOfPlayers,int numberOfPlayersAlive,float zoneRadiusExpandSpeed,int totalTime,float enemiesSpawnRate,float zoneRadius)
    {
        Debug.Log("Called on client");
        mapLogic.Value.SetMap(numberOfPlayers, numberOfPlayersAlive, zoneRadiusExpandSpeed, totalTime, enemiesSpawnRate, zoneRadius);
    }
    [ServerRpc]
    public void SetNumberOfPlayerListServerRpc(ulong clientId) {

        for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerStatsController>().zoneAsigned.Value = (zoneColors)i;
        }
    }
    [ServerRpc]
    public void SetTimeLeftOnClientsServerRpc(ulong clientId)
    {
        for (ulong i = 0; i < (ulong)NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            GameObject player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject;
            player.GetComponentInChildren<Canvas>().GetComponentInChildren<TextMeshProUGUI>().text = "Player" + i+1;
        }

    }
    [ServerRpc]
    public void OnPlayerEnterServerRpc()
    {
          numberOfPlayersAlive.Value++;
          numberOfPlayers.Value++;
    }
    [ServerRpc]
    public void OnPlayerOutServerRpc()
    {
        numberOfPlayersAlive.Value--;
        numberOfPlayers.Value--;
    }
    #endregion

}

public enum zoneColors
{
    red,
    blue,
    green,
    yellow,
    purple,
    orange,
    pink,
    brown,
}

