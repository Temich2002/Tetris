using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{//Загрузка выбранной сцены
    public GameParams gameParams;
    public void SceneLoad(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void PlaySceneLoad()
    {//Загрузка сцены с нужным режимом для игры
        if (gameParams.GameModeIsNormal)
        {
            SceneManager.LoadScene(1);
        }
        else { SceneManager.LoadScene(2); }
    }
}
