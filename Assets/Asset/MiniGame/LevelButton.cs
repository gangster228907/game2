
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int levelNumber; // Поставить в инспекторе (1, 2, 3...)
    public Button button;
    public Image iconImage; // Сама звезда

    public Color lockedColor = Color.gray;
    public Color availableColor = Color.white;
    public Color completedColor = Color.yellow; // Золотой

    void Start()
    {
        // По умолчанию 1-й уровень всегда доступен
        if (levelNumber == 1 && PlayerPrefs.GetInt("Level_1_Status", 0) == 0)
        {
            PlayerPrefs.SetInt("Level_1_Status", 2); // Сделать доступным
        }

        int status = PlayerPrefs.GetInt("Level_" + levelNumber + "_Status", 0);
        // 0 - закрыто, 1 - пройдено (золото), 2 - доступно (белый)

        if (status == 1) // Пройден
        {
            button.interactable = true;
            iconImage.color = completedColor;
        }
        else if (status == 2) // Доступен
        {
            button.interactable = true;
            iconImage.color = availableColor;
        }
        else // Закрыт
        {
            button.interactable = false;
            iconImage.color = lockedColor;
        }
    }

    public void NextLvl1()
    {
        SceneManager.LoadScene("21GameScene");
    }

    public void NextLvl2()
    {
        SceneManager.LoadScene("211GameScene");
    }

    public void NextLvl3()
    {
        SceneManager.LoadScene("2111GameScene");
    }


}