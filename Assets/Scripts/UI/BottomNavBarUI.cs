using UnityEngine;
using UnityEngine.UI;

public class BottomNavBarUI : MonoBehaviour
{
    private Transform[] containers;
    private Transform currentSelected;

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

        // Mặc định chọn Button 2
        currentSelected = containers[1];
        DOAnimationManager.ScaleBounce(currentSelected, 0.2f);
    }
    public void ChangeCurrentSelected(int index)
    {

    }
    private void OnButtonClicked(Transform selectedContainer)
    {
        // Nếu nhấn lại chính nó → không làm gì
        if (currentSelected == selectedContainer)
            return;

        // Reset scale các button khác
        foreach (var container in containers)
            container.localScale = Vector3.one;

        // Scale cho item được chọn
        DOAnimationManager.ScaleBounce(selectedContainer, 0.2f);

        // Cập nhật biến currentSelected
        currentSelected = selectedContainer;
    }
}
