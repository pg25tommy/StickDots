using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlayerContainer : MonoBehaviour
{
    [SerializeField] private float initialMoveSpeed = 300f; 
    [SerializeField] public float finalMoveSpeed = 10f;
    [SerializeField] public float accelerationRate = 100f;
    [SerializeField] private Transform highlightPos;
    [SerializeField] private float layoutOffset = 20f;
    [SerializeField] private float childSize = 80f;

    private List<GameObject> avatorList = new List<GameObject>();

    private List<Vector2> posList = new List<Vector2>();

    void Start()
    {
        
        //StartCoroutine(MoveToPos(new Vector2(-200,0)));
    }

    void Update()
    {
        
    }

    public void InitAvatorList(int playerCount)
    {
        avatorList.Clear();
        posList.Clear();

        Vector3 localPosition = transform.InverseTransformPoint(highlightPos.position);

        posList.Add(localPosition);

        for (int i = 0; i < playerCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.GetComponentInChildren<Image>().color = GamePlayManager.Instance.players[i].GetComponent<Player>().myColor;
            Color color = child.GetComponentInChildren<Image>().color;
            color.a = 1f;
            child.GetComponentInChildren<Image>().color = color;


            avatorList.Add(child.gameObject);

            if (i != 0)
            {
                posList.Add(new Vector2(i * layoutOffset, 0));
            }
            Debug.Log("posList "+i + posList[i]);
        }



        foreach (GameObject avator in avatorList)
        {
            Debug.Log(avator.name);
        }
        Layout();
    }

    public void Layout()
    {
        Debug.Log("Layout"+ avatorList.Count);

        if(avatorList.Count <= 0) { return; }

        for (int i = 0; i < avatorList.Count; i++)
        {
            RectTransform rectTransform = avatorList[i].GetComponent<RectTransform>();
            rectTransform.localPosition = posList[i];
            rectTransform.sizeDelta = new Vector2(childSize, childSize);
        }
    }
    private int times = 1;
    public void rotateAvator()
    {

        StopAllCoroutines();
        Vector3 localPosition = transform.InverseTransformPoint(highlightPos.position);
        int newSiblingIndex = 0;
        for (int i = 0; i < avatorList.Count; i++)
        {
            int index = Mathf.Abs(i - times) % avatorList.Count;

            if(posList[index].x == localPosition.x && posList[index].y == localPosition.y)
            {
                //Debug.Log("ToRight"+ i);
                // Ensure the newSiblingIndex is within the valid range
                newSiblingIndex = 0;//Mathf.Clamp(newSiblingIndex, 0, avatorList.Count - 1);

                // Move the object to the new sibling index
                avatorList[i].transform.SetSiblingIndex(newSiblingIndex);
            }
            else
            {
                //Debug.Log("++ " + i);
                int currentSiblingIndex = avatorList[i].transform.GetSiblingIndex();

                newSiblingIndex = currentSiblingIndex - 1;

                avatorList[i].transform.SetSiblingIndex(newSiblingIndex);
            }

            Debug.Log("index"+index);
            StartCoroutine(MoveToPos(avatorList[i], posList[index]));
        }
        times+=1;
    }

    private IEnumerator MoveToPos(GameObject obj, Vector2 targetPosition)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        float currentSpeed = initialMoveSpeed;
        while (currentSpeed > finalMoveSpeed)
        {
            rect.anchoredPosition  = Vector2.MoveTowards(rect.anchoredPosition,targetPosition, currentSpeed * Time.deltaTime);

            currentSpeed -= accelerationRate * Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = targetPosition; 
    }

}
