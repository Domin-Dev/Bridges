using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    protected CharacterController controller;
    protected Transform character;

    [SerializeField] Animator animator;
    Vector3 spawnPoint;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        spawnPoint = transform.position;
        character = transform.GetChild(0);
    }

    protected void MovePlayer(Vector2 input)
    {
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        float value = move.normalized.magnitude;
        move.y = controller.isGrounded ? 0f : -1f;
        controller.Move(move.normalized * moveSpeed * Time.deltaTime);

        Vector3 lookDir = new Vector3(move.x, 0, move.z);
        if (lookDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            character.rotation = Quaternion.Slerp(transform.GetChild(0).rotation, targetRotation, Time.deltaTime * 14f); 
        }
        animator.SetFloat("MoveSpeed", value);

        if (transform.position.y < -2f) transform.position = spawnPoint;
    }
}
