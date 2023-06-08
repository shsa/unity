using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public enum DialogEnum
{
    Running,
    Ok,
    Cancel,
}

public class InputBoxController : MonoBehaviour
{
    public static InputBoxController Instance;
    static DialogEnum running = DialogEnum.Running;
    static string value;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        Instance = this;

        var root = GetComponent<UIDocument>().rootVisualElement;
        var buttonOk = root.Q<Button>("buttonOk");
        var buttonCancel = root.Q<Button>("buttonCancel");

        var inputText = root.Q<TextField>("inputText");

        buttonOk.clicked += () =>
        {
            running = DialogEnum.Ok;
            value = inputText.text;
        };
        buttonCancel.clicked += () => running = DialogEnum.Cancel;
    }

    public async Task<string> GetResult()
    {
        running = DialogEnum.Running;
        gameObject.SetActive(true);

        while (running == DialogEnum.Running)
        {
            await Task.Delay(100);
        }

        gameObject.SetActive(false);

        if (running == DialogEnum.Ok)
        {
            return value;
        }

        return null;
    }

    public static async Task<string> Show()
    {
        return await Instance.GetResult();
    }
}
