using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform viewPortTransform;
    public RectTransform contentPanelTransform;
    public VerticalLayoutGroup VLG; 
    public RectTransform[] ItemList;
    private Vector2 OldVelocity;
    bool isUpdated;

    void Start()
    {
        isUpdated = false;
        OldVelocity = Vector2.zero;

        ItemList = contentPanelTransform.GetComponentsInChildren<RectTransform>();
        ItemList = ItemList.Where(item => item != contentPanelTransform).ToArray();

        int ItemsToAdd = Mathf.CeilToInt(viewPortTransform.rect.height / (ItemList[0].rect.height + VLG.spacing));

        for (int i = 0; i < ItemsToAdd; i++)
        {
            RectTransform RT = Instantiate(ItemList[i % ItemList.Length], contentPanelTransform);
            RT.SetAsLastSibling();
        }

        for (int i = 0; i < ItemsToAdd; i++)
        {
            int num = ItemList.Length - i - 1;
            while (num < 0)
            {
                num += ItemList.Length;
            }
            RectTransform RT = Instantiate(ItemList[num], contentPanelTransform);
            RT.SetAsFirstSibling();
        }

        float contentHeight = ItemList[0].rect.height * ItemsToAdd + VLG.spacing * (ItemsToAdd - 1);
        float viewPortHeight = viewPortTransform.rect.height;

        contentPanelTransform.anchoredPosition = new Vector2(contentPanelTransform.anchoredPosition.x, Mathf.Clamp(-contentHeight / 2, -viewPortHeight / 2, viewPortHeight / 2));
    }

    void Update()
    {
        if (isUpdated)
        {
            isUpdated = false;
            scrollRect.velocity = OldVelocity;
        }

        float contentHeight = ItemList[0].rect.height * ItemList.Length + VLG.spacing * (ItemList.Length - 1);
        float viewPortHeight = viewPortTransform.rect.height;

        if (contentPanelTransform.anchoredPosition.y > viewPortHeight / 2)
        {
            Canvas.ForceUpdateCanvases();
            OldVelocity = scrollRect.velocity;
            contentPanelTransform.anchoredPosition -= new Vector2(0, contentHeight);
            isUpdated = true;
        }

        if (contentPanelTransform.anchoredPosition.y < -contentHeight / 2 - viewPortHeight / 2)
        {
            Canvas.ForceUpdateCanvases();
            OldVelocity = scrollRect.velocity;
            contentPanelTransform.anchoredPosition += new Vector2(0, contentHeight);
            isUpdated = true;
        }
    }
}
