using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // EventSystemを使用するために必要

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private RectTransform joystickBackground; // ジョイスティックの背景
    private RectTransform joystickHandle; // ジョイスティックのハンドル
    public Vector3 inputVector; // 入力を保存するベクトル
    public bool IsDragging { get; private set; } = false;
    private void Start()
    {
        joystickBackground = GetComponent<RectTransform>();
        joystickHandle = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
         IsDragging = true;
        Vector2 position;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out position))
        {
            position.x = (position.x / joystickBackground.sizeDelta.x);
            position.y = (position.y / joystickBackground.sizeDelta.y);

            inputVector = new Vector3(position.x * 2, 0, position.y * 2);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            // ハンドルの移動
            joystickHandle.anchoredPosition = new Vector3(inputVector.x * (joystickBackground.sizeDelta.x / 2), inputVector.z * (joystickBackground.sizeDelta.y / 2));
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
         IsDragging = true;
        OnDrag(eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
         IsDragging = false;
        inputVector = Vector3.zero;
        joystickHandle.anchoredPosition = Vector3.zero;
    }

    public float Horizontal()
    {
        return inputVector.x;
    }

    public float Vertical()
    {
        return inputVector.z;
    }
}
