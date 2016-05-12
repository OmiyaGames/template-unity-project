using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames
{
    public class WebsiteButton : MonoBehaviour
    {
        [SerializeField]
        Button button;
        [SerializeField]
        Text label;

        string redirectTo;

        public Button ButtonComponent
        {
            get
            {
                return button;
            }
        }

        public string DisplayedText
        {
            get
            {
                return label.text;
            }
            set
            {
                label.text = value;
            }
        }

        public string RedirectTo
        {
            get
            {
                return redirectTo;
            }
            set
            {
                redirectTo = value;
            }
        }

        public void OnClick()
        {
            Application.OpenURL(RedirectTo);
        }
    }
}
