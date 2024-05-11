using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBite : DragonPattern
{
    private void Awake()
    {
        PatternId = (int)Define.ProjectileType.Dragon_Bite;
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
