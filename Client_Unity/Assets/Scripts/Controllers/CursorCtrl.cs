using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorCtrl : MonoBehaviour
{
    Texture2D AttackCursor;
    Texture2D HandCursor;

    enum CursorType
    {
        None,
        Hand,
        Attack,
    }
    CursorType _cursorType = CursorType.None;

    int GroundMonsterLayerMask = (1 << (int)Define.Layer.Monster) | (1 << (int)Define.Layer.Ground);

    void Start()
    {
        AttackCursor = Managers.resourceMgr.Load<Texture2D>("Textures/Cursors/Attack");
        HandCursor = Managers.resourceMgr.Load<Texture2D>("Textures/Cursors/Hand");
    }

    void Update()
    {
        if (Input.GetMouseButton(0))    // 마우스 클릭중에는 커서 변화 x
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, GroundMonsterLayerMask))
        {
            if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType != CursorType.Attack)
                {
                    Cursor.SetCursor(AttackCursor, new Vector2(AttackCursor.width / 5, 0), CursorMode.Auto);
                    _cursorType = CursorType.Attack;    // SetCursor(texture, hotspot, cursorMode) hotspot : 마우스 포인터가 클릭되는 위치 (0,0) == 왼쪽위 구석
                }                                                               // cursorMode : auto == 하드웨어에 따라 최적화 걍 auto쓰자
            }
            else
            {
                if (_cursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(HandCursor, new Vector2(HandCursor.width / 3, 0), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }
            }
        }
    }
}
