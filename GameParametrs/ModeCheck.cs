using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeCheck : MonoBehaviour
{
    //����� ������� �����
    public GameParams gameParams;
    public Text text;
    void Start()
    {
        if (gameParams.GameModeIsNormal)
        {
            text.text = "������� �����: ����������";
        }
        else
        {
            text.text = "������� �����: ���������";
        }
    }
    public void ModeChange() 
    {
        if (gameParams.GameModeIsNormal)
        {
            text.text = "������� �����: ����������";
        }
        else
        {
            text.text = "������� �����: ���������";
        }
    }
}
