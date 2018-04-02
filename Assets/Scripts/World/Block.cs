using UnityEngine;

public class Block
{
    public string typeId { get; private set; }
    public Vector3 Position { get; private set; }
    public bool IsSolid { get; private set; }

    public Block(Vector3 pos, bool solid, string type)
    {
        typeId = type;
        Position = pos;
        IsSolid = solid;
    }
}
