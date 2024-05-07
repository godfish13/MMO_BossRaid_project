using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UI_HpbarBase : UI_Scene
{
    enum enum_Image // UI로 사용할 버튼/텍스트/게임오브젝트 등의 이름을 동일하게 선언해두고 유니티 엔진 내각 항목과 자동으로 연동하고 사용하기 위한 enum
    {
        BlackBorder_Sprite,
        RedBar_Sprite,
    }

    private void Start()
    {
        init();
    }

    public override void init()      // UI_PopUp에 가상함수로 선언해둔 init()
    {
        base.init();
        Managers.UIMgr.SetCanvas(gameObject, true);

        Bind<Image>(typeof(enum_Image));
    }

    public void HpbarChange(float value)
    {
        GetImage((int)enum_Image.RedBar_Sprite).fillAmount = value;
    }
}

