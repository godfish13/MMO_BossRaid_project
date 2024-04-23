using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    [SerializeField] private GameObject Player = null;

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + 4.6f, -10);
    }

}
