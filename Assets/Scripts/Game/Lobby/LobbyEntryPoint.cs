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
            roomManager.Initialize(lobbyPresenter);
            var authenticator = roomManager.authenticator as ClientAuthenticator;
            authenticator.Initialize(GameState.Instance.LobbyState);
            lobbyUI.Initialize(lobbyPresenter);
            DontDestroyOnLoad(lobbyUI);

            lobbyPresenter.GotoConnection();
        }
    }
}