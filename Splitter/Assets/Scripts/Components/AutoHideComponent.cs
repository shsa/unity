using UnityEngine;
using UnityEngine.UI;

public class AutoHideComponent : MonoBehaviour
{
    void Start()
    {
        var image = GetComponent<Image>();
        if (image != null)
        {
            image.enabled = false;
        }
    }
}
