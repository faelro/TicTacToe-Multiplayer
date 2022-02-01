using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoginScene()
    {
        SceneManager.LoadScene("LoginRegisterScene");
    }
    public void rankingScene()
    {
        SceneManager.LoadScene("RankingScene");
    }
    public void gameScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
