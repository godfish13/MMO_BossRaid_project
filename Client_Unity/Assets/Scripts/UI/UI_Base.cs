using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour       // 상속용 기본 base이므로 abstract 클래스로 선언
{
    Dictionary<Type, UnityEngine.Object[]> D_objects = new Dictionary<Type, UnityEngine.Object[]>(); // Hashtable 일반화 버전

    public abstract void init();

    protected void Bind<T>(Type type) where T : UnityEngine.Object        // Button, Text등의 유니티엔진 내 UI와 이 스크립트에 선언한 UI이름이 동일하게 존재할 경우
    {                                                           // 해당 UI와 이 스크립트에 선언한 변수를 연동시켜줌
        string[] Names = Enum.GetNames(type);       // type enum내의 항목들 각각 string으로 이름변환해서 배열로 돌려줌

        UnityEngine.Object[] obj = new UnityEngine.Object[Names.Length];
        D_objects.Add(typeof(T), obj);

        for (int i = 0; i < Names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))     // GameObject는 Monobehavior, ... 을 상속받지 않은 것이라 따로 구현하라고 오류뜸 그러므로 일반화아닌버전 사용
                obj[i] = Utils.FindComponentinChild(gameObject, Names[i], true);           // Utils script에 만들어둔 하위 오브젝트 탐색 후 배열에 저장 메소드
            else
                obj[i] = Utils.FindComponentinChild<T>(gameObject, Names[i], true);

            if (obj[i] == null)
                Debug.Log($"Failed to Bind(Can't Find '{Names[i]}')");
        }
    }

    protected T Get<T>(int idx) where T : UnityEngine.Object       // Bind로 연동한 UI 내역 enum의 값을 int로 명시적 변환하여 호출할 수 있게해줌
    {
        UnityEngine.Object[] objects = null;
        if (D_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }

    // Text, Button, Image 갖다쓰기 편하게 Get_UI써서 긁어오는 메소드 만들어둠
    protected Text GetText(int idx) { return Get<Text>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }
    protected GameObject GetGameObject(int idx) { return Get<GameObject>(idx); }

    // UI에 마우스 관련 Event연동시키는 메소드
    public static void BindUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler evt = Utils.GetOrAddComponent<UI_EventHandler>(go);

        switch(type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
        }
    }
}
