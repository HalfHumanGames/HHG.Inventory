using HHG.Common.Runtime;
using TMPro;
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
        public UIInventorySlot Slot => slot;

        [SerializeField] private bool disableDrag;
        [SerializeField, FormerlySerializedAs("ItemIcon")] private Image icon;
        [SerializeField, FormerlySerializedAs("ItemBackground")] private Image background;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private ActionEvent onBeginDrag = new ActionEvent();
        [SerializeField] private ActionEvent onEndDrag = new ActionEvent();

        private IInventoryItem item;
        private Canvas canvas;
        private RectTransform rect;
        private CanvasGroup canvasGroup;
        private UIInventorySlot slot;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>(true);
            rect = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            slot = GetComponentInParent<UIInventorySlot>(true);
        }

        public virtual void Refresh(IInventoryItem inventoryItem)
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

            if (description != null)
            {
                description.text = item?.TooltipText;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!disableDrag)
            {
                transform.SetParent(rect.root);
                transform.SetAsLastSibling();
                canvasGroup.blocksRaycasts = false;
                onBeginDrag?.Invoke(this);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!disableDrag)
            {
                rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!disableDrag)
            {
                transform.SetParent(slot.transform);
                canvasGroup.blocksRaycasts = true;
                rect.anchoredPosition = Vector2.zero;
                onEndDrag?.Invoke(this);
            }
        }

        public void EnableDrag()
        {
            disableDrag = false;
        }

        public void DisableDrag()
        {
            disableDrag = true;
        }
    }
}