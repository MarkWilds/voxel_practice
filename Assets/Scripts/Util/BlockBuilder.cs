using System.Collections.Generic;
using UnityEngine;

public class BlockBuilder : MonoBehaviour
{
    [SerializeField] private List<BlockData> blockData;

    private Dictionary<string, BlockData> blockDataHash;

    public BlockBuilder()
    {
        blockDataHash = new Dictionary<string, BlockData>();
    }

    private void Awake()
    {
        foreach (BlockData data in blockData)
        {
            blockDataHash[data.name] = data;
        }
    }

    public BlockData getBlock(string name)
    {
        return blockDataHash[name];
    }
}