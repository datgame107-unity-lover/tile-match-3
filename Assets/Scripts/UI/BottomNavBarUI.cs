using UnityEngine;
using UnityEngine.UI;

public class BottomNavBarUI : MonoBehaviour
{
    private Transform[] containers;

    void Start()
    {
        containers = new Transform[3];
        containers[0] = transform.Find("Button1/Container");
        containers[1] = transform.Find("Button2/Container");
        containers[2] = transform.Find("Button3/Container");

        Button btn1 = transform.Find("Button1").GetComponent<Button>();
        btn1.onClick.AddListener(() => OnButtonClicked(containers[0]));

        Button btn2 = transform.Find("Button2").GetComponent<Button>();
        btn2.onClick.AddListener(() => OnButtonClicked(containers[1]));

        Button btn3 = transform.Find("Button3").GetComponent<Button>();
        btn3.onClick.AddListener(() => OnButtonClicked(containers[2]));
    }

    private void OnButtonClicked(Transform selectedContainer)
    {
        if (selectedContainer.localScale.x > 1f)
            return;
        foreach(Transform container in containers)
        {
            container.localScale = Vector3.one;
        }    
        foreach (Transform container in containers)
        {
            if (container == selectedContainer)
            {
                DOAnimationManager.ScaleBounce(container, 1.3f, 0.2f);
            }
            else
            {
                container.localScale = Vector3.one;
            }
        }
    }
}
