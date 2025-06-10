
using System.Net;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AIMovement : PlayerMovement
{
    Player player;
    public GameObject target;

    private BridgeBuilder bridge;
    private BlockCollector blockCollector;
    int platrofmIndex = 0;

   [SerializeField] int targetNumberBlocks;


    int buildingState = 0;
    private void Start()
    {
        player = GetComponent<Player>();
        controller = GetComponent<CharacterController>();
        blockCollector = GetComponent<BlockCollector>();
        character = transform.GetChild(0);
        NewPlatform();
    }
    void Update()
    {
        if (target != null && (target.TryGetComponent<Block>(out Block b) || buildingState > 0))
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            MovePlayer(new Vector2(direction.x,direction.z));
            float dis = Vector3.Distance(transform.position, target.transform.position);
            if(buildingState > 0 && blockCollector.blockCounter == 0)
            {
                target = bridge.startPoint;
                buildingState = 3;
            }

            if (dis < 0.4)
            {
                FindNewTarget();
            }            
        }
        else
        {
            FindNewTarget();
        }
    }


    private void FindNewTarget()
    {
        if (buildingState == 0)
        {
            if (blockCollector.blockCounter >= targetNumberBlocks)
            {
                target = bridge.startPoint;
                buildingState = 1;
            }
            else
            {
                target = FindClosestBlock();
            }
        }
        else
        {
            SetTarget();
        }
    }
    private void SetTarget()
    {
        switch (buildingState)
        {
            case 1:
                target = bridge.endPoint;
                buildingState = 2;
                break;
            case 2:
                buildingState = 0;
                platrofmIndex++;
                NewPlatform();
                break;
            case 3:
                buildingState = 0;
                break;
        }
    }

    GameObject FindClosestBlock()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject blockObj in blocks)
        {
            float distance = Vector3.Distance(currentPosition, blockObj.transform.position);
            Block block = blockObj.GetComponent<Block>();
            if (distance < shortestDistance && block != null)
            {
                int index = block.playerIndex;
                if (index == -1 || index == player.playerIndexGet)
                {
                    shortestDistance = distance;
                    closest = blockObj;
                }
            }
            else if (distance > 200f)
            {
                Destroy(blockObj);
            }
        }
        return closest;
    }

    private void NewPlatform()
    {
        bridge = GameManager.instance.GetBridgeBuilder(platrofmIndex);
        targetNumberBlocks = UnityEngine.Random.Range((int)(bridge.length * 0.4f), bridge.length);
    }
}