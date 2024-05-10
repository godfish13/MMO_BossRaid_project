using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBite : DragonPattern
{
    private void Awake()
    {
        PatternId = (int)Define.ProjectileType.DragonBite;
    }

    protected override void Init()
    {
        // none
    }

    protected override void Update()
    {
        // none
    }

    protected override void UpdateAnim()
    {
        // none
    }
}
