using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int playerIndex = -1;
    private int x, y;
    private BlockSpawner spawner;

    public void SetUp(int playerIndex,int x, int y,BlockSpawner spawner)
    {
        this.playerIndex = playerIndex; 
        this.x = x;
        this.y = y;
        this.spawner = spawner; 
    }

    public void BlockCollected()
    {
        spawner.BlockCollected(x,y,playerIndex);    
    }
}
