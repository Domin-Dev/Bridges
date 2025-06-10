using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerMovement : Movement
{

    Vector2? lastTouch = null; // Inicjalizacja jako null
    Vector2 lastDir = Vector2.zero;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouch = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && lastTouch.HasValue)
            {
                if (Vector2.Distance(touch.position, lastTouch.Value) > 50)
                {
                    lastDir = (touch.position - lastTouch.Value).normalized;
                    lastTouch = touch.position;
                }

                MovePlayer(lastDir);
            }
            else
            {
                MovePlayer(lastDir);
            }

            Debug.Log(lastDir);
        }
    }

        private Vector2 GetInput()
    {
        Vector2 input;
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        return input;
    }
}
