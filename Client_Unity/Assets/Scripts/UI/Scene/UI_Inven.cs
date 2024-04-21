using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class UI_Inven : UI_Scene
{
    enum GameObjects
    {
        GridPanel,
    }

    void Start()
    {
        init();
    }

    public override void init()
    {
        base.init();

        Bind<GameObject>(typeof(GameObjects));

        GameObject gridPanel = Get<GameObject>((int)GameObjects.GridPanel);
        foreach(Transform child in gridPanel.transform)     // 완성 형태만 보려고 미리 구현해둔 아이콘들 정리(삭제)
            Managers.resourceMgr.Destroy(child.gameObject);

        for(int i = 0; i < 8; i++)
        { 
            GameObject item = Managers.UIMgr.MakeSubItem<UI_Inven_Item>(parent: gridPanel.transform).gameObject;    //  ~~~: ~~~라는 매개변수라고 표시
            UI_Inven_Item InvenItem = item.GetOrAddComponent<UI_Inven_Item>();
            InvenItem.Setname($"테스트 {i}");
        }
    }
}
