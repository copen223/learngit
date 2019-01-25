using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;

    // 模板
    public GameObject blockPrefab;

    public GameObject blockParent;

    // Block缓存
    public List<GameObject> Blocks;
    public BlockData[] blockDatas;          // 
    public int blockDataIndex;              // block文件随大关改变而变

    // Block位置缓存
    public List<Vector2> blockPositions;
    

    // 随机
    private int RandomIndex<T>(List<T> ts)
    {
        return Random.Range(0, ts.Count);
    }
    private int RandomIndex<T>(T[] a)
    {
        return Random.Range(0, a.Length-1);
    }

    // 生成Block
   public GameObject CreateNewBlockAtPos(int blockNum,Vector2 blockPos)
    {
        GameObject currentBlock = Instantiate(blockPrefab, blockPos, Quaternion.identity, blockParent.transform);

        currentBlock.GetComponent<TileManager>().blockData = blockDatas[blockDataIndex];
        currentBlock.name = "Block" + blockNum;

        currentBlock.SendMessage("Init",blockNum);
        return currentBlock;
    }

    // 计算Block生成位置
    private void GetNextBlockPos()
    {

    }

    // 生成所有Block
    public void CreatAllBlock()
    {
        for(int i =0;i <= blockDatas[blockDataIndex].block.Length;i++)
        {
            CreateNewBlockAtPos(i, blockPositions[i]);
            Debug.LogWarning(i);
        }
    }

    // 生成n个相连的Block
    public void CreatNewBlocks(int n)
    {
        for(int i = 0;i < n;i++)
        {
            CreateNewBlockAtPos(RandomIndex(blockDatas[blockDataIndex].block), blockPositions[RandomIndex(blockPositions)]);
        }
    }

    private void Start()
    {
        Init();
        CreatAllBlock();
    }

    public void Init()
    {
        Blocks = new List<GameObject>();
        blockPositions = new List<Vector2>();
        blockPositions.Add(new Vector2(0, 0));
        blockPositions.Add(new Vector2(6, 7));
    }
}
