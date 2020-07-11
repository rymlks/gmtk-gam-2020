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
        Vector3 targetPos = target.transform.position + offset;
        targetPos.y = Mathf.Max(targetPos.y, 0.0f);
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.2f);
    }
}
