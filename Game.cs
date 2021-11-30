using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Game : MonoBehaviour,IPointerDownHandler,IDragHandler, IPointerUpHandler
{
    private Vector2 origin;                                          // ������� ������ ��� �������
    private Vector2 direction;                                         // ����������� �������������
    private Vector2[] originPosition = new Vector2[4];                  // ������� ������� � ������ ������� ������� �� �����
    public Text scor;

    private List<int> Figures = new List<int>();                         // ������ ��������� �������
    private List<GameObject> movingObject = new List<GameObject>();      // ������ ���������� �������� ������� �������� �������
    private List<GameObject> nextObject = new List<GameObject>();        // ��������� �������
    private List<List<GameObject>> Field = new List<List<GameObject>>(); // ������ ����� ��� ������(�� �������� movingObject)

    public GameObject[] SpawnObject = new GameObject[7];                 // ������ �������� ������� ����� ������������� ��� ���������� �������

    private float timer;                        // ������ ������� ������������ ���������� ��������� �������
    private float interval = 0.5f;              // �������� ����� ���������� �������
    private float step = 60f;                   // ��� ���������� ���������� ��������� ������ ����� ���������� ����� �� ����� ������� ����� ������� ���������� � �����
    private float lastPress;                    // ����� ������� �� �����
    private float stepCount;                    // ���������� �����������

    private int currentFigure, nextFigure;      // �������������� ������� � ��������� �������
    private int figureState, figureType;        // ��� ������ � � ����� �������� ��� ��������� ����� ����� ������� ������������ �������
    private int[] currentIndexX = new int[4];   // ������ �������� � ������� ������� ����� � ������� Field 
    private int[] currentIndexY = new int[4];   // ������ �������� y ������� ������� ����� � ������� Field 
    private int[] originCurrentIndexX = new int[4];  // ������� �������� ������� ������������ ������� ���������� ��� ���������
    private int[] originCurrentIndexY = new int[4];  //
    private int moveDist = 0;                        // ��������� �� ������� ����� � ������ ���������� ������� ��� ���������� ������� ����
    private int score = 0;                           // ����

    public GameParams gameParams;               // ������ ��������� ������� � �������� ������

    private bool canMoveDown;   // ����� �� ������� �������� ������ ����
    private bool canMoveSide;   // ����� �� ������� �������� � ���� �� ������
    private bool shouldMove;    // ������������� ������������� ������������ ������� ����������

    // ���������� ��� ������ � ������� � ������ �������
    public void OnPointerDown(PointerEventData eventData)
    {
        origin = eventData.position;
        for (int i = 0; i < 4; i++)
        {
            originPosition[i] = movingObject[i].transform.position;
            originCurrentIndexX[i] = currentIndexX[i];
        }
        lastPress = Time.realtimeSinceStartup;
        shouldMove = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ���������� ���������� ����������� � �� �����������
        Vector2 currentPos = eventData.position;
        direction = currentPos - origin;
        stepCount = Mathf.Floor(direction.x / step);
        // �������� �� ������� ����������� �� ���� � ����� ����������
        for (int i = 0; i < movingObject.Count; i++)
        {
            if (originCurrentIndexX[i] + (int)stepCount > 9 || originCurrentIndexX[i] + (int)stepCount < 0)
            {
                canMoveSide = false;
                break;
            }
            if (stepCount > 0)
            {
                for (int j = 1; j <= (int)stepCount; j++)
                {

                    if (Field[currentIndexY[i]][originCurrentIndexX[i] + j] != null)
                    {
                        canMoveSide = false;
                        goto LoopEnd;
                    }
                    else { canMoveSide = true; }
                }
            }
            else if (stepCount < 0)
            {
                for (int j = (int)stepCount; j < 0; j++)
                {

                    if (Field[currentIndexY[i]][originCurrentIndexX[i] + j] != null)
                    {
                        canMoveSide = false;
                        goto LoopEnd;
                    }
                    else { canMoveSide = true; }
                }
            }
        }
    LoopEnd:
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) && canMoveSide)
        {
            for (int i = 0; i < 4; i++)
            {
                // �������� ������� ������ ��� �����
                movingObject[i].transform.position = new Vector3(originPosition[i].x + 0.4f * stepCount, originPosition[i].y);
                currentIndexX[i] = originCurrentIndexX[i] + 1 * ((int)stepCount);
            }
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y) && direction.y < -80 && shouldMove)
        {
            // ���������� ���������� �� ������� ����� �������� �������
            for (int i = 0; i < 4; i++)
            {
                originCurrentIndexY[i] = currentIndexY[i];
            }
            while (true)
            {
                for (int i = 0; i < 4; i++)
                {
                   
                    if ( originCurrentIndexY[i] == 20 || Field[originCurrentIndexY[i] + 1][currentIndexX[i]] != null)
                    {
                        goto End;
                    }

                    originCurrentIndexY[i]++;
                }
                moveDist++;
            }
        End:
            // �������� �������
            for (int i = 0; i < 4; i++)
            {
                if (movingObject[i] != null)
                {
                    movingObject[i].transform.Translate(0f, -0.4f * moveDist, 0f);
                    currentIndexY[i] += moveDist;
                }
            }
            moveDist = 0;
            for (int i = 0; i < movingObject.Count; i++)
            {// �������� �� ������� �������� � ������ ������ � ����������� ���������(����������� ���� ��������) ���� 
             // � ����� ��� ������ �������� ������� ��� ���� ������
                if (Field[currentIndexY[i]][currentIndexX[i]] == null)
                {
                    Field[currentIndexY[i]][currentIndexX[i]] = movingObject[i];
                }
                else
                {
                    interval = 0.5f;
                    score = 0;
                    scor.text = score.ToString();
                    for (int k = 0; k < 21; k++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (Field[k][j] != null)
                            {
                                Destroy(Field[k][j]);
                                Field[k][j] = null;
                            }
                        }
                    }
                    for (int d = 0; d < 4; d++) { Destroy(movingObject[d]); movingObject[d] = null; }
                }
            }
            // �������� �� ������� �������� � ������ ������ � ����������� ���������(����������� ���� ��������)
            for (int n = 0; n < 10; n++)
            {
                if (Field[0][n] != null)
                {
                    interval = 0.5f;
                    score = 0;
                    scor.text = score.ToString();
                    for (int k = 0; k < 21; k++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (Field[k][j] != null)
                            {
                                Destroy(Field[k][j]);
                                Field[k][j] = null;
                            }
                        }
                    }
                    for (int d = 0; d < 4; d++) { Destroy(movingObject[d]); movingObject[d] = null; }
                    break;
                }
            }
            FigureGenerator();
            shouldMove = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (Time.realtimeSinceStartup - lastPress < 0.1f)
        {
            // ���� ����������� �����(������� ������� �� ��������� ��� �������� � �������� ������� �������) ��������� ������� � �������� �� ������������
            if (currentIndexX[0] + 1 > 9 | currentIndexX[0] - 1 < 0 || currentIndexY[0] + 1 > 20 || currentIndexY[0] - 1 < 0) 
            {
                return;
            }
            // ���������� ����� ������� � ����� ������� ������ ��������� � ������ � ���������� �������������� �������� ��� �� �� ���� �������� �����������
            if (figureType == 1)
            {
               

                if (Field[currentIndexY[0] + 1][currentIndexX[0]] != null || Field[currentIndexY[0] - 1][currentIndexX[0]] != null || Field[currentIndexY[0] + 1][currentIndexX[0] + 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] + 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0] + 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0] - 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] - 1] != null || Field[currentIndexY[0] + 1][currentIndexX[0] - 1] != null)
                {
                    return;
                }
                if (figureState == 1)
                {

                    movingObject[1].transform.Translate(0.4f, 0.4f, 0f);
                    movingObject[2].transform.Translate(-0.4f, -0.4f, 0f);
                    movingObject[3].transform.Translate(-0.4f, 0.4f, 0f);

                    currentIndexX[1]++;
                    currentIndexX[2]--;
                    currentIndexX[3]--;

                    currentIndexY[1]--;
                    currentIndexY[2]++;
                    currentIndexY[3]--;

                    figureState++;
                }
                else if (figureState == 2)
                {
                    movingObject[1].transform.Translate(0.4f, -0.4f, 0f);
                    movingObject[2].transform.Translate(-0.4f, 0.4f, 0f);
                    movingObject[3].transform.Translate(0.4f, 0.4f, 0f);

                    currentIndexX[1]++;
                    currentIndexX[2]--;
                    currentIndexX[3]++;

                    currentIndexY[1]++;
                    currentIndexY[2]--;
                    currentIndexY[3]--;

                    figureState++;
                }
                else if (figureState == 3)
                {
                    movingObject[1].transform.Translate(-0.4f, -0.4f, 0f);
                    movingObject[2].transform.Translate(0.4f, 0.4f, 0f);
                    movingObject[3].transform.Translate(0.4f, -0.4f, 0f);

                    currentIndexX[1]--;
                    currentIndexX[2]++;
                    currentIndexX[3]++;

                    currentIndexY[1]++;
                    currentIndexY[2]--;
                    currentIndexY[3]++;

                    figureState++;
                }
                else if (figureState == 4)
                {
                    movingObject[1].transform.Translate(-0.4f, 0.4f, 0f);
                    movingObject[2].transform.Translate(0.4f, -0.4f, 0f);
                    movingObject[3].transform.Translate(-0.4f, -0.4f, 0f);

                    currentIndexX[1]--;
                    currentIndexX[2]++;
                    currentIndexX[3]--;

                    currentIndexY[1]--;
                    currentIndexY[2]++;
                    currentIndexY[3]++;

                    figureState = 1;
                }
            }
            else if (figureType == 2)
            { }
            else if (figureType == 3)
            {
                if (currentIndexX[0] + 2 > 9 || currentIndexY[0] + 2 > 20||Field[currentIndexY[0] - 1][currentIndexX[0]-1] != null || Field[currentIndexY[0] - 1][currentIndexX[0]] != null || Field[currentIndexY[0] + 1][currentIndexX[0] + 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] + 1] != null || Field[currentIndexY[0] + 1][currentIndexX[0]] != null || Field[currentIndexY[0] + 2][currentIndexX[0]] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] - 1] != null || Field[currentIndexY[0] + 2][currentIndexX[0] + 1] != null || Field[currentIndexY[0]][currentIndexX[0] + 2] != null
                     || Field[currentIndexY[0] + 1][currentIndexX[0] + 2] != null || Field[currentIndexY[0] + 2][currentIndexX[0] + 2] != null )
                {
                    return;
                }

                if (figureState == 1)
                {

                    movingObject[1].transform.Translate(0.4f, 0.4f, 0f);
                    movingObject[2].transform.Translate(-0.4f, -0.4f, 0f);
                    movingObject[3].transform.Translate(-0.8f, -0.8f, 0f);

                    currentIndexX[1]++;
                    currentIndexX[2]--;
                    currentIndexX[3]-=2;

                    currentIndexY[1]--;
                    currentIndexY[2]++;
                    currentIndexY[3]+=2;

                    figureState++;
                }
                else if (figureState == 2)
                {
                    movingObject[1].transform.Translate(-0.4f, -0.4f, 0f);
                    movingObject[2].transform.Translate(0.4f, 0.4f, 0f);
                    movingObject[3].transform.Translate(0.8f, 0.8f, 0f);

                    currentIndexX[1]--;
                    currentIndexX[2]++;
                    currentIndexX[3]+=2;

                    currentIndexY[1]++;
                    currentIndexY[2]--;
                    currentIndexY[3]-=2;

                    figureState=1;
                }
            }
            else if (figureType == 4)
            {
                if (Field[currentIndexY[0] + 1][currentIndexX[0]] != null || Field[currentIndexY[0] - 1][currentIndexX[0]] != null || Field[currentIndexY[0] + 1][currentIndexX[0] + 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] + 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0] + 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0] - 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] - 1] != null || Field[currentIndexY[0] + 1][currentIndexX[0] - 1] != null)
                {
                    return;
                }

                if (figureState == 1)
                {

                    movingObject[1].transform.Translate(0.4f, 0.4f, 0f);
                    movingObject[2].transform.Translate(-0.4f, 0.4f, 0f);
                    movingObject[3].transform.Translate(-0.8f, 0f, 0f);

                    currentIndexX[1]++;
                    currentIndexX[2]--;
                    currentIndexX[3] -= 2;

                    currentIndexY[1]--;
                    currentIndexY[2]--;

                    figureState++;
                }
                else if (figureState == 2)
                {
                    movingObject[1].transform.Translate(-0.4f, -0.4f, 0f);
                    movingObject[2].transform.Translate(0.4f, -0.4f, 0f);
                    movingObject[3].transform.Translate(0.8f, 0f, 0f);

                    currentIndexX[1]--;
                    currentIndexX[2]++;
                    currentIndexX[3] += 2;

                    currentIndexY[1]++;
                    currentIndexY[2]++;

                    figureState = 1;
                }
            }
            else if (figureType == 5)
            {
                if (Field[currentIndexY[0] + 1][currentIndexX[0]] != null || Field[currentIndexY[0] - 1][currentIndexX[0]] != null || Field[currentIndexY[0] + 1][currentIndexX[0] + 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] + 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0] + 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0] - 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] - 1] != null || Field[currentIndexY[0] + 1][currentIndexX[0] - 1] != null)
                {
                    return;
                }

                if (figureState == 1)
                {

                    movingObject[1].transform.Translate(-0.4f, 0.4f, 0f);
                    movingObject[2].transform.Translate(0.4f, 0.4f, 0f);
                    movingObject[3].transform.Translate(0.8f, 0f, 0f);

                    currentIndexX[1]--;
                    currentIndexX[2]++;
                    currentIndexX[3] += 2;

                    currentIndexY[1]--;
                    currentIndexY[2]--;

                    figureState++;
                }
                else if (figureState == 2)
                {
                    movingObject[1].transform.Translate(0.4f, -0.4f, 0f);
                    movingObject[2].transform.Translate(-0.4f, -0.4f, 0f);
                    movingObject[3].transform.Translate(-0.8f, 0f, 0f);

                    currentIndexX[1]++;
                    currentIndexX[2]--;
                    currentIndexX[3] -= 2;

                    currentIndexY[1]++;
                    currentIndexY[2]++;

                    figureState = 1;
                }
            }
            else if (figureType == 6)
            {
                if (Field[currentIndexY[0] + 1][currentIndexX[0]] != null || Field[currentIndexY[0] - 1][currentIndexX[0]] != null || Field[currentIndexY[0] + 1][currentIndexX[0] + 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] + 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0] + 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0] - 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] - 1] != null || Field[currentIndexY[0] + 1][currentIndexX[0] - 1] != null)
                {
                    return;
                }

                if (figureState == 1)
                {

                    movingObject[1].transform.Translate(0.4f, -0.4f, 0f);
                    movingObject[2].transform.Translate(-0.4f, 0.4f, 0f);
                    movingObject[3].transform.Translate(0f, 0.8f, 0f);

                    currentIndexX[1]++;
                    currentIndexX[2]--;

                    currentIndexY[1]++;
                    currentIndexY[2]--;
                    currentIndexY[3]-=2;

                    figureState++;
                }
                else if (figureState == 2)
                {
                    movingObject[1].transform.Translate(-0.4f, -0.4f, 0f);
                    movingObject[2].transform.Translate(0.4f, 0.4f, 0f);
                    movingObject[3].transform.Translate(0.8f, 0f, 0f);

                    currentIndexX[1]--;
                    currentIndexX[2]++;
                    currentIndexX[3] += 2;

                    currentIndexY[1]++;
                    currentIndexY[2]--;

                    figureState++;
                }
                else if (figureState == 3)
                {
                    movingObject[1].transform.Translate(-0.4f, 0.4f, 0f);
                    movingObject[2].transform.Translate(0.4f, -0.4f, 0f);
                    movingObject[3].transform.Translate(0f, -0.8f, 0f);

                    currentIndexX[1]--;
                    currentIndexX[2]++;

                    currentIndexY[1]--;
                    currentIndexY[2]++;
                    currentIndexY[3]+=2;

                    figureState++;
                }
                else if (figureState == 4)
                {
                    movingObject[1].transform.Translate(0.4f, 0.4f, 0f);
                    movingObject[2].transform.Translate(-0.4f, -0.4f, 0f);
                    movingObject[3].transform.Translate(-0.8f, 0f, 0f);

                    currentIndexX[1]++;
                    currentIndexX[2]--;
                    currentIndexX[3]-=2;

                    currentIndexY[1]--;
                    currentIndexY[2]++;

                    figureState=1;
                }
            }
            else if (figureType == 7)
            {

                if (Field[currentIndexY[0] + 1][currentIndexX[0]] != null || Field[currentIndexY[0] - 1][currentIndexX[0]] != null || Field[currentIndexY[0] + 1][currentIndexX[0] + 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] + 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0] + 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0] - 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] - 1] != null || Field[currentIndexY[0] + 1][currentIndexX[0] - 1] != null)
                {
                    return;
                }

                if (figureState == 1)
                {

                    movingObject[1].transform.Translate(0.4f, -0.4f, 0f);
                    movingObject[2].transform.Translate(-0.4f, 0.4f, 0f);
                    movingObject[3].transform.Translate(-0.8f, 0f, 0f);

                    currentIndexX[1]++;
                    currentIndexX[2]--;
                    currentIndexX[3]-=2;

                    currentIndexY[1]++;
                    currentIndexY[2]--;

                    figureState++;
                }
                else if (figureState == 2)
                {
                    movingObject[1].transform.Translate(-0.4f, -0.4f, 0f);
                    movingObject[2].transform.Translate(0.4f, 0.4f, 0f);
                    movingObject[3].transform.Translate(0f, 0.8f, 0f);

                    currentIndexX[1]--;
                    currentIndexX[2]++;

                    currentIndexY[1]++;
                    currentIndexY[2]--;
                    currentIndexY[3] -= 2;

                    figureState++;
                }
                else if (figureState == 3)
                {
                    movingObject[1].transform.Translate(-0.4f, 0.4f, 0f);
                    movingObject[2].transform.Translate(0.4f, -0.4f, 0f);
                    movingObject[3].transform.Translate(0.8f, 0f, 0f);

                    currentIndexX[1]--;
                    currentIndexX[2]++;
                    currentIndexX[3] += 2;

                    currentIndexY[1]--;
                    currentIndexY[2]++;

                    figureState++;
                }
                else if (figureState == 4)
                {
                    movingObject[1].transform.Translate(0.4f, 0.4f, 0f);
                    movingObject[2].transform.Translate(-0.4f, -0.4f, 0f);
                    movingObject[3].transform.Translate(0f, -0.8f, 0f);

                    currentIndexX[1]++;
                    currentIndexX[2]--;
                    
                    currentIndexY[1]--;
                    currentIndexY[2]++;
                    currentIndexY[3] += 2;

                    figureState = 1;
                }
            }
        }
    }
    // ��������� ������� �� ���������� ����� ������ ������ � ������ ����� �������
    private void FigureGenerator()
    {
        
            int objectsInRow = 0, destroyedRows = 0;    // ���������� ��� �������� �������� � ������ � ���� ����� ����� ����� ����� ����
            int count = 0;                              // ���������� ��� ����������� ������� ������� ����(�������� ������ ������������ desroyedRows)
            int middleDestroyMoveDist = 0;              // ���������� ��� �������� ���������� ����� �� ������� ����� �������� ������ ����� ������������ �����
            int[] destroyedRowsIndexes = new int[4];    // ������� ������������ ����� ����� ���������� ����� ������ � �� ������� ��������
            bool shouldDestroy = false;                 // ������������ �� ����� ������ ������(������ ����� ������ �� ������� �������)
        for (int i = 20; i >= 0; i--)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (Field[i][j] != null)
                    {
                        objectsInRow++;
                    }
                }
                if (objectsInRow >= 10)
                {
                    shouldDestroy = true;
                    for (int j = 0; j < 10; j++)
                    {
                        Destroy(Field[i][j]);
                        Field[i][j] = null;

                    }
                    destroyedRows++;
                    destroyedRowsIndexes[count] = i;
                    count++;
                }
                objectsInRow = 0;
        }
        if (shouldDestroy && count > 0)
        {// �������� �� ������� �� ������������ ����� ����� ������������
            for (int i = 1; i < count; i++)
            {
                middleDestroyMoveDist++;
                if (destroyedRowsIndexes[i - 1] - destroyedRowsIndexes[i] > 2)
                {

                    for (int f = 0; f < 10; f++)
                    {
                        if (Field[destroyedRowsIndexes[i - 1] - 1][f] != null)
                        {

                            Field[destroyedRowsIndexes[i - 1] - 1][f].transform.Translate(0f, -0.4f * middleDestroyMoveDist, 0f);
                            Field[destroyedRowsIndexes[i - 1]][f] = Field[destroyedRowsIndexes[i - 1] - 1][f];
                            Field[destroyedRowsIndexes[i - 1] - 1][f] = null;
                        }
                        if (Field[destroyedRowsIndexes[i - 1] - 2][f] != null)
                        {

                            Field[destroyedRowsIndexes[i - 1] - 2][f].transform.Translate(0f, -0.4f * middleDestroyMoveDist, 0f);
                            Field[destroyedRowsIndexes[i - 1] - 1][f] = Field[destroyedRowsIndexes[i - 1] - 2][f];
                            Field[destroyedRowsIndexes[i - 1] - 2][f] = null;
                        }
                    }
                }
                else if (destroyedRowsIndexes[i - 1] - destroyedRowsIndexes[i] > 1)
                {

                    for (int f = 0; f < 10; f++)
                    {
                        if (Field[destroyedRowsIndexes[i - 1] - 1][f] != null)
                        {
                            Field[destroyedRowsIndexes[i - 1] - 1][f].transform.Translate(0f, -0.4f * middleDestroyMoveDist, 0f);
                            Field[destroyedRowsIndexes[i - 1 * middleDestroyMoveDist]][f] = Field[destroyedRowsIndexes[i - 1] - 1][f];
                            Field[destroyedRowsIndexes[i - 1] - 1][f] = null;
                        }
                    }
                }
            }
            // �� ��� ���� ��������� ������������ ������ ��������
            for (int i = destroyedRowsIndexes[count - 1]; i >= 0; i--)
            {

                for (int j = 0; j < 10; j++)
                {
                    if (Field[i][j] != null)
                    {

                        Field[i][j].transform.Translate(0f, -0.4f * destroyedRows, 0f);
                        Field[i + destroyedRows][j] = Field[i][j];
                        Field[i][j] = null;
                    }
                }
            }
        }
        if (destroyedRows == 1)
        {
            score += 100;
            scor.text = score.ToString();
        }
        else if (destroyedRows == 2)
        {
            score += 300;
            scor.text = score.ToString();
        }
        else if (destroyedRows == 3)
        {
            score += 700;
            scor.text = score.ToString();
        }
        else if (destroyedRows == 4)
        {
            score += 1000;
            scor.text = score.ToString();
        }
        // ��������� ����� �������
        currentFigure = nextFigure;
        nextFigure = Random.Range(0, Figures.Count);
        movingObject.Clear();
        currentIndexX[0] = 4;
        for (int i = 0; i < nextObject.Count; i++) 
        {
            Destroy(nextObject[i]);
        }
        nextObject.Clear();
        if (Figures[nextFigure] == 1)
        {
            nextObject.Add(Instantiate(SpawnObject[0], new Vector3(-0.2f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[0], new Vector3(-0.6f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[0], new Vector3(0.2f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[0], new Vector3(-0.2f, 3.84f, 0), Quaternion.identity) as GameObject);
        }
        else if (Figures[nextFigure] == 2)
        {
            nextObject.Add(Instantiate(SpawnObject[1], new Vector3(-0.2f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[1], new Vector3(0.2f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[1], new Vector3(-0.2f, 3.84f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[1], new Vector3(0.2f, 3.84f, 0), Quaternion.identity) as GameObject);
        }
        else if (Figures[nextFigure] == 3)
        {
            nextObject.Add(Instantiate(SpawnObject[2], new Vector3(-0.2f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[2], new Vector3(-0.6f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[2], new Vector3(0.2f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[2], new Vector3(0.6f, 4.24f, 0), Quaternion.identity) as GameObject);
        }
        else if (Figures[nextFigure] == 4)
        {
            nextObject.Add(Instantiate(SpawnObject[3], new Vector3(-0.2f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[3], new Vector3(-0.6f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[3], new Vector3(-0.2f, 3.84f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[3], new Vector3(0.2f, 3.84f, 0), Quaternion.identity) as GameObject);
        }
        else if (Figures[nextFigure] == 5)
        {
            nextObject.Add(Instantiate(SpawnObject[4], new Vector3(-0.2f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[4], new Vector3(0.2f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[4], new Vector3(-0.2f, 3.84f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[4], new Vector3(-0.6f, 3.84f, 0), Quaternion.identity) as GameObject);
        }
        else if (Figures[nextFigure] == 6)
        {
            nextObject.Add(Instantiate(SpawnObject[5], new Vector3(-0.2f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[5], new Vector3(-0.2f, 4.64f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[5], new Vector3(-0.2f, 3.84f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[5], new Vector3(-0.6f, 3.84f, 0), Quaternion.identity) as GameObject);
        }
        else if (Figures[nextFigure] == 7)
        {
            nextObject.Add(Instantiate(SpawnObject[6], new Vector3(-0.2f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[6], new Vector3(-0.2f, 4.64f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[6], new Vector3(-0.2f, 3.84f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[6], new Vector3(0.2f, 3.84f, 0), Quaternion.identity) as GameObject);
        }
        // �������� ������� � ���� ���������� ������������� �������� ����������� ��� ����������� ��������� ������� � � �������� � ����
        if (Figures[currentFigure] == 1)
        {
            figureState = 1;
            figureType = 1;

            movingObject.Add(Instantiate(SpawnObject[0], new Vector3(-0.2f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[0], new Vector3(-0.6f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[0], new Vector3(0.2f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[0], new Vector3(-0.2f, 2.84f, 0), Quaternion.identity) as GameObject);
            

            currentIndexY[0] = 0;
            currentIndexY[1] = 0;
            currentIndexY[2] = 0;
            currentIndexY[3] = 1;

            currentIndexX[1] = currentIndexX[0] - 1;
            currentIndexX[2] = currentIndexX[0] + 1;
            currentIndexX[3] = currentIndexX[0];
        }
        else if (Figures[currentFigure] == 2)
        {
            figureState = 1;
            figureType = 2;

            movingObject.Add(Instantiate(SpawnObject[1], new Vector3(-0.2f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[1], new Vector3(0.2f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[1], new Vector3(-0.2f, 2.84f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[1], new Vector3(0.2f, 2.84f, 0), Quaternion.identity) as GameObject);

            currentIndexY[0] = 0;
            currentIndexY[1] = 0;
            currentIndexY[2] = 1;
            currentIndexY[3] = 1;

            currentIndexX[1] = currentIndexX[0] + 1;
            currentIndexX[2] = currentIndexX[0];
            currentIndexX[3] = currentIndexX[0] + 1;
        }
        else if (Figures[currentFigure] == 3)
        {
            figureState = 1;
            figureType = 3;

            movingObject.Add(Instantiate(SpawnObject[2], new Vector3(-0.2f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[2], new Vector3(-0.6f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[2], new Vector3(0.2f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[2], new Vector3(0.6f, 3.24f, 0), Quaternion.identity) as GameObject);

            currentIndexY[0] = 0;
            currentIndexY[1] = 0;
            currentIndexY[2] = 0;
            currentIndexY[3] = 0;

            currentIndexX[1] = currentIndexX[0] - 1;
            currentIndexX[2] = currentIndexX[0] + 1;
            currentIndexX[3] = currentIndexX[0] + 2;
        }
        else if (Figures[currentFigure] == 4)
        {
            figureState = 1;
            figureType = 4;

            movingObject.Add(Instantiate(SpawnObject[3], new Vector3(-0.2f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[3], new Vector3(-0.6f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[3], new Vector3(-0.2f, 2.84f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[3], new Vector3(0.2f, 2.84f, 0), Quaternion.identity) as GameObject);

            currentIndexY[0] = 0;
            currentIndexY[1] = 0;
            currentIndexY[2] = 1;
            currentIndexY[3] = 1;

            currentIndexX[1] = currentIndexX[0] - 1;
            currentIndexX[2] = currentIndexX[0];
            currentIndexX[3] = currentIndexX[0] + 1;
        }
        else if (Figures[currentFigure] == 5)
        {
            figureState = 1;
            figureType = 5;

            movingObject.Add(Instantiate(SpawnObject[4], new Vector3(-0.2f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[4], new Vector3(0.2f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[4], new Vector3(-0.2f, 2.84f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[4], new Vector3(-0.6f, 2.84f, 0), Quaternion.identity) as GameObject);

            currentIndexY[0] = 0;
            currentIndexY[1] = 0;
            currentIndexY[2] = 1;
            currentIndexY[3] = 1;

            currentIndexX[1] = currentIndexX[0] + 1;
            currentIndexX[2] = currentIndexX[0];
            currentIndexX[3] = currentIndexX[0] - 1;
        }
        else if (Figures[currentFigure] == 6)
        {
            figureState = 1;
            figureType = 6;

            movingObject.Add(Instantiate(SpawnObject[5], new Vector3(-0.2f, 2.84f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[5], new Vector3(-0.2f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[5], new Vector3(-0.2f, 2.44f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[5], new Vector3(-0.6f, 2.44f, 0), Quaternion.identity) as GameObject);

            currentIndexY[0] = 1;
            currentIndexY[1] = 0;
            currentIndexY[2] = 2;
            currentIndexY[3] = 2;

            currentIndexX[1] = currentIndexX[0];
            currentIndexX[2] = currentIndexX[0];
            currentIndexX[3] = currentIndexX[0] - 1;
        }
        else if (Figures[currentFigure] == 7)
        {
            figureState = 1;
            figureType = 7;

            movingObject.Add(Instantiate(SpawnObject[6], new Vector3(-0.2f, 2.84f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[6], new Vector3(-0.2f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[6], new Vector3(-0.2f, 2.44f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[6], new Vector3(0.2f, 2.44f, 0), Quaternion.identity) as GameObject);

            currentIndexY[0] = 1;
            currentIndexY[1] = 0;
            currentIndexY[2] = 2;
            currentIndexY[3] = 2;

            currentIndexX[1] = currentIndexX[0];
            currentIndexX[2] = currentIndexX[0];
            currentIndexX[3] = currentIndexX[0] + 1;
        }
        // ����� ������� ��������� ��� ��� ����� ����� ����� ���� �� ������� � ���� �� �������� ���������� �� ����� ������� ����� �� �������
        // � ����� ������� ����������
        for (int i = 0; i < 4; i++)
        {
            originPosition[i] = movingObject[i].transform.position;
            originCurrentIndexX[i] = currentIndexX[i];
        }
        
    }

    void Start()
    {
        //���������� � ������ �� ����������� ������������� �������
        if (Figures.Count > 0)
        {
            Figures.Clear();
        }
        if (gameParams.FigureT)
        {
            Figures.Add(1);
        }
        if (gameParams.FigureQ)
        {
            Figures.Add(2);
        }
        if (gameParams.FigureI)
        {
            Figures.Add(3);
        }
        if (gameParams.FigureZ)
        {
            Figures.Add(4);
        }
        if (gameParams.FigureS)
        {
            Figures.Add(5);
        }
        if (gameParams.FigureJ)
        {
            Figures.Add(6);
        }
        if (gameParams.FigureL)
        {
            Figures.Add(7);
        }
        // ��������� ������ ���������� ����� ����� ���� ��������� � ������ �� �������� 21 ������ � 10 ���������
        for (int i = 0; i < 21; i++)
        {
            Field.Add(new List<GameObject>());
            for (int j = 0; j < 10; j++)
            {
                Field[i].Add(null);
            }
        }

        timer = Time.realtimeSinceStartup + interval;
        nextFigure = Random.Range(0, Figures.Count);

        currentIndexX[0] = 4; // ������� ������� �������� �� ������� movementObject � ������� List ��� ������ ������ ���������
                              // (� ���� ������ ��� �������� �� ������ ����� ��������� ������������ ���� ������)

        FigureGenerator();

    }

    void FixedUpdate()
    {
        if (Time.realtimeSinceStartup >= timer)
        {
            timer = Time.realtimeSinceStartup + interval;
            // �������� �� ������������� ��������� ��� ��������� �������
            for (int i = 0; i < movingObject.Count; i++)
            {
                if (currentIndexY[i] + 1 > 20   ||  Field[currentIndexY[i] + 1][currentIndexX[i]] != null)
                {
                    canMoveDown = false;
                    break;
                }
                else
                {
                    canMoveDown = true;
                }
            }

            if (canMoveDown)
            {
                // ��������� � ������� ������������ ������� ���������� ����������� �� ��������� ���� � ����������� ������� �� �����
                originPosition[0].y -= 0.4f;
                originPosition[1].y -= 0.4f;
                originPosition[2].y -= 0.4f;
                originPosition[3].y -= 0.4f;
                currentIndexY[0]++;
                currentIndexY[1]++;
                currentIndexY[2]++;
                currentIndexY[3]++;
                
                for (int i = 0; i < movingObject.Count; i++)
                   {
                      movingObject[i].transform.Translate(0f, -0.4f, 0f);
                   }
                
            }
            else
            {
                
                for (int i = 0; i < movingObject.Count; i++)
               {//�������� �� ���������
                    if (Field[currentIndexY[i]][currentIndexX[i]] == null)
                    {
                        Field[currentIndexY[i]][currentIndexX[i]] = movingObject[i];
                    }
                    else 
                    {
                        interval = 0.5f;
                        score = 0;
                        scor.text = score.ToString();
                        for (int k = 0; k < 21; k++)
                        {
                            for (int j = 0; j < 10; j++)
                            {
                                if (Field[k][j] != null)
                                {
                                    Destroy(Field[k][j]);
                                    Field[k][j] = null;
                                }
                            }
                        }
                        for (int d = 0; d < 4; d++) { Destroy(movingObject[d]); movingObject[d] = null; }
                    }
                    for (int n = 0; n < 10; n++)
                    {
                        if (Field[0][n] != null)
                        {
                            interval = 0.5f;
                            score = 0;
                            scor.text = score.ToString();
                            for (int k = 0; k < 21; k++)
                            {
                                for (int j = 0; j < 10; j++)
                                {
                                    if (Field[k][j] != null)
                                    {
                                        Destroy(Field[k][j]);
                                        Field[k][j] = null;
                                    }
                                }
                            }
                            for (int d = 0; d < 4; d++) { Destroy(movingObject[d]); movingObject[d] = null; }
                            break;
                        }
                    }
                }
               FigureGenerator();  
            }
            // ��������� ���� ������� �� ���������� ����� ���������
            if (interval >= 0.02)
            {
                interval -= 0.0001f;
            }
        }
    }
}
