using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Platform : MonoBehaviour
{
    [SerializeField] BlockSpawner blockSpawner;
    [SerializeField] int platformIndex;
    [Header("Fence Settings")]
    [SerializeField] float offsetY = 1f;
    [SerializeField] float thickness = 1f;
    [Header("Outputs Settings")]
    [SerializeField] int outputsNumber;
    [SerializeField] float outputWidth = 1f;
    [Header("Inputs Settings")]
    [SerializeField] int inputsNumber;
    [SerializeField] float inputWidth = 1f;
    [Header("Other settings")]
    [SerializeField] Vector2 bridgeDirection = new Vector2(2,1);
    [SerializeField] float bridgeLength = 20;

    public int index { get { return platformIndex; } }
    public BlockSpawner spawner { get { return blockSpawner; } }


    public Platform _nextPlatform;
    public Platform nextPlatform {
        get { return _nextPlatform; }
        set { if (_nextPlatform == null) _nextPlatform = value; } 
    }

    private void Awake()
    {
        Vector3 pos = transform.position;
        Vector3 offset = transform.localScale/2f;


        CreateAllBridges(pos + new Vector3(-offset.x, offsetY, offset.z),outputsNumber,outputWidth);

        CreateSegmentedFence(pos + new Vector3(-offset.x, offsetY, -offset.z), inputsNumber, inputWidth);
        CreateSegmentedFence(pos + new Vector3(-offset.x, offsetY,  offset.z), outputsNumber, outputWidth);

        CreateFence(pos + new Vector3(-offset.x, offsetY, -offset.z), thickness/ transform.localScale.x, Quaternion.Euler(0, -90f, 0));
        CreateFence(pos + new Vector3(offset.x, offsetY, offset.z), thickness / transform.localScale.x, Quaternion.Euler(0,90f,0));
    }
    private void CreateSegmentedFence(Vector3 startPos,int number, float size)
    {
        float gap = (transform.localScale.x - number * size) / (number + 1);
        float gapScale = gap / transform.localScale.x;

        for (int i = 0; i < number + 1; i++)
        {
            CreateFence(startPos, thickness / transform.localScale.z, Quaternion.identity, gapScale);
            startPos += new Vector3(gap + size, 0, 0);
        }
    }
    private void CreateAllBridges(Vector3 startPos, int number, float size)
    {
        float gap = (transform.localScale.x - number * size) / (number + 1);
        float gapScale = gap / transform.localScale.x;
        float angle = Vector2.Angle(bridgeDirection, Vector2.right);


        for (int i = 0; i < number; i++)
        {
            startPos += new Vector3(gap, 0, 0);
            CreateBridge(startPos,Quaternion.Euler(0,-90, angle), bridgeLength);
            startPos += new Vector3(size/2f, 0, 0);
            var builder = GameManager.instance.CreateBridgeBuilder(startPos - new Vector3(0,0.5f,0) * offsetY, bridgeLength,platformIndex);
            builder.OnBridgeCompleted += BridgeCompleted;
            startPos += new Vector3(size/2f, 0, 0);            
            CreateBridge(startPos,Quaternion.Euler(0,-90,angle), bridgeLength);
        }
    }
    private void BridgeCompleted(int playerIndex)
    {
        if (nextPlatform != null && nextPlatform.blockSpawner != null)
            nextPlatform.blockSpawner.AddPlayer(playerIndex);
        else 
            SceneManager.LoadScene(0);
    }
    private void CreateFence(Vector3 position,float thickness, Quaternion quaternion, float length = 1f)
    {
        Transform fence = Instantiate(GameManager.instance.fence, position, quaternion, transform).transform;
        Vector3 scale = fence.GetChild(0).transform.localScale;
        fence.localScale = new Vector3(length, 1, 1);
        fence.GetChild(0).transform.localScale = new Vector3(scale.x, scale.y, thickness);
    }
    private void CreateBridge(Vector3 position, Quaternion quaternion, float length)
    {
        Transform fence = Instantiate(GameManager.instance.fence, position, quaternion,GameManager.instance.map).transform;
        fence.localScale = new Vector3(length, 1, 1);
    }
    public Vector3 GetPositionNextPlatform(Transform nextPlatform)
    {
        float a = bridgeDirection.y / bridgeDirection.x;
        float b = 1;
        a *= a;
        b *= b;
        float posZ = MathF.Sqrt((bridgeLength * bridgeLength) / (a + b));


        float z = (transform.localScale.z + nextPlatform.localScale.z) / 2f;

        return transform.position + new Vector3(0, posZ * (bridgeDirection.y / bridgeDirection.x), posZ + z);
    }
}
