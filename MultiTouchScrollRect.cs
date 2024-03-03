using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace ScrolRect.Extensions
{
    public class MultiTouchScrollRect : ScrollRect
    {
        #region Fields
        private bool _draging = false;
        private Vector2 _dragOffset = Vector2.zero;
        private Dictionary<int, Vector2> _activePointers = new Dictionary<int, Vector2>();
        #endregion

        #region Properties
        public bool IsHandheld
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorApplication.isRemoteConnected;
#else
                return SystemInfo.deviceType == DeviceType.Handheld;
#endif
            }
        }

        #endregion

        #region Methods

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (IsHandheld)
            {
                _activePointers[eventData.pointerId] = eventData.position;

                if (Input.touchCount >= 1 && !_draging)
                {
                    base.OnBeginDrag(eventData);
                    _draging = true;
                    _dragOffset = eventData.position;
                }
            }
            else if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                base.OnBeginDrag(eventData);
            }
        }
        public override void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (IsHandheld)
            {
                if (Input.touchCount >= 1 && _activePointers.ContainsKey(eventData.pointerId))
                {
                    _dragOffset += eventData.position - _activePointers[eventData.pointerId];
                    _activePointers[eventData.pointerId] = eventData.position;
                    eventData.position = _dragOffset;
                    base.OnDrag(eventData);
                }
            }
            else if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                base.OnDrag(eventData);
            }
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (IsHandheld)
            {
                if (_activePointers.ContainsKey(eventData.pointerId))
                {
                    _activePointers.Remove(eventData.pointerId);
                }

                if (Input.touchCount == 1)
                {
                    base.OnEndDrag(eventData);
                    _draging = false;
                    _dragOffset = Vector2.zero;
                }

            }
            else if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                base.OnEndDrag(eventData);
            }
        }
        #endregion
    }
}
