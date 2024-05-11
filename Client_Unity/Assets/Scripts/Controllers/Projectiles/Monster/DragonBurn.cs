using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBurn : DragonPattern
{
    private void Awake()
    {
        PatternId = (int)Define.ProjectileType.Dragon_Burn;
        PatternType = (int)Define.MonsterPatternType.Melee;
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
