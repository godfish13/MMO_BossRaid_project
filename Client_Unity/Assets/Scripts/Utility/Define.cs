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
        Lizard_FireBall = 3,
        Lizaed_Heal = 4,
        DragonFireball = 5,
        DragonThunder = 6,
    }

    public enum Layer
    {
        Platform = 6,
        Player = 8,
        Monster = 11,
        MonsterSkill = 12,
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
        MaxCount,           // Sound 종류 갯수(현재 Bgm, EffectSound 2개 => Sound enum의 제일 마지막 값인 MaxCount의 현 int값이 2이므로 Sound 종류 갯수를 표시해줌)
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
