using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BW.UI
{
    public class HoldEventTrigger : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEvent _onDown = new UnityEvent();

        [SerializeField] private UnityEvent _onHold = new UnityEvent();

        [SerializeField] private UnityEvent _onUnhold = new UnityEvent();

        public UnityEvent OnDown => this._onDown;

        public UnityEvent OnHold => this._onHold;

        public UnityEvent OnUnhold => this._onUnhold;

        private bool isPointerDown;
        private bool _hold;
        private float _timer;
        private const float MinDuration = 0.2f;

        public bool IsHolding
        {
            get { return this._hold; }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.isPointerDown = true;
            this._hold = false;
            this._timer = 0f;
            this._onDown.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this.isPointerDown = false;
            this._hold = false;
            this._timer = 0f;
            OnUnhold.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.isPointerDown = false;
            this._hold = false;
            this._timer = 0f;
            OnUnhold.Invoke();
        }

        private void Update()
        {
            if (!this.isPointerDown) return;

            if (!this._hold)
            {
                this._timer += Time.deltaTime;

                if (this._timer >= MinDuration)
                {
                    this._hold = true;
                }
            }

            if (this._hold)
            {
                OnHold.Invoke();
            }
        }
    }
}