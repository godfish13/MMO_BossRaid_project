using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceMgr
{
    public T Load<T>(string path) where T : Object  // 오브젝트 관련 메소드들을 여러곳에서 산개하여 작성하면 어디서 무엇을 어떻게 쓰는지 찾기 점점 어려워짐
    {                                               // 따라서 추적하기 쉽게하기위해 ResourceMgr을 통하도록 Load, Destroy등 각 메소드들 Wrapping 해둠
        if(typeof(T) == typeof(GameObject))
        {                                           // T가 GameObject면 Pool내에 존재할 가능성이 있으므로 Pool내에서 검색 후 있으면 그것 사용
            string name = path;
            int index = name.LastIndexOf('/');      // path는 모든 경로명을 포함하므로 제일 마지막의 '/' 이후 글자만 이름으로 Pool에 있으므로 name 가공
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.poolMgr.GetOriginal(name);
            if (go != null)
                return go as T;
        }
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null) // 풀링 시, 기본 5개 생성하고 부족할 시 create를 통해 자동 추가되므로 따로 풀링갯수 지정 x
    {
        GameObject Original = Load<GameObject>($"Prefabs/{path}");
        if (Original == null)
        {
            Debug.Log($"Failed to Load prefab : {path}");
        }

        if (Original.GetComponent<Poolable>() != null)              // Poolable객체면 Pool에서 갖다 씀
        {
            Debug.Log($"Pooled {path}");
            return Managers.poolMgr.Pop(Original, parent).gameObject;
        }

        GameObject go = Object.Instantiate(Original, parent);   // Poolable 아니면 그냥 불러다씀
        go.name = Original.name;
        return go;
    }

    public void Destroy(GameObject go) 
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();    // Poolable이면 pool에 넣어둠
        if(poolable != null)
        {
            Managers.poolMgr.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }

    public void Destroy(GameObject go, float time)
    {
        if (go == null)
            return;
        
        // 만약 Pooling할 객체일 시 풀링 매니저에 위탁

        Object.Destroy(go, time);
    }
}
