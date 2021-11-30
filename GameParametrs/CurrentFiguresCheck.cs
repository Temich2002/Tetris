using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentFiguresCheck : MonoBehaviour
{
    //При звпуске определяет что написать в кнопках
    public GameParams gameParametrs;
    public Text[] state = new Text[7];
    void Start()
    {
        if (gameParametrs.FigureT)
        {
            state[0].text = "Вкл";
        }
        else
        {
            state[0].text = "Выкл";
        }


        if (gameParametrs.FigureQ)
        {
            state[1].text = "Вкл";
        }
        else
        {
            state[1].text = "Выкл";
        }


        if (gameParametrs.FigureI)
        {
            state[2].text = "Вкл";
        }else
        {
            state[2].text = "Выкл";
        }


        if (gameParametrs.FigureZ)
        {
            state[3].text = "Вкл";
        }
        else
        {
            state[3].text = "Выкл";
        }
                    
        
        if (gameParametrs.FigureS)
        {
            state[4].text = "Вкл";
        }
        else
        {
            state[4].text = "Выкл";
        }
                    
        
        if (gameParametrs.FigureJ)
        {
            state[5].text = "Вкл";
        }
        else
        {
            state[5].text = "Выкл";
        }
                    
        
        if (gameParametrs.FigureL)
        {
            state[6].text = "Вкл";
        }
        else
        {
            state[6].text = "Выкл";
        }
    }
}
