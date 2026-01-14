using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement; // Нужно для смены сцен
using System.Collections;         // Нужно для корутин (таймера)

public class KeyboardManager : MonoBehaviour
{
    [Header("Настройки уровня")]
    public int currentLevelIndex = 1; // Номер текущего уровня (1, 2, 3...)
    public string menuSceneName = "LevelMenu"; // Название сцены со скрина

    [Header("Настройки клавиатуры")]
    public GameObject letterButtonPrefab;
    public string correctWord = "ГУМ";
    public string extraLetters = "АДНЕЯЧ";

    [Header("UI Элементы")]
    public TMP_Text resultTextField;
    public Button deleteButton;
    public Button checkButton;

    [Header("Части изображения (Маски)")]
    public GameObject[] imageParts;
    private int currentPartIndex = 0;

    private string currentInput = "";
    private bool isWon = false;

    void Start()
    {
        GenerateKeyboard();
        if (deleteButton != null) deleteButton.onClick.AddListener(DeleteLastLetter);
        if (checkButton != null) checkButton.onClick.AddListener(CheckWord);
        ResetImageParts();
    }

    void GenerateKeyboard()
    {
        List<char> allLetters = new List<char>();
        foreach (char c in correctWord) allLetters.Add(c);
        foreach (char c in extraLetters) allLetters.Add(c);

        for (int i = 0; i < allLetters.Count; i++)
        {
            char temp = allLetters[i];
            int randomIndex = Random.Range(i, allLetters.Count);
            allLetters[i] = allLetters[randomIndex];
            allLetters[randomIndex] = temp;
        }

        foreach (char letter in allLetters)
        {
            GameObject newBtn = Instantiate(letterButtonPrefab, transform);
            newBtn.GetComponentInChildren<TMP_Text>().text = letter.ToString();
            char capturedLetter = letter;
            newBtn.GetComponent<Button>().onClick.AddListener(() => SelectLetter(capturedLetter));
        }
    }

    void SelectLetter(char letter)
    {
        if (isWon) return; // Блокируем ввод, если уже выиграли
        currentInput += letter;
        UpdateUI();
    }

    void DeleteLastLetter()
    {
        if (isWon) return;
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            UpdateUI();
        }
    }

    void UpdateUI() { if (resultTextField != null) resultTextField.text = currentInput; }

    public void CheckWord()
    {
        if (isWon) return;

        if (currentInput.ToUpper() == correctWord.ToUpper())
        {
            isWon = true;
            Debug.Log("Правильно!");
            ShowAllParts();

            // СОХРАНЕНИЕ ПРОГРЕССА
            SaveProgress();

            // ЗАПУСК ТАЙМЕРА НА 4 СЕКУНДЫ
            StartCoroutine(WaitAndLoadMenu());
        }
        else
        {
            ShowNextPart();
            currentInput = "";
            UpdateUI();
        }
    }

    void SaveProgress()
    {
        // 1. Помечаем ТЕКУЩИЙ уровень (например, 2) как пройденный (золотой)
        PlayerPrefs.SetInt("Level_" + currentLevelIndex + "_Status", 1);

        // 2. Вычисляем индекс СЛЕДУЮЩЕГО уровня (например, 2 + 1 = 3)
        int nextLevel = currentLevelIndex + 1;

        // 3. Если 3-й уровень еще не пройден, ставим ему статус 2 (разблокирован)
        if (PlayerPrefs.GetInt("Level_" + nextLevel + "_Status", 0) == 0)
        {
            PlayerPrefs.SetInt("Level_" + nextLevel + "_Status", 2);
        }
        PlayerPrefs.Save(); // Сохраняем данные на диск
    }


    IEnumerator WaitAndLoadMenu()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(menuSceneName);
    }

    void ShowAllParts()
    {
        foreach (GameObject part in imageParts) if (part != null) part.SetActive(true);
        currentPartIndex = imageParts.Length;
    }

    void ShowNextPart()
    {
        if (currentPartIndex < imageParts.Length)
        {
            imageParts[currentPartIndex].SetActive(true);
            currentPartIndex++;
        }
    }

    void ResetImageParts()
    {
        for (int i = 0; i < imageParts.Length; i++)
        {
            if (imageParts[i] != null) imageParts[i].SetActive(i == 0);
        }
        currentPartIndex = 1;
    }
}