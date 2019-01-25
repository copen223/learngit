using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    // Block参数
    public GameObject currentBlock; // 当前Block
    public Transform blockPos;      // 中心位置
    public Vector2 blockSize;      // 尺寸
    public Vector2 tileSize;       // 网格尺寸
    public int blockNum;           // Block编号
    private int tileNum;           // tile数量
    public Vector2 edgeSize;
    public BlockData blockData;    // 当前Block数据
    

    // Tile预设
    public GameObject[] obstacleTilePrefabs;       // 障碍物
    public GameObject[] normalTilePrefabs;

    public enum TileType
    {
        NONE = -1,
        NORMAL = 0,
        OBSTACLE = 1
    }

    // 缓存
    public List<GameObject> tiles;
    public List<Vector2> tilePositions;        // 可用网格位置
    private List<bool> isActivePos;             // 网格状态
    private Dictionary<Vector2, GameObject> tileOscar;

    private void Awake()
    {
        
    }

    // 初始化
    public void Init(int _blockNum)
    {
        blockNum = _blockNum;
        blockSize = blockData.blockSizes[blockNum];
        tileNum = (int)(blockSize.x * blockSize.y);

        // 初始化缓存
        tiles = new List<GameObject>();
        tilePositions = new List<Vector2>();
        InitTilePositions();
        tileOscar = new Dictionary<Vector2, GameObject>();

        isActivePos = new List<bool>();
        for (int i = 0; i < tileNum; i++)
        {
            isActivePos.Add(true);
        }

        CreatTilesByData(blockData.block[blockNum], blockData.blockSizes[blockNum]);
        //CreateTilesAtVoid(TileType.NORMAL);
        //ChangeTileAtPos(TileType.OBSTACLE, new Vector2(3, 4));
    }


    // 随机选择预组
    private GameObject ChoosePrefab(GameObject[]prefabs,int num)
    {
        if(num < -1 || num >= prefabs.Length)
        {
            Debug.Log("超出预设数组范围");
            return null;
        }

        if(num == -1)
        {
            num = Random.Range(0, prefabs.Length - 1);
        }

        return prefabs[num]; 
    }

    // 选择网格位置
    private Vector2 TilePosBySize(Vector2 startPos,Vector2 _tileSize,Vector2 tilePos)
    {
        Vector2 realTilePos;

        realTilePos = startPos + _tileSize * tilePos;

        return realTilePos;
    }

    // 初始化可用Tile位置
    private void InitTilePositions()
    {
        Vector2 currentTilePos;
        for(int j = (int)blockSize.y - 1; j >=0 ;j--)
        {
            for(int i=0; i < blockSize.x ;i++)
            {
                currentTilePos = (Vector2)blockPos.position - new Vector2((blockSize.x * tileSize.x)/2,(blockSize.y * tileSize.y)/2) + new Vector2(i * tileSize.x, j * tileSize.y);
                tilePositions.Add(currentTilePos);
            }
        }
    }

    // 初始化
    private void InitBlockManager(Vector2 _blockSize,Vector2 _tileSize,Vector2 _edgeSize)
    {
        this.blockSize = _blockSize;
        this.tileSize = _tileSize;
        this.edgeSize = _edgeSize;
    }

    // 生成Tile
    private GameObject CreateNewTileAtPos(TileType tileType, Vector2 tilePos)
    {
        
        int listIndex = (int)((blockSize.y - tilePos.x) * blockSize.x + (tilePos.y - 1));

        do
        {
            if (listIndex > tilePositions.Count)
                break;

            if (tileType == TileType.NONE)
                break;

            if (!isActivePos[listIndex])
                break;

            if (tileType == TileType.NORMAL)
            {
                GameObject newTile;
                newTile = Instantiate(ChoosePrefab(normalTilePrefabs, -1),
                    tilePositions[listIndex],
                    Quaternion.identity,currentBlock.transform);

                isActivePos[listIndex] = false;

                newTile.name = "NT" + tilePos.x + tilePos.y;

                tiles.Add(newTile);

                tileOscar.Add(tilePositions[listIndex], newTile);

                return newTile;
            }
            if(tileType == TileType.OBSTACLE)
            {
                GameObject newTile;
                newTile = Instantiate(ChoosePrefab(obstacleTilePrefabs, -1),
                    tilePositions[listIndex],
                    Quaternion.identity, currentBlock.transform);

                newTile.name = "OT" + tilePos.x + tilePos.y;

                isActivePos[listIndex] = false;

                tiles.Add(newTile);

                tileOscar.Add(tilePositions[listIndex], newTile);

                return newTile;
            }

        }
        while (false);
        Debug.Log("生成错误1");
        return null;  
    }

    private GameObject CreateNewTileAtPos(TileType tileType, Vector2 tilePos,int num)
    {

        int listIndex = (int)((blockSize.y - tilePos.x) * blockSize.x + (tilePos.y - 1));

        do
        {
            if (listIndex > tilePositions.Count)
                break;

            if (tileType == TileType.NONE)
                break;

            if (!isActivePos[listIndex])
                break;

            if (tileType == TileType.NORMAL)
            {
                GameObject newTile;
                newTile = Instantiate(ChoosePrefab(normalTilePrefabs, num),
                    tilePositions[listIndex],
                    Quaternion.identity, currentBlock.transform);

                newTile.name = "NT" + tilePos.x + tilePos.y;

                isActivePos[listIndex] = false;

                tiles.Add(newTile);

                tileOscar.Add(tilePositions[listIndex], newTile);

                return newTile;
            }
            if (tileType == TileType.OBSTACLE)
            {
                GameObject newTile;
                newTile = Instantiate(ChoosePrefab(obstacleTilePrefabs, num),
                    tilePositions[listIndex],
                    Quaternion.identity, currentBlock.transform);

                newTile.name = "OT" + tilePos.x + tilePos.y;

                isActivePos[listIndex] = false;

                tiles.Add(newTile);

                tileOscar.Add(tilePositions[listIndex], newTile);

                return newTile;
            }

        }
        while (false);
        Debug.Log("生成错误2");
        return null;
    }

    private void CreateTilesAtVoid(TileType tileType)
    {
        for(int i = 0;i<tilePositions.Count;i++)
        {
            do
            {
                if (!isActivePos[i])
                    break;

                if (tileType == TileType.NONE)
                    break;

                if (tileType == TileType.NORMAL)
                {
                    GameObject newTile;
                    newTile = Instantiate(ChoosePrefab(normalTilePrefabs, -1),
                        tilePositions[i],
                        Quaternion.identity, currentBlock.transform);

                    isActivePos[i] = false;

                    tileOscar.Add(tilePositions[i], newTile);

                    tiles.Add(newTile);
                }
                if (tileType == TileType.OBSTACLE)
                {
                    GameObject newTile;
                    newTile = Instantiate(ChoosePrefab(obstacleTilePrefabs, -1),
                        tilePositions[i],
                        Quaternion.identity, currentBlock.transform);

                    isActivePos[i] = false;

                    tileOscar.Add(tilePositions[i], newTile);

                    tiles.Add(newTile);
                }

            }
            while (false);
        }



    }

    //替换Tile
    private GameObject ChangeTileAtPos(TileType tileType, Vector2 tilePos)
    {
        int listIndex = (int)((tilePos.x - 1) * blockSize.x + (tilePos.y - 1));

        do
        {
            if (listIndex > tilePositions.Count)
                break;

            if (tileType == TileType.NONE)
                break;

            if (isActivePos[listIndex])
                break;

            if (tileType == TileType.NORMAL)
            {
                GameObject newTile;
                newTile = Instantiate(ChoosePrefab(normalTilePrefabs, -1),
                    tilePositions[listIndex],
                    Quaternion.identity, currentBlock.transform);

                newTile.name = "NT" + tilePos.x + tilePos.y;

                isActivePos[listIndex] = false;

                GameObject.Destroy(tiles[listIndex]);

                tiles.RemoveAt(listIndex);
                
                tiles.Insert(listIndex, newTile);

                return newTile;
            }
            if (tileType == TileType.OBSTACLE)
            {
                GameObject newTile;
                newTile = Instantiate(ChoosePrefab(obstacleTilePrefabs, -1),
                    tilePositions[listIndex],
                    Quaternion.identity, currentBlock.transform);

                isActivePos[listIndex] = false;

                Debug.Log(listIndex);
                newTile.name = "OT" + tilePos.x + tilePos.y;

                isActivePos[listIndex] = false;

                GameObject.Destroy(tiles[listIndex]);

                tiles.RemoveAt(listIndex);

                tiles.Insert(listIndex, newTile);

                return newTile;
            }

        }
        while (false);
        Debug.Log("替换失败");
        return null;
    }
    


    public void CreatTilesByData(string paramas,Vector2 _blockSize)
    {
        blockSize = _blockSize;
        Vector2 tilePos;
        
        char []tiles = paramas.Replace("\n", "").Replace(" ", "").Replace("\n", "").Replace(" ", "").ToCharArray();
        

        for (int i = 0,j = 0;i<tiles.Length; i++)
        {
            if(i%(blockSize.x+1) == 0)
            {
                continue;
            }
            tilePos = new Vector2(blockSize.y + 1 -(j / (int)_blockSize.x + 1),j % (int)_blockSize.x + 1);
            Debug.Log(tilePos);
            Debug.Log(tiles[i]+"g");
            if(tiles[i] == 'a')
            {
                CreateNewTileAtPos(TileType.NORMAL, tilePos);
            }
            else if(tiles[i] == 'b')
            {
                CreateNewTileAtPos(TileType.OBSTACLE, tilePos);
            }
            j++;
        }
    }
}
