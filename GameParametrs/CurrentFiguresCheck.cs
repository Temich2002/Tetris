using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentFiguresCheck : MonoBehaviour
{
    //��� ������� ���������� ��� �������� � �������
    public GameParams gameParametrs;
    public Text[] state = new Text[7];
    void Start()
    {
        if (gameParametrs.FigureT)
        {
            state[0].text = "���";
        }
        else
        {
            state[0].text = "����";
        }


        if (gameParametrs.FigureQ)
        {
            state[1].text = "���";
        }
        else
        {
            state[1].text = "����";
        }


        if (gameParametrs.FigureI)
        {
            state[2].text = "���";
        }else
        {
            state[2].text = "����";
        }


        if (gameParametrs.FigureZ)
        {
            state[3].text = "���";
        }
        else
        {
            state[3].text = "����";
        }
                    
        
        if (gameParametrs.FigureS)
        {
            state[4].text = "���";
        }
        else
        {
            state[4].text = "����";
        }
                    
        
        if (gameParametrs.FigureJ)
        {
            state[5].text = "���";
        }
        else
        {
            state[5].text = "����";
        }
                    
        
        if (gameParametrs.FigureL)
        {
            state[6].text = "���";
        }
        else
        {
            state[6].text = "����";
        }
    }
}
