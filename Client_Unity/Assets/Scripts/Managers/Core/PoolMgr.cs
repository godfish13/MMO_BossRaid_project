using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PoolMgr
{
    #region Pool
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        Stack<Poolable> poolStack = new Stack<Poolable>();

        public void init(GameObject original, int PoolCount = 5)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Root";
            
            for (int i = 0; i < PoolCount; i++)
            {
                Push(Create());
            }
        }

        Poolable Create()   // pooling하려는 프리팹에 혹시 Poolable 없으면 달아줌(기본적으로 pooling할 객체는 Poolabel을 달아준 prefab으로 사용하지만
        {                   // inspecter내에서 조작시 가끔 이런경우가 생길수도 있으므로 GetOrAddComponent 사용
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;
            return go.GetOrAddComponent<Poolable>();
        }

        public void Push(Poolable poolable)     // Poolable 오브젝트 비활성화 후 스택에 저장
        {
            if (poolable == null)
                return;

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.isUsing = false;
            poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent)
        {
            Poolable poolable;
            if (poolStack.Count > 0)
                poolable = poolStack.Pop();
            else
                poolable = Create();      // Pool에 오브젝트 부족하면 더만들어줌

            poolable.gameObject.SetActive(true);

            if (parent == null)  // DontDestroyOnLoad 해제를 위해 따로 그런 기능은 없으므로 타 오브젝트를 parent로 잠시 지정            
                poolable.transform.parent = Managers.sceneMgrEx.CurrentScene.transform;
            
            poolable.transform.parent = parent;
            poolable.isUsing = true;
            return poolable;
        }
    }
    #endregion


    Dictionary<string, Pool> poolDictionary = new Dictionary<string, Pool>();
    Transform _root;

    public void init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    public void CreatePool(GameObject original, int PoolCount = 5)
    {
        Pool pool = new Pool();
        pool.init(original, PoolCount);
        pool.Root.parent = _root;

        poolDictionary.Add(original.name, pool);
    }

    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if (poolDictionary.ContainsKey(name) == false)   // 혹시 오브젝트 풀이 미생성된 상태에서 오브젝트를 풀에 넣으려하면 넣지말고 오브젝트 Destroy
        {
            Debug.Log("no Key contains");
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        poolDictionary[name].Push(poolable);
    }

    public Poolable Pop(GameObject original, Transform parent = null, int PoolCount = 5)
    {
        if (poolDictionary.ContainsKey(original.name) == false)
            CreatePool(original, PoolCount);
        return poolDictionary[original.name].Pop(parent);
    }

    public GameObject GetOriginal(string name)
    {
        if (poolDictionary.ContainsKey(name) == false)
            return null;

        return poolDictionary[name].Original;
    }

    public void Clear()
    {
        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);

        poolDictionary.Clear();
    }
}