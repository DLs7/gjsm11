using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TileAutomata : MonoBehaviour
{

    [Range(0, 100)]
    public int iniChance;

    [Range(0, 100)]
    public int treeChance;

    [Range(1,8)]
    public int birthLimit;

    [Range(1,8)]
    public int deathLimit;

    [Range(1,10)]
    public int numR;

    private int count = 0;

    private int[,] terrainMap;
    public Vector2Int tmapSize;

    public Tilemap topMap;
    public Tilemap botMap;
    public Tilemap treeMap;
    public Tilemap botTree;
    public Tile topTile1;
    public Tile topTile2;
    public Tile botTile;
    public Tile treeTile1;
    public Tile treeTile2;
    public Tile bushTile1;
    public Tile bushTile2;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject SnowPile;

    private float nextSpawnTime;

    int width;
    int height;

    public void doSim(int numR)
    {
        clearMap(false);
        width = tmapSize.x;
        height = tmapSize.y;

        if(terrainMap == null)
        {
            terrainMap = new int[width, height];
            initPos();
        }

        for(int i = 0; i < numR; i++)
        {
            terrainMap = genTilePos(terrainMap);
        }

        bool done1 = false;
        bool done2 = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (terrainMap[x, y] == 1 || x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    if (Random.Range(0, 100) < 50)
                    {
                        if(y == height - 1)
                            botTree.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), treeTile1);
                        else if(x == 0 || y == 0 || x == width - 1)
                            treeMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), treeTile1);
                        else
                            treeMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), bushTile1);
                    } 
                    else
                    {
                        if (y == height - 1)
                            botTree.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), treeTile2);
                        else if (x == 0 || y == 0 || x == width - 1)
                            treeMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), treeTile2);
                        else
                            treeMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), bushTile2);
                    }
                    botMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), botTile);
                    terrainMap[x, y] = 1;
                }
                else
                {
                    if (Random.Range(0, 100) < 50)
                    {
                        topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), topTile1);
                    }
                    else
                    {
                        topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), topTile2);
                    } 
                    if (Random.Range(0, 100) < 5)
                    {
                        Instantiate(SnowPile, new Vector3((-x + width / 2) + 0.5f, (-y + height / 2), 2), Quaternion.identity);
                    }
                }
            }
        }

        while (!done1 && !done2)
        {
            for (int x = width * 1 / 4; x < width * 3 / 4; x++)
            {
                for (int y = height * 1 / 4; y < height * 3 / 4; y++)
                {
                    if (terrainMap[x, y] == 0)
                    {
                        if (!done2)
                        {
                            //coloca p1
                            if (Random.Range(0, 100) < 25)
                            {
                                Player2.transform.position = new Vector3((-x + width / 2) + 0.5f, (-y + height / 2), 0);
                                done2 = true;
                            }
                        }
                        else
                        {
                            //coloca p2
                            if (Random.Range(0, 100) < 25)
                            {
                                Player1.transform.position = new Vector3((-x + width / 2) + 0.5f, (-y + height / 2), 0);
                                done1 = true;
                            }
                        }
                    }
                }
            }
        }
    }

    public int [,] genTilePos(int[,] oldMap)
    {
        int[,] newMap = new int[width, height];
        int neighb;
        BoundsInt myB = new BoundsInt(-1, -1, 0, 3, 3, 1);

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++ )
            {
                neighb = 0;

                foreach(var b in myB.allPositionsWithin)
                {
                    if (b.x == 0 && b.y == 0) continue;
                    if(x + b.x >= 0 && x + b.x < width && y + b.y >=0 && y + b.y < height)
                    {
                        neighb += oldMap[x + b.x, y + b.y];
                    } else
                    {
                        neighb++;
                    }
                }

                if (oldMap[x, y] == 1)
                {
                    if (neighb < deathLimit) newMap[x, y] = 0;
                    else
                    {
                        newMap[x, y] = 1;
                    }
                }

                if (oldMap[x, y] == 0)
                {
                    if (neighb > birthLimit) newMap[x, y] = 1;
                    else
                    {
                        newMap[x, y] = 0;
                    }
                }
            }
        }

        return newMap;
    }

    public void initPos()
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                terrainMap[x, y] = Random.Range(1, 101) < iniChance ? 1 : 0;
            }
        }
    }

    private void Start()
    {
        nextSpawnTime = 10.0f;
        doSim(numR);
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.timeSinceLevelLoad > nextSpawnTime)
        {
            //do stuff here (like instantiate)
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (terrainMap[x, y] == 0)
                    {
                        if (Random.Range(0, 100) < 5)
                        {
                            Instantiate(SnowPile, new Vector3((-x + width / 2) + 0.5f, (-y + height / 2), 2), Quaternion.identity);
                        }
                    }
                }
            }

            //increment next_spawn_time
            nextSpawnTime += 10.0f;
        }
    }

    public void clearMap(bool complete)
    {
        topMap.ClearAllTiles();
        botMap.ClearAllTiles();
        treeMap.ClearAllTiles();
        botTree.ClearAllTiles();

        if (complete)
        {
            terrainMap = null;
        }
    }
}
