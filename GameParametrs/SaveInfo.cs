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
                gameParametrs.GameModeIsNormal = true; //������� �����
            }
            else 
            {
                gameParametrs.GameModeIsNormal = false; //���������
            }   
    }
    public void ChangeAnabledFigure(int figure) //��������� � ����������� �������
    {
            switch (figure)
            {
                case 0:
                if (gameParametrs.FigureT)
                {
                    gameParametrs.FigureT = false;
                    state.text = "����";
                }
                else 
                { 
                    gameParametrs.FigureT = true;
                    state.text = "���";
                }

                break;

                case 1:
                    if (gameParametrs.FigureQ)
                    {
                        gameParametrs.FigureQ = false;
                        state.text = "����";
                    }
                    else 
                    { 
                        gameParametrs.FigureQ = true;
                        state.text = "���";
                    }
                    break;

                case 2:
                    if (gameParametrs.FigureI)
                    {
                        gameParametrs.FigureI = false;
                        state.text = "����";
                    }
                    else 
                    { 
                        gameParametrs.FigureI = true;
                        state.text = "���";
                    }
                    break;

                case 3:
                    if (gameParametrs.FigureZ)
                    {
                        gameParametrs.FigureZ = false;
                        state.text = "����";
                    }
                    else 
                    { 
                        gameParametrs.FigureZ = true;
                        state.text = "���";
                    }
                    break;

                case 4:
                    if (gameParametrs.FigureS)
                    {
                        gameParametrs.FigureS = false;
                        state.text = "����";
                    }
                    else 
                    { 
                        gameParametrs.FigureS = true;
                        state.text = "���";
                    }
                    break;

                case 5:
                    if (gameParametrs.FigureJ)
                    {
                        gameParametrs.FigureJ = false;
                        state.text = "����";
                    }
                    else 
                    { 
                        gameParametrs.FigureJ = true;
                        state.text = "���";
                    }
                    break;

                default:
                    if (gameParametrs.FigureL)
                    {
                        gameParametrs.FigureL = false;
                        state.text = "����";
                    }
                    else 
                    { 
                        gameParametrs.FigureL = true;
                        state.text = "���";
                    }
                    break;
            } 
    }
    
}
