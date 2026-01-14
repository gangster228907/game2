using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public Slider loadingSlider;
    public float speed = 0.5f; // Скорость заполнения

    void Update()
    {
        if (loadingSlider.value < 1f)
        {
            // Плавно прибавляем значение
            loadingSlider.value += speed * Time.deltaTime;
        }
        else
        {
            SceneManager.LoadScene("MiniGameScene");
        }
    }
}