using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler   // 각각 드래그 하기 전 눌렀을때, 드래그해서 옮겨다닐때 작동하는 인터페이스
{
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnDragHandler = null;

    public void OnPointerClick(PointerEventData eventData) // eventData는 포인터가 이벤트(클릭, 드래그 등)를 실행할 떄 포인터의 여러가지 정보들을 담고있음
    {                                                      // PointerEventData누르고 f12눌러서 확인해보면 겁나많음    
        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

    
}
