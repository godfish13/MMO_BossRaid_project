using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UI_Btn : UI_PopUp   // Bind, Get~~ 등 기본 UI베이스 상속
{ 
    enum enum_Button        // UI로 사용할 버튼/텍스트/게임오브젝트 등의 이름을 동일하게 선언해두고 유니티 엔진 내각 항목과 자동으로 연동하고 사용하기 위한 enum
    {
        PointBtn,
    }

    enum enum_Text
    {
        PointTxt,
        ScoreTxt,
    }

    enum enum_Image
    {
        ItemIcon,
    }

    enum enum_GameObject
    {
        TestObj,
    }

    private void Start()
    {
        init();
    }

    public override void init()      // UI_PopUp에 가상함수로 선언해둔 init()
    {                
        base.init();
        Managers.UIMgr.SetCanvas(gameObject, true);

        Bind<Button>(typeof(enum_Button));
        Bind<Text>(typeof(enum_Text));
        Bind<Image>(typeof(enum_Image));
        Bind<GameObject>(typeof(enum_GameObject));

        GetText((int)enum_Text.PointTxt).text = "Testing";
        GetText((int)enum_Text.ScoreTxt).text = "Binding test";
        Debug.Log(GetGameObject((int)enum_GameObject.TestObj).name);

        GetButton((int)enum_Button.PointBtn).gameObject.BindUIEvent(OnBtnClicked);   // ExtensionMethod를 통해 쉽게 사용

        GameObject go = GetImage((int)enum_Image.ItemIcon).gameObject;
        BindUIEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag); // 람다형식으로 BindUIEvent
    }

    int score = 0;
    public void OnBtnClicked(PointerEventData data)
    {
        score++;
        Debug.Log("Btn Clicked");
        GetText((int)enum_Text.ScoreTxt).text = $"점수 : {score}";
    }
}
