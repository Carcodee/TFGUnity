    using System;
    using System.Collections;
using System.Collections.Generic;
    using System.Threading.Tasks;
    using Michsky.UI.ModernUIPack;
    using Unity.Netcode;
    using Unity.Services.Lobbies;
    using Unity.Services.Lobbies.Models;
    using Unity.Services.Relay;
    using Unity.Services.Relay.Models;
    using UnityEngine;
    using UnityEngine.UI;

    public class MainMenuController : MonoBehaviour
    {
        public ModalWindowManager modalWindowTabs;
        public CustomDropdown customDropdown;
        public ModalWindowTabs tabs;
        public NetworkSceneManager networkSceneManager;
        public Transform lobbyList;
        public LobbyItem lobbyPrefab;
        
        public void OpenModalWindow()
        {
            modalWindowTabs.OpenWindow();
        }
        public async void LoadAllLobbies()
        {

            var options = new QueryLobbiesOptions();
            options.Count = 10;
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"
                ),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0"
                )
            };
            var lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            for (int i = 1; i < lobbyList.childCount; i++)
            {
                Destroy(lobbyList.GetChild(i).gameObject);
            }

            for (int i = 0; i < lobbies.Results.Count; i++)
            {
                Transform item = Instantiate(lobbyPrefab.GetComponent<Transform>(), lobbyList);
                item.GetComponent<LobbyItem>().Initialise(this, lobbies.Results[i]);
                item.GetComponent<LobbyItem>().lobbyName.text = lobbies.Results[i].Data["LobbyName"].Value;

            }

            Debug.Log("Lobbies: " + lobbies.Results.Count);

        }

        public async void JoinAsync(Lobby lobby)
        {
            try
            {
                var joinLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobby.Id);
                string joinCode = joinLobby.Data["JoinCode"].Value;
                JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCode);

                networkSceneManager.GetTransport().SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port,
                    a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
                NetworkManager.Singleton.StartClient();

            }
            catch
            {

            }
        }
    }