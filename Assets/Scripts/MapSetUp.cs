using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapSetUp : MonoBehaviour
{
    Dictionary<int, Platform> map = new Dictionary<int, Platform>();

    private void Awake()
    {
        LoadMap();
        BuildMap();
    }
    private void LoadMap()
    {
        map = new Dictionary<int, Platform>();
        Platform[] platforms = FindObjectsOfType<Platform>();
        foreach (Platform platform in platforms)
        {
            map.Add(platform.index, platform);
        }
    }
    private void BuildMap()
    {
        if (map.Count < 2) return;
        Platform previous = map[0];
        previous.nextPlatform = map[1];

        for (int i = 1; i < map.Count; i++)
        {
            Platform platform = map[i];
            Vector3 newPosition = previous.GetPositionNextPlatform(platform.transform);
            if (platform.spawner != null)
            {
                Vector3 offset = platform.spawner.transform.position - platform.transform.position;
                platform.spawner.transform.position = newPosition + offset;
            }

            if(i + 1 < map.Count)
            {
                platform.nextPlatform = map[i + 1];
            }
            platform.transform.position = newPosition;
            previous = platform;
        }
    }
}
