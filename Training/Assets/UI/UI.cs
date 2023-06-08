using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    public InputBoxController inputBox;

    // Start is called before the first frame update
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var buttonStart = root.Q<Button>("buttonStart");
        var buttonStop = root.Q<Button>("buttonStart");
        var buttonTest = root.Q<Button>("buttonTest");

        buttonStart.clicked += async () => 
        {
            Debug.Log(await inputBox.GetResult());
        };
    }
}
