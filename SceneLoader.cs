using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{//�������� ��������� �����
    public GameParams gameParams;
    public void SceneLoad(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void PlaySceneLoad()
    {//�������� ����� � ������ ������� ��� ����
        if (gameParams.GameModeIsNormal)
        {
            SceneManager.LoadScene(1);
        }
        else { SceneManager.LoadScene(2); }
    }
}
