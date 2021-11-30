using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GameChaos : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    Vector2 origin;                                 // Позиция пальца при нажатии
    Vector2 direction;                              // Направление пальца при перетягивании
    Vector2[] originPosition = new Vector2[4];      // Позиция движущегося объекта в момент нажатия пальцем


    public Camera MainCamera;
    public Text scor;

    List<int> Figures = new List<int>();                            // Список доступных фигурок
    List<GameObject> movingObject = new List<GameObject>();         // Список опускаемых объектов которые образуют фигурку     
    List<GameObject> nextObject = new List<GameObject>();           // Следующая фигурка
    List<List<GameObject>> Field = new List<List<GameObject>>();    // Массив ячеек для блоков(не содержит movingObject)

    public GameObject[] SpawnObject = new GameObject[7];            // Массив объектов которые будут использоватся для построения фигурок    
    public GameObject Border;

    float timer;                      // Момент времени срабатывания следующего опускания фигурки
    float interval = 0.5f;            // Интервал между опусканием фигурки
    float step = 80f;                 // Эта переменная определяет насколько сильно нужно отодвинуть палец от места нажатия чтобы фигурка сдвинулась с места  
    float lastPress;                  // Время нажатия на экран
    float stepCount;                  // количество перемещений
    float xPos = -0.2f;               // текущая позиция центрального кубика движущегося обьекта

    int LastIndex = 50010, firstIndex =49090;   // Индексы самого левого и самого правого существующего объекта в массиве
    int currentFigure, nextFigure;              // Зарандомленные текущая и следующая фигурки
    int figureState, figureType,figureTypeNext; // Тип фигуры и в каком повороте они находятся чтобы позже двигать составляющие фигурки       
    int[] currentIndexX = new int[4];           // Массив индексов х позиций каждого блока в массиве Field 
    int[] currentIndexY = new int[4];           // Массив индексов y позиций каждого блока в массиве Field 
    int[] originCurrentIndexX = new int[4];     // Массивы индексов позиций относительно которых происходят все изменения
    int[] originCurrentIndexY = new int[4];     //
    int moveDist = 0;                           // Дистанция на которую может и должна опуститься фигурка при проведении пальцем вниз
    int score = 0;                              // Счёт

    public GameParams gameParams;      // Список доступных фигурок и текущего режима        

    bool canMoveDown;   // Может ли фигурка двигатся дальше вниз
    bool canMoveSide;   // Может ли фигурка двигатся в одну из сторон
    bool shouldMove;    // Предотвращает множественное срабатывание скрипта отпускание

    // Записывает все данные о фигурке в момент нажатия
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
        // Определяет Количество перемещений и их направление
        Vector2 currentPos = eventData.position;
        direction = currentPos - origin;
        stepCount = Mathf.Floor(direction.x / step);
        // Проверка на наличие препятствий на пути к точке перемещния
        for (int i = 0; i < movingObject.Count; i++)
          { 
            if (currentIndexX[i] + (int)stepCount < 0 || currentIndexX[i] + (int)stepCount > Field[0].Count)
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
            // Перемещние фигурки и камеры вправо или влево
            for (int i = 0; i < 4; i++)
            {
                movingObject[i].transform.position = new Vector3(originPosition[i].x + 0.4f * stepCount, originPosition[i].y);
                currentIndexX[i] = originCurrentIndexX[i] + 1 * ((int)stepCount);

            }
            // Периодически в игре появлялся баг где movingObject не двигался а камера и остальные объекты двигались 
            // бага я смог избежать только устанавливая координаты относительно текущей позиции мовингОбжект
            nextObject[0].transform.position = new Vector3(movingObject[0].transform.position.x, nextObject[0].transform.position.y);
            if (figureTypeNext == 1)
            {
                nextObject[1].transform.position = new Vector3(nextObject[0].transform.position.x-0.4f, nextObject[1].transform.position.y);
                nextObject[2].transform.position = new Vector3(nextObject[0].transform.position.x+0.4f, nextObject[2].transform.position.y);
                nextObject[3].transform.position = new Vector3(nextObject[0].transform.position.x, nextObject[3].transform.position.y);
            }
            else if (figureTypeNext == 2)
            {
                nextObject[1].transform.position = new Vector3(nextObject[0].transform.position.x+0.4f, nextObject[1].transform.position.y);
                nextObject[2].transform.position = new Vector3(nextObject[0].transform.position.x, nextObject[2].transform.position.y);
                nextObject[3].transform.position = new Vector3(nextObject[0].transform.position.x+0.4f, nextObject[3].transform.position.y);
            }
            else if (figureTypeNext == 3)
            {
                nextObject[1].transform.position = new Vector3(nextObject[0].transform.position.x-0.4f, nextObject[1].transform.position.y);
                nextObject[2].transform.position = new Vector3(nextObject[0].transform.position.x+ 0.4f, nextObject[2].transform.position.y);
                nextObject[3].transform.position = new Vector3(nextObject[0].transform.position.x+ 0.8f, nextObject[3].transform.position.y);
            }
            else if (figureTypeNext == 4)
            {
                nextObject[1].transform.position = new Vector3(nextObject[0].transform.position.x-0.4f, nextObject[1].transform.position.y);
                nextObject[2].transform.position = new Vector3(nextObject[0].transform.position.x, nextObject[2].transform.position.y);
                nextObject[3].transform.position = new Vector3(nextObject[0].transform.position.x+ 0.4f, nextObject[3].transform.position.y);
            }
            else if (figureTypeNext == 5)
            {
                nextObject[1].transform.position = new Vector3(nextObject[0].transform.position.x+ 0.4f, nextObject[1].transform.position.y);
                nextObject[2].transform.position = new Vector3(nextObject[0].transform.position.x, nextObject[2].transform.position.y);
                nextObject[3].transform.position = new Vector3(nextObject[0].transform.position.x- 0.4f, nextObject[3].transform.position.y);
            }
            else if (figureTypeNext == 6)
            {
                nextObject[1].transform.position = new Vector3(nextObject[0].transform.position.x, nextObject[1].transform.position.y);
                nextObject[2].transform.position = new Vector3(nextObject[0].transform.position.x, nextObject[2].transform.position.y);
                nextObject[3].transform.position = new Vector3(nextObject[0].transform.position.x- 0.4f, nextObject[3].transform.position.y);
            }
            else if (figureTypeNext == 7)
            {
                nextObject[1].transform.position = new Vector3(nextObject[0].transform.position.x, nextObject[1].transform.position.y);
                nextObject[2].transform.position = new Vector3(nextObject[0].transform.position.x, nextObject[2].transform.position.y);
                nextObject[3].transform.position = new Vector3(nextObject[0].transform.position.x+ 0.4f, nextObject[3].transform.position.y);
            }

            Border.transform.position = new Vector3(movingObject[0].transform.position.x + 0.2f, Border.transform.position.y);
            MainCamera.transform.position = new Vector3(movingObject[0].transform.position.x + 0.2f, MainCamera.transform.position.y, MainCamera.transform.position.z);
            // Отключает или включает прорисовку объектов которые находятся вне или внутри камеры
            for (int j = 0; j < 21; j++)
            {
                for (int i = currentIndexX[0] - 15; i < currentIndexX[0] - 10; i++)
                {
                    if (Field[j][i] != null)
                    {
                        Field[j][i].SetActive(false);
                    }
                }
                for (int i = currentIndexX[0] + 10; i < currentIndexX[0] + 15; i++)
                {
                    if (Field[j][i] != null)
                    {
                        Field[j][i].SetActive(false);
                    }
                }
                for (int i = currentIndexX[0] - 10; i < currentIndexX[0] + 10; i++)
                {
                    if (Field[j][i] != null)
                    {
                        Field[j][i].SetActive(true);
                    }
                }
            }
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y) && direction.y < -240 && shouldMove)
        {
            // Определяем расстояние на которое нужно опустить фигурку
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
            // Опускаем фигурку
            for (int i = 0; i < 4; i++)
            {
                if (movingObject[i] != null)
                {
                    movingObject[i].transform.Translate(0f, -0.4f * moveDist, 0f);
                    currentIndexY[i] += moveDist;
                }
            }
            moveDist = 0;
            xPos = movingObject[0].transform.position.x;
            for (int i = 0; i < movingObject.Count; i++)
            { // Проверка на наличие объектов в точках спавна и последующее поражение(уничтожение всех обьектов) если 
              // в месте где должна появится фигурка уже есть объект
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
                        for (int j = firstIndex; j <= LastIndex; j++)
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
            for (int n = firstIndex; n <= LastIndex; n++)
            {// Проверка на наличие объектов в первой строке и последующее поражение(уничтожение всех обьектов)
                if (Field[0][n] != null)
                {
                    interval = 0.5f;
                    score = 0;
                    scor.text = score.ToString();
                    for (int k = 0; k < 21; k++)
                    {
                        for (int j = firstIndex; j < LastIndex; j++)
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

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Time.realtimeSinceStartup - lastPress < 0.1f)
        {
            // Если центральный кубик(который никогда не двигается при повороте и является центром фигурки) находится впритык к границам не поворачивать
            if (figureType == 1)
            {


                if (Field[currentIndexY[0] + 1][currentIndexX[0]] != null || Field[currentIndexY[0] - 1][currentIndexX[0]] != null || Field[currentIndexY[0] + 1][currentIndexX[0] + 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] + 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0] + 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0] - 1] != null ||
                     Field[currentIndexY[0]][currentIndexX[0] - 1] != null || Field[currentIndexY[0] + 1][currentIndexX[0] - 1] != null)
                {
                    return;
                }
                // Определяет какая фигурка в какой позиции сейчас находится и меняет её координаты предварительно проверив нет ли на пути поворота препятствий
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
                if (currentIndexX[0] + 2 > Field[0].Count|| currentIndexX[0] -2 < 0 || currentIndexY[0] + 2 > 20|| currentIndexY[0] - 1 <0 || Field[currentIndexY[0] - 1][currentIndexX[0] - 1] != null || Field[currentIndexY[0] - 1][currentIndexX[0]] != null || Field[currentIndexY[0] + 1][currentIndexX[0] + 1] != null ||
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
    {   // Если движущийся объект существует(а также эта функция вызывается только в начале и когда приземляется прошлая фигурка) 
        // обозначаем границы массива 
        if (movingObject.Count > 0)
        {   //
            int max = 0;
            int min = 0;
            for (int i = 0; i < 4; i++)
            {
                if (max < currentIndexX[i])
                {
                    max = currentIndexX[i];
                }
                if (min > currentIndexX[i])
                {
                    min = currentIndexX[i];
                }
            }
            if (min < firstIndex)
            {
                firstIndex = min;
            }
            if (max > LastIndex)
            {
                LastIndex = max;
            }
            // проверяем какие столбци нужно уничтожить
            int objectsInRow = 0; //Подсчёт обьектов в строке
            int rowsDestroyed = 0;//Уничтожаемые строки
            List<int> localDestroyIndexesX = new List<int>();   // При первом проходе все индексы сохраняются в local 
            List<int> localDestroyIndexesY = new List<int>();   // после если строка уничтожается индексы записываются в
            List<int> globalDestroyIndexesX = new List<int>();  // global
            List<int> globalDestroyIndexesY = new List<int>();
            List<GameObject> mustDestroyObjects = new List<GameObject>(); // объекты подлежащие уничтожению
            // уничтожаем нужные объекты и записываем их индексы после чего перемещение будет не по строкам а по столбцам
            for (int j = 0; j < 21; j++)
            {
                for (int i = firstIndex; i <= LastIndex; i++)
                {
                    if (Field[j][i] != null)
                    {
                        localDestroyIndexesY.Add(j);
                        localDestroyIndexesX.Add(i);
                        mustDestroyObjects.Add(Field[j][i]);
                        objectsInRow++;
                    }
                    else if (objectsInRow >= 10)
                    {
                        for (int f = 0; f < mustDestroyObjects.Count; f++)
                        {
                            globalDestroyIndexesX.Add(localDestroyIndexesX[f]);
                            globalDestroyIndexesY.Add(localDestroyIndexesY[f]);
                            Destroy(mustDestroyObjects[f]);
                        }
                        objectsInRow = 0;
                        mustDestroyObjects.Clear();
                        localDestroyIndexesX.Clear();
                        localDestroyIndexesY.Clear();
                        rowsDestroyed++;
                    }
                    else
                    {
                        localDestroyIndexesX.Clear();
                        localDestroyIndexesY.Clear();
                        objectsInRow = 0;
                        mustDestroyObjects.Clear();
                    }
                }
                if (objectsInRow >= 10)
                {
                    for (int f = 0; f < mustDestroyObjects.Count; f++)
                    {
                        globalDestroyIndexesX.Add(localDestroyIndexesX[f]);
                        globalDestroyIndexesY.Add(localDestroyIndexesY[f]);
                        Destroy(mustDestroyObjects[f]);
                    }
                    objectsInRow = 0;
                    mustDestroyObjects.Clear();
                    localDestroyIndexesX.Clear();
                    localDestroyIndexesY.Clear();
                    rowsDestroyed++;
                }
                else
                {
                    localDestroyIndexesX.Clear();
                    localDestroyIndexesY.Clear();
                    objectsInRow = 0;
                    mustDestroyObjects.Clear();
                }
            }
            // если какая то строка уничтожается смещаем элементы в стобцах
            if (globalDestroyIndexesX.Count > 0)
            {
                List<List<int>> colons = new List<List<int>>(); // y индексы в столбце
                List<int> colonIndexX = new List<int>();        // х индекс каждого столбика
                colons.Add(new List<int>());
                colons[0].Add(globalDestroyIndexesY[0]);
                colonIndexX.Add(globalDestroyIndexesX[0]);
                int count = 1;
                bool xAdding = false;//Нужно ли добавлять новый столбец
                //формируем столбци
                for (int i = 1; i < globalDestroyIndexesX.Count; i++)
                {

                    for (int f = 0; f < colonIndexX.Count; f++)
                    {
                        if (colonIndexX[f] == globalDestroyIndexesX[i])
                        {
                            colons[f].Add(globalDestroyIndexesY[i]);
                            xAdding = false;
                            break;
                        }
                        else
                        {
                            xAdding = true;
                        }
                    }
                    if (xAdding)
                    {

                        colonIndexX.Add(globalDestroyIndexesX[i]);
                        colons.Add(new List<int>());
                        colons[count].Add(globalDestroyIndexesY[i]);
                        count++;
                    }
                }
                //перемещаем элементы в столбцах
                int middleMoveDist = 0;
                for (int i = 0; i < colonIndexX.Count; i++)
                {
                    if (colons[i].Count > 1)
                    {
                        colons[i].Sort();
                        for (int f = colons[i].Count - 1; f > 0; f--)
                        {
                            middleMoveDist++;
                            if (colons[i][f] - colons[i][f - 1] > 2)
                            {
                                if (Field[colons[i][f] - 1][colonIndexX[i]] != null)
                                {
                                    Field[colons[i][f]][colonIndexX[i]] = Field[colons[i][f] - 1][colonIndexX[i]];
                                    Field[colons[i][f] - 1][colonIndexX[i]].transform.Translate(0f, -0.4f, 0f);
                                    Field[colons[i][f] - 1][colonIndexX[i]] = null;
                                }
                                if (Field[colons[i][f] - 2][colonIndexX[i]] != null)
                                {
                                    Field[colons[i][f] - 1][colonIndexX[i]] = Field[colons[i][f] - 2][colonIndexX[i]];
                                    Field[colons[i][f] - 2][colonIndexX[i]].transform.Translate(0f, -0.4f, 0f);
                                    Field[colons[i][f] - 2][colonIndexX[i]] = null;
                                }
                            }
                            else if (colons[i][f] - colons[i][f - 1] > 1)
                            {
                                if (Field[colons[i][f] - 1][colonIndexX[i]] != null)
                                {
                                    Field[colons[i][f] + middleMoveDist - 1][colonIndexX[i]] = Field[colons[i][f] - 1][colonIndexX[i]];
                                    Field[colons[i][f] - 1][colonIndexX[i]].transform.Translate(0f, -0.4f * middleMoveDist, 0f);
                                    Field[colons[i][f] - 1][colonIndexX[i]] = null;
                                }
                            }
                        }
                        for (int f = colons[i][0]; f > 0; f--)
                        {
                            if (Field[f - 1][colonIndexX[i]] != null)
                            {
                                Field[f + rowsDestroyed - 1][colonIndexX[i]] = Field[f - 1][colonIndexX[i]];
                                Field[f - 1][colonIndexX[i]].transform.Translate(0f, -0.4f * rowsDestroyed, 0f);
                                Field[f - 1][colonIndexX[i]] = null;
                            }
                        }
                    }
                    else if (colons[i].Count == 1)
                    {

                        for (int f = colons[i][0]; f > 0; f--)
                        {
                            if (Field[f - 1][colonIndexX[i]] != null)
                            {
                                Field[f][colonIndexX[i]] = Field[f - 1][colonIndexX[i]];
                                Field[f - 1][colonIndexX[i]].transform.Translate(0f, -0.4f, 0f);
                                Field[f - 1][colonIndexX[i]] = null;
                            }
                        }
                    }
                    middleMoveDist = 0;
                }
            }
            if (rowsDestroyed == 1)
            {
                score += 100;
                scor.text = score.ToString();
            }
            else if (rowsDestroyed == 2)
            {
                score += 300;
                scor.text = score.ToString();
            }
            else if (rowsDestroyed == 3)
            {
                score += 700;
                scor.text = score.ToString();
            }
            else if (rowsDestroyed == 4)
            {
                score += 1000;
                scor.text = score.ToString();
            }
        }
        // генерация нового объекта
        currentFigure = nextFigure;
        nextFigure = Random.Range(0, Figures.Count);
        movingObject.Clear();
        for (int i = 0; i < nextObject.Count; i++)
        {
            Destroy(nextObject[i]);
        }
        nextObject.Clear();
        if (Figures[nextFigure] == 1)
        {
            figureTypeNext = 1;

            nextObject.Add(Instantiate(SpawnObject[0], new Vector3(xPos, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[0], new Vector3(xPos - 0.4f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[0], new Vector3(xPos + 0.4f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[0], new Vector3(xPos, 3.84f, 0), Quaternion.identity) as GameObject);
        }
        else if (Figures[nextFigure] == 2)
        {
            figureTypeNext = 2;

            nextObject.Add(Instantiate(SpawnObject[1], new Vector3(xPos, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[1], new Vector3(xPos + 0.4f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[1], new Vector3(xPos, 3.84f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[1], new Vector3(xPos + 0.4f, 3.84f, 0), Quaternion.identity) as GameObject);
        }
        else if (Figures[nextFigure] == 3)
        {
            figureTypeNext = 3;

            nextObject.Add(Instantiate(SpawnObject[2], new Vector3(xPos, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[2], new Vector3(xPos - 0.4f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[2], new Vector3(xPos + 0.4f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[2], new Vector3(xPos + 0.8f, 4.24f, 0), Quaternion.identity) as GameObject);
        }
        else if (Figures[nextFigure] == 4)
        {
            figureTypeNext = 4;

            nextObject.Add(Instantiate(SpawnObject[3], new Vector3(xPos, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[3], new Vector3(xPos - 0.4f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[3], new Vector3(xPos, 3.84f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[3], new Vector3(xPos + 0.4f, 3.84f, 0), Quaternion.identity) as GameObject);
        }
        else if (Figures[nextFigure] == 5)
        {
            figureTypeNext = 5;

            nextObject.Add(Instantiate(SpawnObject[4], new Vector3(xPos, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[4], new Vector3(xPos + 0.4f, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[4], new Vector3(xPos, 3.84f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[4], new Vector3(xPos - 0.4f, 3.84f, 0), Quaternion.identity) as GameObject);
        }
        else if (Figures[nextFigure] == 6)
        {
            figureTypeNext = 6;

            nextObject.Add(Instantiate(SpawnObject[5], new Vector3(xPos, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[5], new Vector3(xPos, 4.64f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[5], new Vector3(xPos, 3.84f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[5], new Vector3(xPos - 0.4f, 3.84f, 0), Quaternion.identity) as GameObject);
        }
        else if (Figures[nextFigure] == 7)
        {
            figureTypeNext = 7;

            nextObject.Add(Instantiate(SpawnObject[6], new Vector3(xPos, 4.24f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[6], new Vector3(xPos, 4.64f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[6], new Vector3(xPos, 3.84f, 0), Quaternion.identity) as GameObject);
            nextObject.Add(Instantiate(SpawnObject[6], new Vector3(xPos + 0.4f, 3.84f, 0), Quaternion.identity) as GameObject);
        }
        
        if (Figures[currentFigure] == 1)
        {
            figureState = 1;
            figureType = 1;

            movingObject.Add(Instantiate(SpawnObject[0], new Vector3(xPos, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[0], new Vector3(xPos - 0.4f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[0], new Vector3(xPos+0.4f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[0], new Vector3(xPos, 2.84f, 0), Quaternion.identity) as GameObject);

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

            movingObject.Add(Instantiate(SpawnObject[1], new Vector3(xPos, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[1], new Vector3(xPos+0.4f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[1], new Vector3(xPos, 2.84f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[1], new Vector3(xPos+0.4f, 2.84f, 0), Quaternion.identity) as GameObject);

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
            movingObject.Add(Instantiate(SpawnObject[2], new Vector3(xPos, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[2], new Vector3(xPos-0.4f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[2], new Vector3(xPos+0.4f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[2], new Vector3(xPos + 0.8f, 3.24f, 0), Quaternion.identity) as GameObject);

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

            movingObject.Add(Instantiate(SpawnObject[3], new Vector3(xPos, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[3], new Vector3(xPos - 0.4f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[3], new Vector3(xPos, 2.84f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[3], new Vector3(xPos+0.4f, 2.84f, 0), Quaternion.identity) as GameObject);

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

            movingObject.Add(Instantiate(SpawnObject[4], new Vector3(xPos, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[4], new Vector3(xPos+0.4f, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[4], new Vector3(xPos, 2.84f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[4], new Vector3(xPos - 0.4f, 2.84f, 0), Quaternion.identity) as GameObject);

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

            movingObject.Add(Instantiate(SpawnObject[5], new Vector3(xPos, 2.84f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[5], new Vector3(xPos, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[5], new Vector3(xPos, 2.44f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[5], new Vector3(xPos-0.4f, 2.44f, 0), Quaternion.identity) as GameObject);

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

            movingObject.Add(Instantiate(SpawnObject[6], new Vector3(xPos, 2.84f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[6], new Vector3(xPos, 3.24f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[6], new Vector3(xPos, 2.44f, 0), Quaternion.identity) as GameObject);
            movingObject.Add(Instantiate(SpawnObject[6], new Vector3(xPos+0.4f, 2.44f, 0), Quaternion.identity) as GameObject);

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
        //Добавление доступных фигурок
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
        // генерация массива из 100000 элементов
        for (int i = 0; i < 21; i++)
        {
            Field.Add(new List<GameObject>());
            for (int j = 0; j < 100000; j++)
            {
                Field[i].Add(null);
            }
        }

        timer = Time.realtimeSinceStartup + interval;
        nextFigure = Random.Range(0, Figures.Count);

        currentIndexX[0] = 50000;


        FigureGenerator();
        
    }

    void FixedUpdate()
    {
        if (Time.realtimeSinceStartup >= timer)
        {
            timer = Time.realtimeSinceStartup + interval;
            //Проверка на то можно ли опустить фигурку
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
                xPos = movingObject[0].transform.position.x;
                for (int i = 0; i < movingObject.Count; i++)
                {   // Проверка на поражение
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
                            for (int j = firstIndex; j <= LastIndex; j++)
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
                    for (int n = firstIndex; n <= LastIndex; n++)
                    {
                        if (Field[0][n] != null)
                        {
                            interval = 0.5f;
                            score = 0;
                            scor.text = score.ToString();
                            for (int k = 0; k < 21; k++)
                            {
                                for (int j = firstIndex; j < LastIndex; j++)
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

            if (interval >= 0.02)
            {
                interval -= 0.001f;
            }
        }
    }
}
