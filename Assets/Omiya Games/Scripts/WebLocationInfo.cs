using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

namespace OmiyaGames
{
    public class WebLocationInfo : MonoBehaviour
    {
        const string ForWebGlMessage = "This menu is meant to provide information for WebGL builds.";
        const string LoadingMessage = "Loading web information...";

        [SerializeField]
        Text infoLabel;

        // Use this for initialization
        IEnumerator Start()
        {
            // Grab the web checker
            WebLocationChecker webChecker = null;
            if (Singleton.Instance.IsWebplayer == true)
            {
                webChecker = Singleton.Get<WebLocationChecker>();
            }

            // Grab information about webChecker
            if(webChecker != null)
            {
                // Print that we're loading
                infoLabel.text = LoadingMessage;

                // Wait until the WebLocationChecker is done
                while(webChecker.CurrentState == WebLocationChecker.State.InProgress)
                {
                    yield return null;
                }

                // Update the reason for this dialog to appear
                infoLabel.text = Utility.BuildTestMessage(new StringBuilder(), webChecker);
            }
            else
            {
                infoLabel.text = ForWebGlMessage;
            }
        }
    }
}
