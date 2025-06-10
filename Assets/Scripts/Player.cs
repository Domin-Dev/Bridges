using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] int playerIndex;
    public Transform ignoresCollisions;

    public int playerIndexGet { get { return playerIndex; } }
    private void Start()
    {
        Color color = GameManager.instance.GetColor(playerIndex);
        meshRenderer.materials[0].color = color;
        meshRenderer.materials[1].color = color;
    }
}
