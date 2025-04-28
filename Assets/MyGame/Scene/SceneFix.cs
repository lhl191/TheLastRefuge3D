using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SceneInitializer : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(ActivateEventSystem());
    }

    private IEnumerator ActivateEventSystem()
    {
        yield return null; // Chờ 1 frame
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem != null && !eventSystem.gameObject.activeSelf)
        {
            eventSystem.gameObject.SetActive(true);
        }
    }
}

