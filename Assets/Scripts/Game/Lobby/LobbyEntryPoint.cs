using Game.Lobby.Services;
using Game.Lobby.View;
using Game.State;
using UnityEngine;

namespace Game.Lobby
{
    public class LobbyEntryPoint : MonoBehaviour
    {
        public RoomManager RoomManagerPrefab;
        public LobbyUI LobbyUIPrefab;

        void Start()
        {
            var roomManager = Instantiate(RoomManagerPrefab);
            var lobbyUI = Instantiate(LobbyUIPrefab);
            var lobbyPresenter = new LobbyPresenter(roomManager, lobbyUI, GameState.Instance);
            lobbyUI.Initialize(lobbyPresenter);

            lobbyPresenter.GotoConnection();
        }
    }
}