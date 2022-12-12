using UnityEngine;

namespace Game.Lobby.View
{
    public class LobbyUI : MonoBehaviour
    {
        private LobbyPresenter _presenter;

        private GameObject ConnectionForm;

        public void Initialize(LobbyPresenter presenter)
        {
            _presenter = presenter;
        }

        public void ShowConnectionForm()
        {
            ConnectionForm.SetActive(true);
        }
    }
}