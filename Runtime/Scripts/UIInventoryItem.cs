using HHG.Common.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace HHG.InventorySystem.Runtime
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIInventoryItem : MonoBehaviour, IRefreshable<IInventoryItem>, IBeginDragHandler, IDragHandler, IEndDragHandler, ITooltip
    {
        public IInventoryItem Item => item;
        public string TooltipText => item?.TooltipText ?? string.Empty;

        [SerializeField, FormerlySerializedAs("ItemIcon")] private Image icon;
        [SerializeField, FormerlySerializedAs("ItemBackground")] private Image background;
        [SerializeField] private ActionEvent onBeginDrag = new ActionEvent();

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

            icon.sprite = item?.Icon;
            icon.enabled = item?.Icon;
            icon.color = item?.IconColor ?? Color.white;

            if (background != null)
            {
                background.sprite = item?.Background;
                background.enabled = item?.Background;
                background.color = item?.BackgroundColor ?? Color.white;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = .6f;
            canvasGroup.blocksRaycasts = false;
            onBeginDrag?.Invoke(this);
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