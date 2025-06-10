using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(AddComponent<Block>(1f));
    }

    IEnumerator AddComponent<T>(float delay) where T : Component
    {
        yield return new WaitForSeconds(delay);

        if (GetComponent<T>() == null)
        {
            gameObject.AddComponent<T>();
            Destroy(this);
        }
    }

}
