using Game.Arena.Character;
using Game.Lobby.Services;
using Mirror;

namespace Game.Arena
{
    public class ArenaManager : NetworkBehaviour
    {
        private RoomManager _roomManager;
        private CharacterFactory _factory;

        public void Initialize(RoomManager roomManager, CharacterFactory factory)
        {
            _factory = factory;
            _roomManager = roomManager;
        }

        public void SetupPlayer(CharacterContainer container)
        {
            _factory.SetupCharacter(container);
        }
    }
}