using System;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public Vector3 Position { get; private set; }
    public bool IsSolid { get; set; }

    public Block(Vector3 pos, bool solid)
    {
        Position = pos;
        IsSolid = solid;
    }
}
