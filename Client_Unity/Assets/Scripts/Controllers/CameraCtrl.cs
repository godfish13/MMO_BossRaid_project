using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    [SerializeField] private Define.CameraMode _mode = Define.CameraMode.QuaterView;      //Define에 선언해둔 CameraMode
    [SerializeField] private Vector3 _delta = new Vector3(0, 0, 0); // 카메라가 캐릭터 바라보는 방향
    [SerializeField] private GameObject Player = null;

    void Start()
    {
        SetQuaterView(new Vector3(0, 6, -5));
    }

    void LateUpdate()
    {
        if (_mode == Define.CameraMode.QuaterView)
        {
            RaycastHit hit;
            if(Physics.Raycast(Player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("Wall")))
            {
                float dist = (hit.point - Player.transform.position).magnitude * 0.8f;
                transform.position = Player.transform.position + _delta.normalized * dist;
            }
            else
            {
                transform.position = Player.transform.position + _delta;
                transform.LookAt(Player.transform);
            }
        }
    }

    public void SetQuaterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuaterView;
        _delta = delta;
    }
}
