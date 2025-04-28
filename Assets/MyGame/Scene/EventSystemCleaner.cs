// EventSystemFixer.cs
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class EventSystemFixer : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(FixEventSystems());
    }

    private IEnumerator FixEventSystems()
    {
        yield return null; // Chờ đúng 1 frame
        EventSystem[] systems = FindObjectsOfType<EventSystem>();
        if (systems.Length > 1)
        {
            for (int i = 1; i < systems.Length; i++)
            {
                Destroy(systems[i].gameObject); // Xoá cái thừa
            }
        }
    }
}
