using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Mathematics;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private float blockDensity = 0.5f;
    [SerializeField] private List<int> players;
    [SerializeField] private int playerCount = 5;

    public List<int2> freeslots = new List<int2>();
    private float tickInterval = 0.3f;
    private float tickTimer;
    private Vector3 topLeft;


    int[] counters;
    int perPlayer;

    private void Awake()
    {
        Destroy(GetComponent<MeshFilter>());
        Destroy(GetComponent<MeshRenderer>());
    }
    private void Start()
    {
        SpawnBlocks(); 
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer -= tickInterval;
            CreateNewBlocks();
        }
    }


    private void CreateNewBlocks()
    {
        if(freeslots.Count > 0)
        {
            int playerIndex = GetNextIndex();
            if (playerIndex != -1)
            {
                int slot = UnityEngine.Random.Range(0, freeslots.Count);
                int2 pos = freeslots[slot];
                freeslots.RemoveAt(slot);

                if (playerIndex != -1) SpawnBlock(playerIndex, pos.x, pos.y, topLeft);
            }
        }
    }
    private void SpawnBlocks()
    {
        topLeft = transform.position - (transform.localScale / 2f) + new Vector3(blockDensity, 0, blockDensity);
        int x = (int)(transform.localScale.x / blockDensity);
        int y = (int)(transform.localScale.z / blockDensity);

        int perPlayer = (x * y) / playerCount;
        counters = new int[playerCount];
        for (int i = 0; i < counters.Length; i++)
        {
            counters[i] = perPlayer;
        }

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                int playerIndex = GetNextIndex();
                if (playerIndex != -1) SpawnBlock(playerIndex, j, i, topLeft);
                else
                {
                    freeslots.Add(new int2(j, i));
                }
            }
        }
    }
    private void SpawnBlock(int playerIndex, int x, int y, Vector3 pos)
    {
        GameObject obj = Instantiate(GameManager.instance.block, pos + new Vector3(x * blockDensity, 0, y * blockDensity), Quaternion.identity,GameManager.instance.map);
        obj.GetComponent<MeshRenderer>().material.color = GameManager.instance.GetColor(playerIndex);
        obj.GetComponent<Block>().SetUp(playerIndex, x, y, this);
    }
    private int GetNextIndex()
    {
        if(players.Count == 0) return -1;   

        int i = UnityEngine.Random.Range(0, players.Count);
        int playerIndex = players[i];
        if(counters[playerIndex] > 0)
        {
            counters[playerIndex]--;
            return playerIndex;
        }
        else
        {
            for (int k = 0; k < players.Count; k++)
            {
                playerIndex = players[k];

                if (counters[playerIndex] > 0)
                {
                    counters[playerIndex]--;
                    return playerIndex;
                }
            }
            return -1;
        }
    }
    public void BlockCollected(int x, int y,int playerIndex)
    {
        counters[playerIndex]++;
        freeslots.Add(new int2(x, y));
    }
    public void AddPlayer(int playerIndex)
    {
        if(!players.Contains(playerIndex))
            players.Add(playerIndex);
    }
}
