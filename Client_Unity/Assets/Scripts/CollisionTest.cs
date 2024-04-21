using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        //Debug.Log(Input.mousePosition);
        //Debug.Log(Camera.main.ScreenToViewportPoint(Input.mousePosition));  //viewPort 좌표

        /*if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Vector3 dir = (mousePos - Camera.main.transform.position).normalized;                                      // 카메라 ClipPlane의 near 값

            Debug.DrawRay(Camera.main.transform.position, dir * 100.0f, Color.red, 1.0f);
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, dir, out hit, 100.0f))
            {
                Debug.Log($"RayCast Camera : {hit.collider.gameObject.name}");
            }
        }*/     // 아래것을 풀어서 설명한 버전 ray = mousePos + dir

        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

            //int mask = (1 << 6) | (1 << 8);      // Monster를 6번레이어로 해놨으므로 6번째 비트플래그로 써주면 몬스터만 인식됨
                                                 //연산자 | 한개인거 주의! 두개쓰는건 왼쪽이 참/거짓일 경우 뒤 조건을 연산하지 않음 : 조건을 따질때 쓰는 문법
                                                 //비트플래그에서는 값을 연산하려고 쓰는것이므로 조건을 따지는 두개가아닌 한개로 쓰는것
            LayerMask mask = LayerMask.GetMask("Monster") | LayerMask.GetMask("Wall");  // 근데 그냥 비트플래그 안쓰고 LayerMask 써도 됨
            RaycastHit hit;                     
            if (Physics.Raycast(ray, out hit, 100.0f, mask))
            {
                Debug.Log($"RayCast Camera : {hit.collider.gameObject.name}");
            }
        }
    }
}
