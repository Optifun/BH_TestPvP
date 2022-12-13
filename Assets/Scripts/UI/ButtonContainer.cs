using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ButtonContainer : MonoBehaviour
    {
        [field: SerializeField] public Button Button { get; private set; }
        [field: SerializeField] public Image Image { get; private set; }
        [field: SerializeField] public TMP_Text Text { get; private set; }

        private void Reset()
        {
            Button = GetComponent<Button>();
            Image = GetComponent<Image>();
            Text = GetComponentInChildren<TMP_Text>();
        }
    }
}