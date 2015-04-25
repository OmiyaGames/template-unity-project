using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class SplashPanel : MonoBehaviour
{
    const string VisibleField = "Visible";

    [SerializeField]
    bool fadeoutSplashOnStart = true;

	// Use this for initialization
	IEnumerator Start ()
    {
        yield return null;
        GetComponent<Animator>().SetBool(VisibleField, false);
	}
}
