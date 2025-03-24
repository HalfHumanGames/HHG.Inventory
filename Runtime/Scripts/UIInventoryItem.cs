using HHG.Common.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace HHG.Inventory.Runtime
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIInventoryItem : 
        MonoBehaviour, 
        IRefreshable<IInventoryItem>, 
        ITooltip, 
        IBeginDragHandler, 
        IDragHandler, 
        IEndDragHandler
    {
        public static UIInventoryItem Dragged;

        public IInventoryItem Item => item;
        public string TooltipText => item?.TooltipText ?? string.Empty;
        public UIInventorySlot Slot => slot;
        public RectTransform RectTransform => rect;

        [SerializeField] private bool disableDrag;
        [SerializeField, FormerlySerializedAs("ItemIcon")] private Image icon;
        [SerializeField, FormerlySerializedAs("ItemBackground")] private Image background;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private ActionEvent onBeginDrag = new ActionEvent();
        [SerializeField] private ActionEvent onEndDrag = new ActionEvent();

        private InputActionReference submitActionReference;
        private InputActionReference cancelActionReference;
        private UIInventorySlot slot;
        private RectTransform rect;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private IInventoryItem item;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>(true);
            rect = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            slot = GetComponentInParent<UIInventorySlot>(true);

            if (EventSystem.current && EventSystem.current.TryGetComponent(out InputSystemUIInputModule module))
            {
                submitActionReference = module.submit;
                cancelActionReference = module.cancel;
                submitActionReference.action.performed += OnSubmit;
                cancelActionReference.action.performed += OnCancel;
            }           
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
                HandleBeginDrag();
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
                HandleEndDrag();   
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

        private void HandleDrop(UIInventorySlot slot)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.pointerDrag = Dragged.gameObject;
            slot.OnDrop(pointerEventData);

            Dragged = null;
            HandleEndDrag();
        }

        private void HandleBeginDrag()
        {
            transform.SetParent(rect.root);
            transform.SetAsLastSibling();
            canvasGroup.blocksRaycasts = false;
            onBeginDrag?.Invoke(this);
        }

        private void HandleEndDrag()
        {
            transform.SetParent(slot.transform);
            canvasGroup.blocksRaycasts = true;
            rect.anchoredPosition = Vector2.zero;
            onEndDrag?.Invoke(this);
        }

        private void OnSubmit(InputAction.CallbackContext ctx)
        {
            if (slot.Selectable.gameObject != EventSystem.current.currentSelectedGameObject)
            {
                return;
            }

            if (!disableDrag)
            {
                if (Dragged == null)
                {
                    Dragged = this;
                    HandleBeginDrag();

                    EventSystem.current.SetSelectedGameObject(null);
                    slot.Selectable.Select();
                }
                else
                {
                    Dragged.HandleDrop(slot);
                }
            }
        }

        private void OnCancel(InputAction.CallbackContext ctx)
        {
            if (slot.Selectable.gameObject != EventSystem.current.currentSelectedGameObject)
            {
                return;
            }

            if (!disableDrag)
            {
                if (Dragged)
                {
                    Dragged.HandleEndDrag();
                }
            }
        }

        private void OnDestroy()
        {
            if (submitActionReference)
            {
                submitActionReference.action.performed -= OnSubmit;
            }

            if (cancelActionReference)
            {
                cancelActionReference.action.performed -= OnCancel;
            }
        }
    }
}