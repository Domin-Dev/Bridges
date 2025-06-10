using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BlockCollector : MonoBehaviour
{

    [SerializeField] private float blockOffset = 0.1f;
    [SerializeField] private Transform backpack;
    private int playerIndex;
    public int playerIndexGet { get { return playerIndex; } }
    List<Transform> blocks = new List<Transform>();
    List<Vector3> targets = new List<Vector3>();

    Dictionary<int, Transform> BlocksInBackpack = new Dictionary<int, Transform>();
    public int blockCounter { get; private set; }

    private void Start()
    {
        playerIndex = GetComponent<Player>().playerIndexGet;
        blockCounter = 0;
    }
    private void Update()
    {
        for (int i = blocks.Count - 1; i >= 0; i--)
        {
            Transform block = blocks[i];
            Vector3 taget = targets[i] + backpack.position;
            block.position = Vector3.Lerp(block.position, taget, Time.deltaTime * 14);

            if (Vector3.Distance(block.position, taget) < 0.4f)
            {
                block.gameObject.GetComponent<TrailRenderer>().enabled = false;
                block.parent = backpack;
                block.localPosition = targets[i];
                block.localRotation = Quaternion.Euler(0, 90, 0);
                blocks.RemoveAt(i);
                targets.RemoveAt(i);
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Block")
        { 
            Block block = other.GetComponent<Block>();  
            if(block != null &&  RightColor(block.playerIndex))
            {
                if(block.playerIndex == -1)
                    other.gameObject.GetComponent<MeshRenderer>().material.color = GameManager.instance.GetColor(playerIndex);


                other.gameObject.GetComponent<BoxCollider>().enabled = false;
                other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                if (block.playerIndex != -1) block.BlockCollected();
                other.gameObject.tag = "Untagged";
                other.gameObject.layer = 12;

                Destroy(other.gameObject.GetComponent<Block>());

                TrailRenderer trail = other.gameObject.GetComponent<TrailRenderer>();
                Color color = GameManager.instance.GetColor(playerIndex);
                trail.startColor = color;
                trail.endColor = color;

                blocks.Add(other.transform);
                targets.Add(new Vector3(0, blockOffset, 0) * blockCounter);
                BlocksInBackpack.Add(blockCounter, other.transform);
                blockCounter++;
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Player")
        {
            BlockCollector blockCollector = hit.gameObject.GetComponent<BlockCollector>();
            if (blockCounter >= blockCollector.blockCounter)
                Drop(blockCollector);
            else
                Drop(this);
        }
    }

    private void Drop(BlockCollector blockCollector)
    {
        blockCollector.blockCounter = 0;
        foreach (var item in blockCollector.BlocksInBackpack)
        {
            BlockDrop(item.Value);
        }

        for (int i = 0; i < blockCollector.blocks.Count; i++)
        {
            var item = blockCollector.blocks[i];
            item.parent = blockCollector.backpack;
            item.localPosition = blockCollector.targets[i];
            BlockDrop(item);
        }

        blockCollector.targets.Clear();
        blockCollector.blocks.Clear();
        blockCollector.BlocksInBackpack.Clear();
    }

    private void BlockDrop(Transform obj)
    {
        obj.GetComponent<BoxCollider>().enabled = true;
        obj.parent = GameManager.instance.map;
        Vector3 pos = transform.position;
        obj.parent = GameManager.instance.map;
        pos.y = math.abs(pos.y);
        transform.position = pos;

        obj.tag = "Block";
        obj.gameObject.layer = 11;
        obj.AddComponent<Cooldown>();
        obj.GetComponent<MeshRenderer>().material.color = GameManager.instance.GetColor(-1);
        Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        Vector3 randomDirection = UnityEngine.Random.onUnitSphere;
        rigidbody.AddForce(randomDirection * 1f, ForceMode.Impulse);
    }
    private bool RightColor(int playerIndex)
    {
        return playerIndex == this.playerIndex || playerIndex == -1;
    }
    public void TakeBlock()
    {
        Transform toRemove = BlocksInBackpack[blockCounter - 1];
        BlocksInBackpack.Remove(blockCounter - 1);
        if (blocks.Contains(toRemove))
        {
            int i = blocks.IndexOf(toRemove);
            blocks.RemoveAt(i);
            targets.RemoveAt(i);
        }
        Destroy(toRemove.gameObject);
        blockCounter--;
    }

    
}
