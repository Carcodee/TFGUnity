    using System;
    using System.Collections;
using System.Collections.Generic;
    using System.Threading.Tasks;
    using Michsky.UI.ModernUIPack;
    using Unity.Netcode;
    using Unity.Services.Lobbies;
    using Unity.Services.Lobbies.Models;
    using Unity.Services.Relay.Models;
    using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public ModalWindowManager modalWindowTabs;
    public CustomDropdown customDropdown;
    public ModalWindowTabs tabs;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
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
           var lobbies= await Lobbies.Instance.QueryLobbiesAsync(options);
           
           for (int i = 1; i < customDropdown.dropdownItems.Count; i++)
           {
               customDropdown.dropdownItems.RemoveAt(i);
           }
           
           for (int i = 1; i < lobbies.Results.Count; i++)
           {
               customDropdown.dropdownItems.Add(new CustomDropdown.Item {itemName = lobbies.Results[i].Data["LobbyName"].Value});
           }
           Debug.Log("Lobbies: "+lobbies.Results.Count);

   }

   public async void JoinAsync(Lobby lobby)
   {
       try
       {
           var joinLobby= await Lobbies.Instance.JoinLobbyByCodeAsync(lobby.Id);
           string joingCode= joinLobby.Data["JoinCode"].Value;

           NetworkManager.Singleton.StartClient();
           
       }
       catch
       {

       }
   }

   public async Task FetchLobbies()
   {
       
   }

}
    public struct lobbyInfo
    {
        public string lobbyName;
        public int lobbyPlayers;
        public int lobbyMaxPlayers;
        public string lobbyRegion;
    }