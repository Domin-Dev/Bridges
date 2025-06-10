using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Vector2 bridgePartOffset = new Vector2(0.4f, 0.2f);
    [Space]
    [SerializeField] private GameObject fencePrefab;
    [SerializeField] private GameObject bridgePrefab;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject bridgePartPrefab;
    [SerializeField] private List<Color> playerColors;
    [SerializeField] private Color grayColor;

    [SerializeField] private Transform  mapParent;
    public Vector2 bridgePartOffsetGet { get { return bridgePartOffset; } }
    public GameObject fence { get { return fencePrefab; } }
    public GameObject block { get { return blockPrefab; } }
    public GameObject bridge { get { return bridgePrefab; } }
    public GameObject bridgePart { get { return bridgePartPrefab; } }
    public Transform map { get { return mapParent; } }
    public static GameManager instance
    {
        private set; get;
    }
    List<BridgeBuilder> bridges = new List<BridgeBuilder>();
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public Color GetColor(int playerID)
    {
        if (playerID >= 0 && playerID < playerColors.Count) return playerColors[playerID];
        else return grayColor;
    }
    public BridgeBuilder CreateBridgeBuilder(Vector3 position,float bridgeLength, int platformIndex)
    {
        BridgeBuilder bridgeBuilder = new BridgeBuilder();

        float a = bridgePartOffset.y / bridgePartOffset.x;
        float b = 1;
        a *= a;
        b *= b;
        float length = MathF.Sqrt((bridgeLength* bridgeLength) / (a + b));

        bridgeBuilder.SetUp(position,position + new Vector3(0, length * (bridgePartOffset.y / bridgePartOffset.x), length), 1 + (int)Math.Round(length/bridgePart.transform.localScale.z),platformIndex);  
        bridges.Add(bridgeBuilder);

        Transform bridge = Instantiate(GameManager.instance.bridge, position, Quaternion.identity,map).transform;
        bridgeBuilder.AddCollider(bridge.GetComponent<BridgeBuilderCollider>());
        return bridgeBuilder;
    }
    public void CreateBridgeBuilder(Vector3 position, BridgeBuilder bridgeBuilder,int playerIndex,int blockIndex)
    {
        Transform bridge = Instantiate(GameManager.instance.bridge, position, Quaternion.identity, map).transform;
        BridgeBuilderCollider bridgeBuilderCollider = bridge.GetComponent<BridgeBuilderCollider>();
        bridgeBuilderCollider.SetUp(playerIndex, blockIndex);
        bridgeBuilder.AddCollider(bridgeBuilderCollider);

    }
    public Transform CreateBridgePart(Vector3 position,int playerIndex)
    {
        GameObject obj = Instantiate(bridgePart, position,Quaternion.identity,map);
        obj.GetComponent<MeshRenderer>().material.color = GetColor(playerIndex);
        return obj.transform;
    }
    public BridgeBuilder GetBridgeBuilder(int platformIndex)
    {
        List<BridgeBuilder> bridgeBuilders = new List<BridgeBuilder>(); 
        for (int i = 0; i < bridges.Count; i++)
        {
            if (bridges[i].platform == platformIndex)
            {
                bridgeBuilders.Add(bridges[i]);
            }
        }
        return bridgeBuilders[UnityEngine.Random.Range(0, bridgeBuilders.Count)];
    }
}
