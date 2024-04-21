using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabTest : MonoBehaviour
{
    GameObject prefab;
    GameObject Tank;
    
    void Start()
    {
        Tank = Managers.resourceMgr.Instantiate("Tank");
        
        Managers.Destroy(Tank, 3.0f);
    }
}
