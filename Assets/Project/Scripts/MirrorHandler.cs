using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorHandler : MonoBehaviour
{
    public Transform playerCamera;
    public Transform mirrorCamera;
    public Transform mirrorPlane;

    void Update()
    {
        Vector3 localPlayer = mirrorCamera.InverseTransformPoint(playerCamera.position);
        transform.position = mirrorCamera.TransformPoint(new Vector3(localPlayer.x, localPlayer.y, -localPlayer.z));

        Vector3 lookAtMirror = mirrorCamera.TransformPoint(new Vector3(-localPlayer.x, localPlayer.y, localPlayer.z));
        transform.LookAt(lookAtMirror);
    }
}
