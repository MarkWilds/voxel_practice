using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlockData : ScriptableObject
{
    public enum Side
    {
        FRONT,
        BACK,
        LEFT,
        RIGHT,
        BOTTOM,
        TOP,
        ALL
    }

    public string name;

    public List<SideData> sideData;

    [Serializable]
    public struct SideData
    {
        public Side side;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;
        public Vector2 uv4;
    }
}