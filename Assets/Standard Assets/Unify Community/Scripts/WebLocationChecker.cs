using UnityEngine;
using System.Collections;
using System.Text;

/// <summary>
/// Add this script to an object in the first scene of your game.
/// It doesn't do anything for non-webplayer builds. For webplayer
/// builds, it checks the domain to make sure it contains at least
/// one of the strings, or it will redirect the page to the proper
/// URL for the game.
/// </summary>
/// <remarks>
/// Code by andyman from Github:
/// https://gist.github.com/andyman/e58dea85cce23cccecff
/// </remarks>
public class WebLocationChecker : MonoBehaviour
{
    ///<summary>
    /// If it is a webplayer, then the domain must contain any
    /// one or more of these strings, or it will be redirected
    ///</summary>
    [SerializeField]
    string[] domainMustContain;

    ///<summary>
    /// This is where to redirect the webplayer page if none of
    /// the strings in domainMustContain are found.
    ///</summary>
    [SerializeField]
    string redirectURL;

#if UNITY_WEBPLAYER || UNITY_WEBGL
    void Awake()
    {
		if (domainMustContain.Length > 0)
		{
			StringBuilder buf = new StringBuilder();

			for(int index = 0; index < domainMustContain.Length; index++)
			{
				string domain = domainMustContain[index];

				if (index > 0)
				{
					buf.Append(" && ");
				}
				buf.Append("(document.location.host.indexOf('"+domain+"') == -1)");
			}
			string criteria = buf.ToString();
			Application.ExternalEval("if("+criteria+") { document.location='"+redirectURL+"'; }");
		}
    }
#endif
}

