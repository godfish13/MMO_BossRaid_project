using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum SkillId
    {
        Human_Slash = 1,
        Human_ThrowBomb = 2,
        Elf_ArrowShot = 3,
        Elf_Knife = 4,
        Furry_Slash = 5,
        Furry_Guard = 6,
        Lizard_FireBall = 7,
        Lizaed_Heal = 8,
    }

    public enum ProjectileType
    {
        Human_Bomb = 1,
        Elf_Arrow = 2,
        Elf_ArrowHit = 3,
        Lizard_FireBall = 4,
        Lizaed_Heal = 5,
        Dragon_Bite = 11,
        Dragon_Burn = 12,
        Dragon_Fireball = 13,
        Dragon_Thunder = 14,
        Dragon_FireballExplosion = 15,
    }

    public enum MonsterPatternType
    {
        Melee = 1,
        Range = 2,
    }

    public enum Layer
    {
        Platform = 6,
        Player = 8,
        Monster = 11,
        MonsterSkill = 12,
        MonsterProjectile = 13,
    }

    public enum Scene
    {
        UnKnown,    // default
        LogIn,
        Lobby,      // Select character
        InGame,
    }

    public enum Sound
    {
        Bgm,
        EffectSound,
        MaxCount,           // Sound ���� ����(���� Bgm, EffectSound 2�� => Sound enum�� ���� ������ ���� MaxCount�� �� int���� 2�̹Ƿ� Sound ���� ������ ǥ������)
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Press,
        Click,
    }
}
