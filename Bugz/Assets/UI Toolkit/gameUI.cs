using Assets.Scripts.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class gameUI : MonoBehaviour
{
    public static ItemEnum SelectedItem;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var pipe = root.Q<Button>("pipe");
        pipe.clicked += Pipe_clicked;

        SelectedItem = ItemEnum.None;
    }

    private void Pipe_clicked()
    {
        SelectedItem = ItemEnum.Pipe;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
