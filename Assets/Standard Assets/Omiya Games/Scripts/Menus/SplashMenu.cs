using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    [RequireComponent(typeof(Animator))]
    public class SplashMenu : MonoBehaviour
    {
        const string VisibleField = "Visible";

        [SerializeField]
        bool fadeoutSplashOnStart = true;

        // Use this for initialization
        IEnumerator Start()
        {
            yield return null;
            GetComponent<Animator>().SetBool(VisibleField, false);
        }
    }
}
