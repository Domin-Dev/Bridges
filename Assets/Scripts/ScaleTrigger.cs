using UnityEngine;

[ExecuteInEditMode]
public class ScaleTrigger : MonoBehaviour
{
    public Transform targetToMove;
    public Vector3 newPositionIfScaled;
    private Vector3 lastScale;

    void OnValidate()
    {
            Debug.Log("Skala siê zmieni³a!");
    }
}