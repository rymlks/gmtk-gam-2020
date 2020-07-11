using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = target.transform.position + offset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, 0.2f);
    }
}
