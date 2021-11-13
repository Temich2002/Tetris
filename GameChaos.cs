using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GameChaos : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    Vector2 origin;
    Vector2 direction;
    Vector2[] originPosition = new Vector2[4];                  
    Vector3 cameraOriginPos;
    Vector2 originBorderPos;

    public Camera MainCamera;

    List<int> Figures = new List<int>();
    List<GameObject> movingObject = new List<GameObject>();      
    List<GameObject> nextObject = new List<GameObject>();        
    List<List<GameObject>> Field = new List<List<GameObject>>();
    List<GameObject> mustDestroyObjects = new List<GameObject>();

    public GameObject[] SpawnObject = new GameObject[7];                
    public GameObject Border;

    float timer;                      
    float interval = 0.5f;              
    float step = 60f;                   
    float lastPress;                    
    float stepCount;                   

    int currentFigure, nextFigure;      
    int figureState, figureType;        
    int[] currentIndexX = new int[4];   
    int[] currentIndexY = new int[4];  
    int[] originCurrentIndexX = new int[4];  
    int[] originCurrentIndexY = new int[4];  
    int moveDist = 0;                            
    List<int> colonShouldMove = new List<int>(); 
    List<int> colonIndex = new List<int>();      

    public GameParams gameParams;              

    bool canMoveDown;
    bool canMoveSide = true;
    bool shouldMove;

   
    public void OnPointerDown(PointerEventData eventData)
    {
        origin = eventData.position;
        cameraOriginPos = MainCamera.transform.position;
        originBorderPos = Border.transform.position;
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
       
        Vector2 currentPos = eventData.position;
        direction = currentPos - origin;
        stepCount = Mathf.Floor(direction.x / step);
        /*  for (int i = 0; i < movingObject.Count; i++)
          {
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
      LoopEnd:*/
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) && canMoveSide)
        {
         
            for (int i = 0; i < 4; i++)
            {

                if ((originCurrentIndexX[i] + 1 * (int)(stepCount)) <= 0)
                {


                    for (int f = 0; f < Mathf.Abs(originCurrentIndexX[i] + 1 * (int)stepCount); f++)
                    {
                        for (int j = 0; j < Field[0].Count; j++)
                        {
                            Field[j].Insert(0, null);
                        }
                    }
                    MainCamera.transform.position = new Vector3(cameraOriginPos.x + 0.4f * stepCount, cameraOriginPos.y, cameraOriginPos.z);
                    movingObject[i].transform.position = new Vector3(originPosition[i].x + 0.4f * stepCount, originPosition[i].y);
                    Border.transform.position = new Vector3(originBorderPos.x + 0.4f * stepCount, originBorderPos.y);
                    canMoveSide = false;

                }
                else if (originCurrentIndexX[i] + 1 * (int)stepCount >= Field[0].Count - 1)
                {

                    for (int f = 0; f < originCurrentIndexX[i] + 1 * (int)stepCount - Field[0].Count - 1; f++)
                    {
                        for (int j = 0; j < 21; j++)
                        {
                            Field[j].Add(null);
                        }
                    }
                    MainCamera.transform.position = new Vector3(cameraOriginPos.x + 0.4f * stepCount, cameraOriginPos.y, cameraOriginPos.z);
                    movingObject[i].transform.position = new Vector3(originPosition[i].x + 0.4f * stepCount, originPosition[i].y);
                    currentIndexX[i] = originCurrentIndexX[i] + 1 * ((int)stepCount);
                    Border.transform.position = new Vector3(originBorderPos.x + 0.4f * stepCount, originBorderPos.y);
                    canMoveSide = false;
                }


            }
            if (shouldMove)
            {
                for (int i = 0; i < 4; i++)
                {
                    Border.transform.position = new Vector3(originBorderPos.x + 0.4f * stepCount, originBorderPos.y);
                    MainCamera.transform.position = new Vector3(cameraOriginPos.x + 0.4f * stepCount, cameraOriginPos.y, cameraOriginPos.z);
                    movingObject[i].transform.position = new Vector3(originPosition[i].x + 0.4f * stepCount, originPosition[i].y);
                    currentIndexX[i] = originCurrentIndexX[i] + 1 * ((int)stepCount);
                }
            }
            canMoveSide = true;
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y) && direction.y < -80 && shouldMove)
        {
           
            for (int i = 0; i < 4; i++)
            {
                originCurrentIndexY[i] = currentIndexY[i];
            }
            while (true)
            {
                for (int i = 0; i < 4; i++)
                {

                    if (originCurrentIndexY[i] == 20 || Field[originCurrentIndexY[i] + 1][currentIndexX[i]] != null)
                    {
                        goto End;
                    }

                    originCurrentIndexY[i]++;
                }
                moveDist++;
            }
        End:
            
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
            {
                Field[currentIndexY[i]][currentIndexX[i]] = movingObject[i];
            }
            FigureGenerator();
            shouldMove = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Time.realtimeSinceStartup - lastPress < 0.1f)
        {
            
            if (currentIndexX[0] + 1 > 9 | currentIndexX[0] - 1 < 0 || currentIndexY[0] + 1 > 20 || currentIndexY[0] - 1 < 0)
            {
                return;
            }
           
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
                if (currentIndexX[0] + 2 > 9 || currentIndexY[0] + 2 > 20 || Field[currentIndexY[0] - 1][currentIndexX[0] - 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0]] != null || Field[currentIndexY[0] + 1][currentIndexX[0] + 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] + 1] != null || Field[currentIndexY[0] + 1][currentIndexX[0]] != null || Field[currentIndexY[0] + 2][currentIndexX[0]] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] - 1] != null || Field[currentIndexY[0] + 2][currentIndexX[0] + 1] != null || Field[currentIndexY[0]][currentIndexX[0] + 2] != null
                     || Field[currentIndexY[0] + 1][currentIndexX[0] + 2] != null || Field[currentIndexY[0] + 2][currentIndexX[0] + 2] != null)
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
                    currentIndexX[3] -= 2;

                    currentIndexY[1]--;
                    currentIndexY[2]++;
                    currentIndexY[3] += 2;

                    figureState++;
                }
                else if (figureState == 2)
                {
                    movingObject[1].transform.Translate(-0.4f, -0.4f, 0f);
                    movingObject[2].transform.Translate(0.4f, 0.4f, 0f);
                    movingObject[3].transform.Translate(0.8f, 0.8f, 0f);

                    currentIndexX[1]--;
                    currentIndexX[2]++;
                    currentIndexX[3] += 2;

                    currentIndexY[1]++;
                    currentIndexY[2]--;
                    currentIndexY[3] -= 2;

                    figureState = 1;
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
                    currentIndexY[3] -= 2;

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
                    currentIndexY[3] += 2;

                    figureState++;
                }
                else if (figureState == 4)
                {
                    movingObject[1].transform.Translate(0.4f, 0.4f, 0f);
                    movingObject[2].transform.Translate(-0.4f, -0.4f, 0f);
                    movingObject[3].transform.Translate(-0.8f, 0f, 0f);

                    currentIndexX[1]++;
                    currentIndexX[2]--;
                    currentIndexX[3] -= 2;

                    currentIndexY[1]--;
                    currentIndexY[2]++;

                    figureState = 1;
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
                    currentIndexX[3] -= 2;

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
   
    private void FigureGenerator()
    {
        /*if (gameParams.GameModeIsNormal)
        {
           int objectsInRow = 0, destroyedRows = 0;     
            int count = 0;
            int middleDestroyMoveDist = 0;
            int[] destroyedRowsIndexes = new int[4];
            bool shouldDestroy = false;
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
            {
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
                for (int i = destroyedRowsIndexes[count - 1]; i >= 0; i--) //error
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

        }
        else
        {

        }
          */
        currentFigure = nextFigure;
        nextFigure = Random.Range(0, Figures.Count);
        movingObject.Clear();
       
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
            Debug.Log(currentIndexX[0]);
            movingObject.Add(Instantiate(SpawnObject[2], new Vector3(-0.2f+0.4f*(currentIndexX[0]-4), 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[2], new Vector3(-0.6f + 0.4f * (currentIndexX[0] - 4), 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[2], new Vector3(0.2f + 0.4f * (currentIndexX[0] - 4), 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[2], new Vector3(0.6f + 0.4f * (currentIndexX[0] - 4), 3.24f, 0), Quaternion.identity) as GameObject);

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
        
        for (int i = 0; i < 4; i++)
        {
            originPosition[i] = movingObject[i].transform.position;
            originCurrentIndexX[i] = currentIndexX[i];
        }
    }

    void Start()
    {
        
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

        currentIndexX[0] = 4; 

        FigureGenerator();

    }

    void FixedUpdate()
    {
        if (Time.realtimeSinceStartup >= timer)
        {
            timer = Time.realtimeSinceStartup + interval;
           
            for (int i = 0; i < movingObject.Count; i++)
            {
                if (currentIndexY[i] + 1 > 20 || Field[currentIndexY[i] + 1][currentIndexX[i]] != null)
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
                {
                    Field[currentIndexY[i]][currentIndexX[i]] = movingObject[i];
                }
                FigureGenerator();
            }

            if (interval >= 0.02)
            {
                interval -= 0.001f;
            }
        }
    }
}
