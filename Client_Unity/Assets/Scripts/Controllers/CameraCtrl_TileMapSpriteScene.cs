using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CameraCtrl_TileMapSpriteScene : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;

    float horizontal;
    float vertical;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            vertical = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            vertical = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontal = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            horizontal = -1;
        }

        if (Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.S) == false)
            vertical = 0;
        if (Input.GetKey(KeyCode.D) == false && Input.GetKey(KeyCode.A) == false)
            horizontal = 0;

        Move(horizontal, vertical);
    }

    void Move(float horizontal, float vertical)
    {
        Vector2 movedir = (Vector2.right * horizontal) + (Vector2.up * vertical);
        GetComponent<Transform>().Translate(movedir.normalized * _speed * Time.deltaTime, Space.Self);
    }
}
