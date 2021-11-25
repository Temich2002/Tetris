using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SaveInfo : MonoBehaviour
{
    public GameParams gameParametrs;
    public Text state;
    
    public void ChangeMode(int mode) 
    {
            if (mode == 1)
            {
                gameParametrs.GameModeIsNormal = true; //Обычный режим
            }
            else 
            {
                gameParametrs.GameModeIsNormal = false; //Хаотичный
            }   
    }
    public void ChangeAnabledFigure(int figure) //доступные и недоступные фигурки
    {
            switch (figure)
            {
                case 0:
                if (gameParametrs.FigureT)
                {
                    gameParametrs.FigureT = false;
                    state.text = "Выкл";
                }
                else 
                { 
                    gameParametrs.FigureT = true;
                    state.text = "Вкл";
                }

                break;

                case 1:
                    if (gameParametrs.FigureQ)
                    {
                        gameParametrs.FigureQ = false;
                        state.text = "Выкл";
                    }
                    else 
                    { 
                        gameParametrs.FigureQ = true;
                        state.text = "Вкл";
                    }
                    break;

                case 2:
                    if (gameParametrs.FigureI)
                    {
                        gameParametrs.FigureI = false;
                        state.text = "Выкл";
                    }
                    else 
                    { 
                        gameParametrs.FigureI = true;
                        state.text = "Вкл";
                    }
                    break;

                case 3:
                    if (gameParametrs.FigureZ)
                    {
                        gameParametrs.FigureZ = false;
                        state.text = "Выкл";
                    }
                    else 
                    { 
                        gameParametrs.FigureZ = true;
                        state.text = "Вкл";
                    }
                    break;

                case 4:
                    if (gameParametrs.FigureS)
                    {
                        gameParametrs.FigureS = false;
                        state.text = "Выкл";
                    }
                    else 
                    { 
                        gameParametrs.FigureS = true;
                        state.text = "Вкл";
                    }
                    break;

                case 5:
                    if (gameParametrs.FigureJ)
                    {
                        gameParametrs.FigureJ = false;
                        state.text = "Выкл";
                    }
                    else 
                    { 
                        gameParametrs.FigureJ = true;
                        state.text = "Вкл";
                    }
                    break;

                default:
                    if (gameParametrs.FigureL)
                    {
                        gameParametrs.FigureL = false;
                        state.text = "Выкл";
                    }
                    else 
                    { 
                        gameParametrs.FigureL = true;
                        state.text = "Вкл";
                    }
                    break;
            } 
    }
    
}
