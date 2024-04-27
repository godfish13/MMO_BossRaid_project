using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    [SerializeField] private GameObject Player = null;
    public float transdormYoffset = 4.6f;
    public float transformZ = -10.0f;

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + transdormYoffset, transformZ);
    }

}
