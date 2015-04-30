using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour
{
    [SerializeField]
    Vector3 angularDirection = Vector3.one;

    Vector3 eularAngles = Vector3.zero;

    void Start()
    {
        eularAngles = transform.localRotation.eulerAngles;
    }

	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(angularDirection * Time.deltaTime);
	}
}
