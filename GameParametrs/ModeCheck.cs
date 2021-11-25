using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeCheck : MonoBehaviour
{
    //Пишем текущий режим
    public GameParams gameParams;
    public Text text;
    void Start()
    {
        if (gameParams.GameModeIsNormal)
        {
            text.text = "Текущий режим: Нормальный";
        }
        else
        {
            text.text = "Текущий режим: Хаотичный";
        }
    }
    public void ModeChange() 
    {
        if (gameParams.GameModeIsNormal)
        {
            text.text = "Текущий режим: Нормальный";
        }
        else
        {
            text.text = "Текущий режим: Хаотичный";
        }
    }
}
