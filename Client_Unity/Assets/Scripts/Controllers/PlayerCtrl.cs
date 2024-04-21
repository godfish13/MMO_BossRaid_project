using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    //GameObject (Player) 부착

    [SerializeField] float M_speed = 10.0f;
    Vector3 MouseClickDestination;

    public enum PlayerStatue
    {
        Die,
        Moving,
        Idle,
    }
    [SerializeField] PlayerStatue _Statue = PlayerStatue.Idle;

    private void Start()
    {
        /*Managers.inputMgr.KeyAction -= Move; // 다른부분에서 Move가 연동되있을시 액션이벤트가 여러번 발생하는 버그방지를 위해 초기화
        Managers.inputMgr.KeyAction += Move; // inputMgr의 KeyAction에 Move 연동*/ // 본작에서는 일단 마우스 조종만 구현

        Managers.inputMgr.MouseAction -= OnMouseClicked;
        Managers.inputMgr.MouseAction += OnMouseClicked;
    }

    void Update()
    {
        switch (_Statue)
        {
            case PlayerStatue.Die:
                UpdateDie();
                break;
            case PlayerStatue.Idle:
                UpdateIdle();
                break;
            case PlayerStatue.Moving:
                UpdateMoving();
                break;
        }    
    }

    void UpdateDie()
    {
        // 아무것도 못하는 사망상태
    }

    void UpdateMoving()
    {
        Vector3 dir = MouseClickDestination - transform.position;
        if (dir.magnitude < 0.0001f)
        {
            _Statue = PlayerStatue.Idle;
        }
        else
        {
            float MoveDist = Mathf.Clamp(M_speed * Time.deltaTime, 0, dir.magnitude);
            // Clamp(float value, min, max) : value값이 min과 max 범위 이외의 값을 넘지 않도록 함 -> 정확히 이동할 거리 계산
            transform.position += dir.normalized * MoveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }

        // 이하 애니메이션 처리
        Animator Anim = GetComponent<Animator>();
        // 현재 게임 상태에 대한 정보를 넘겨줌
        Anim.SetFloat("Speed", M_speed);
    }

    /*void OnRunEvent()     // AnimationEvent 실험
    {
        Debug.Log("뚜벅뚜벅")
    }*/

    void UpdateIdle()
    {

        // 이하 애니메이션 처리
        Animator Anim = GetComponent<Animator>();
        Anim.SetFloat("Speed", 0);
    }

 /*   void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.1f);
            transform.position += Vector3.forward * Time.deltaTime * M_speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.1f);
            transform.position += Vector3.back * Time.deltaTime * M_speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.1f);
            transform.position += Vector3.left * Time.deltaTime * M_speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.1f);
            transform.position += Vector3.right * Time.deltaTime * M_speed;
        }
    }*/

    void OnMouseClicked(Define.MouseEvent evt)
    {
        /*if (evt != Define.MouseEvent.Click)     // 마우스 클릭하고 뗏을때만 작동하도록 함, inputMgr의 Mouse클릭 이벤트 액션 호출 참조
            return;*/

        if (_Statue == PlayerStatue.Die)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        LayerMask mask = LayerMask.GetMask("Wall");
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, mask))
        {
            MouseClickDestination = hit.point;
            _Statue = PlayerStatue.Moving;
            //Debug.Log($"RayCast Camera : {hit.collider.gameObject.name}");
        }
    }
}
