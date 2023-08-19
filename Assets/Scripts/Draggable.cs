using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IInitializePotentialDragHandler,  IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Canvas canvas;

    private CanvasGroup m_canvasGroup;
    private RectTransform m_RectTransform;
    private RectTransform dropArea;
    private RectTransform childDropArea;
    private GameObject blockObject;
    private void Awake()
    {
        m_RectTransform= GetComponent<RectTransform>();
        m_canvasGroup= GetComponent<CanvasGroup>();
    }

    public void SetDropArea(RectTransform area)
    {
        this.dropArea= area;
    }

    public RectTransform GetDropArea()
    {
        return this.dropArea;
    }

    public void SetChildDropArea(RectTransform area)
    {
        this.childDropArea = area;
    }

    public RectTransform GetChildDropArea()
    {
        return this.childDropArea;
    }

    public void SetGameObject(GameObject block)
    {
        this.blockObject = block;
    }

    public GameObject GetGameObject()
    {
        return this.blockObject;
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        eventData.useDragThreshold = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        m_canvasGroup.blocksRaycasts = false;
        m_canvasGroup.alpha = .6f;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        m_canvasGroup.blocksRaycasts = true;
        m_canvasGroup.alpha = 1f;

        if(dropArea != null)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(childDropArea, eventData.position, eventData.pressEventCamera))
            {
                ResettingValue();
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_RectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //Debug.Log("OnDrag");
    }

    public void OnDrop(PointerEventData eventData)
    {

    }

    public void ResettingValue()
    {
        if(blockObject != null)
        {
            IResetable<bool> currentObj = blockObject.GetComponent<IResetable<bool>>();
            Debug.Log(currentObj);
            bool proceed = currentObj.ResetValue();
        }
    }
}

