using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inven_Item : UI_Base
{
    enum GameObjects
    {
        ItemIcon_Image,
        ItemName_txt,
    }

    string _name = null;

    void Start()
    {
        init();
    }

    public override void init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.ItemName_txt).GetComponent<Text>().text = _name;
        Get<GameObject>((int)GameObjects.ItemIcon_Image).BindUIEvent((PointerEventData) => { Debug.Log($"테스트 {_name}가 클릭되었다!"); }) ;
    }

    public void Setname(string name)
    {
        _name = name;
    }
}
