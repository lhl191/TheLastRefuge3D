using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemFix : MonoBehaviour
{
    void Awake()
    {
        var systems = FindObjectsOfType<EventSystem>();

        // Nếu có nhiều hơn 1 EventSystem, giữ lại cái đầu tiên, xóa những cái sau
        for (int i = 1; i < systems.Length; i++)
        {
            Debug.LogWarning("Duplicate EventSystem detected, destroying: " + systems[i].gameObject.name);
            Destroy(systems[i].gameObject);
        }
    }
}

