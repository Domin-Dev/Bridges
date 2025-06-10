using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;

public class BridgeBuilder
{

    Vector3 startPosition;
    GameObject start,end;
    List<BridgeBuilderCollider> colliders = new List<BridgeBuilderCollider>();

    public event Action<int> OnBridgeCompleted;


    int platformIndex;
    int bridgeLength = 100;
    Transform[] bridge;
    int[] bridgePlayerIndexes;

    public int platform { get { return platformIndex; } }   
    public int length { get { return bridgeLength; } }
    public GameObject startPoint { get { return start; } }
    public GameObject endPoint { get { return end; } }

    public void SetUp(Vector3 startPosition,Vector3 endPosition, int bridgeLength, int platformIndex)
    {
        this.platformIndex = platformIndex;
        this.bridgeLength = bridgeLength;
        this.startPosition = startPosition;
        this.bridge = new Transform[bridgeLength];
        this.bridgePlayerIndexes = new int[bridgeLength];
        for (int i = 0; i < bridgeLength; i++)
        {
            bridgePlayerIndexes[i] = -1;
        }
        start = new GameObject("Start");
        end = new GameObject("End");
        start.transform.position = startPosition - new Vector3(0,0,1);
        end.transform.position = endPosition + new Vector3(0, 0, 1);
    }
    private void Collision(BlockCollector collector, BridgeBuilderCollider builderCollider)
    {
        collector.TakeBlock();
        int previous = bridgePlayerIndexes[builderCollider.blockIndex];
        UpdateBridge(collector.playerIndexGet, builderCollider.blockIndex);
        builderCollider.blockIndex++;


        if ((previous != -1 && bridgePlayerIndexes[builderCollider.blockIndex-1] == bridgePlayerIndexes[builderCollider.blockIndex]))
        {
            colliders.Remove(builderCollider);
            builderCollider.SelfDestruction();
        }
        else if(builderCollider.blockIndex >= bridgeLength)
        {
            colliders.Remove(builderCollider);
            builderCollider.SelfDestruction();
            OnBridgeCompleted?.Invoke(collector.playerIndexGet);
        }
        else
            builderCollider.transform.position += new Vector3(0, 0.2f, 0.4f);
    }
    private void UpdateBridge(int playerIndex, int blockIndex)
    {
        if(blockIndex - 1 >= 0)
        {
           if(bridgePlayerIndexes[blockIndex] != -1 && blockIndex + 1 < bridgeLength &&
                (bridgePlayerIndexes[blockIndex + 1] == -1 || bridgePlayerIndexes[blockIndex] != bridgePlayerIndexes[blockIndex + 1]))
           {
                for (int i = colliders.Count - 1; i >= 0; i--)
                {
                    BridgeBuilderCollider collider = colliders[i];
                    if (collider.blockIndex == blockIndex + 1)
                    {
                        collider.SelfDestruction();
                        colliders.RemoveAt(i);
                    }
                }
           }
        }
        else
        { 
            GameManager.instance.CreateBridgeBuilder(startPosition,this,playerIndex,0);
        }


        if (bridge[blockIndex] == null)
        {
            Vector2 offset = GameManager.instance.bridgePartOffsetGet;
            Transform block = GameManager.instance.CreateBridgePart(startPosition + new Vector3(0, offset.y, offset.x) * blockIndex, playerIndex);
            bridge[blockIndex] = block;
        }
        else
        {
            Transform obj = bridge[blockIndex];
            obj.GetComponent<MeshRenderer>().material.color = GameManager.instance.GetColor(playerIndex);
        }
        bridgePlayerIndexes[blockIndex] = playerIndex;
    }
    public void AddCollider(BridgeBuilderCollider bridge)
    {
        colliders.Add(bridge);
        bridge.OnCollisionEnterEvent += Collision;
    }
}
