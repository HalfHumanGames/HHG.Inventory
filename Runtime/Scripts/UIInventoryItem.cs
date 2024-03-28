using HHG.Common.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HHG.InventorySystem.Runtime
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIInventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, ITooltip
    {
        public IInventoryItem Item => item;
        public string TooltipText => item.TooltipText;

        [SerializeField] private Image ItemIcon;

        private IInventoryItem item;
        private Canvas canvas;
        private RectTransform rect;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            rect = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Refresh(IInventoryItem inventoryItem)
        {
            item = inventoryItem;
            ItemIcon.sprite = item?.Icon;
            ItemIcon.enabled = item?.Icon;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = .6f;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            rect.anchoredPosition = Vector2.zero;
        }
    }
}