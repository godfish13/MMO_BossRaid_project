using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Layer
    {
        Monster = 6,
        Ground = 7,
        Block = 8,
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

    public enum CameraMode
    {
        QuaterView,
    }
}
