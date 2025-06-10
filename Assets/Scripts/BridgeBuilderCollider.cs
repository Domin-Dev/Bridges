using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class BridgeBuilderCollider : MonoBehaviour
{
    [SerializeField] private BoxCollider collider;

    public event Action<BlockCollector, BridgeBuilderCollider> OnCollisionEnterEvent;

    public int playerIndex = -1;
    public int blockIndex = 0;
    public void SetUp(int indexPlayer, int blockIndex)
    {
        this.playerIndex = indexPlayer;
        this.blockIndex = blockIndex;
        IgnoreCollision(LayerMask.NameToLayer("Player"+indexPlayer),true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            BlockCollector blockCollector = other.GetComponent<BlockCollector>();
            Player player = other.GetComponent<Player>();
            if (player.ignoresCollisions != null && player.ignoresCollisions == this.transform)
            {
                IgnoreCollision(other.gameObject.layer, false);
                player.ignoresCollisions = null;
            }

            if (blockCollector.blockCounter > 0 && CheckPlayerIndex(blockCollector.playerIndexGet))
            {
                OnCollisionEnterEvent?.Invoke(blockCollector, this);
            }
        }
    }
    private bool CheckPlayerIndex(int index)
    {
        return playerIndex == -1 || index != this.playerIndex;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 direction = (collision.transform.position - transform.position).normalized;
        Player player = collision.gameObject.GetComponent<Player>();
        if (direction.y > 0.0f && playerIndex != player.playerIndexGet && player.ignoresCollisions == null)
        {
            IgnoreCollision(collision.gameObject.layer, true);
            collision.gameObject.GetComponent<Player>().ignoresCollisions = this.transform;
        }
    }
    private void IgnoreCollision(int layer,bool ignore)
    {
        int enemyLayerMask = 1 << layer;
        if(ignore) collider.excludeLayers |= enemyLayerMask;
        else collider.excludeLayers &= ~enemyLayerMask;
    }
    public void SelfDestruction()
    {
        Destroy(this.gameObject);
    }
}
