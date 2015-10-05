using UnityEngine;
using System.Collections;

namespace Enums
{
    public enum KeyState : byte
    {
        Still = 0x0,            //00000000  0
        Runing = 0x1,           //00000001  1
        WalkBack = 0x2,         //00000010  2
        RuningBack = 0x3,       //00000011  3
        WalkStrafeLeft = 0x5,   //00000101  5
        WalkStrafeRight = 0x6,  //00000110  6
        RunStrafeLeft = 0x10,   //00001010  10
        RunStrafeRight = 0x11,  //00001011  11
        Left = 0x4,             //00000100  4
        Right = 0x8,            //00001000  8
        Jump = 0x10,            //00010000  16
        Free1 = 0x20,           //00100000  32
        Free2 = 0x40,           //01000000  64
        NoChange = 0x80,        //10000000  128
        Vertical = 0x3,         //00000011  3
        Horizontal = 0xC,       //00001100  12
        Walking = 0xF,          //00001111  15


    }
}
