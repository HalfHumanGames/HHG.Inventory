using HHG.Common.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HHG.InventorySystem.Runtime
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIInventoryItem : MonoBehaviour, IRefreshable<IInventoryItem>, IBeginDragHandler, IDragHandler, IEndDragHandler, ITooltip
    {
        public IInventoryItem Item => item;
        public string TooltipText => item?.TooltipText ?? string.Empty;

        [SerializeField] private Image ItemIcon;
        [SerializeField] private Image ItemBackground;

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
            ItemIcon.color = item?.IconColor ?? Color.white;

            if (ItemBackground != null)
            {
                ItemBackground.sprite = item?.Background;
                ItemBackground.enabled = item?.Background;
                ItemBackground.color = item?.BackgroundColor ?? Color.white;
            }
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