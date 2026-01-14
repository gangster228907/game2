using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovementScript : MonoBehaviour
{
    [Header("Ссылки на объекты")]
    public Transform playerTransform;
    public Tilemap floorTilemap;
    public Tilemap obstaclesTilemap;

    [Header("Настройки")]
    public float minSwipeDistance = 30f;

    private Vector2 touchStart;
    private Vector2 touchEnd;
    private bool isInputDetected = false;

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // 1. Сначала проверяем РЕАЛЬНЫЙ ТАЧ (телефон)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) touchStart = touch.position;
            if (touch.phase == TouchPhase.Ended)
            {
                touchEnd = touch.position;
                ProcessMovement();
            }
            return; // Если есть тач, мышь игнорируем
        }

        // 2. Если тача нет, проверяем МЫШЬ (ПК)
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Input.mousePosition;
            isInputDetected = true;
        }
        if (Input.GetMouseButtonUp(0) && isInputDetected)
        {
            touchEnd = Input.mousePosition;
            isInputDetected = false;
            ProcessMovement();
        }
    }

    void ProcessMovement()
    {
        Vector2 delta = touchEnd - touchStart;
        if (delta.magnitude < minSwipeDistance) return;

        Vector3Int direction = Vector3Int.zero;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            direction = delta.x > 0 ? Vector3Int.right : Vector3Int.left;
        else
            direction = delta.y > 0 ? Vector3Int.up : Vector3Int.down;

        MovePlayer(direction);
    }

    void MovePlayer(Vector3Int direction)
    {
        if (!playerTransform) return;

        // Определяем текущую клетку СТРОГО по позиции мальчика
        Vector3Int currentCell = floorTilemap.WorldToCell(playerTransform.position);
        Vector3Int targetCell = currentCell + direction;

        // Проверка: есть ли там трава (Floor)?
        if (!floorTilemap.HasTile(targetCell))
        {
            Debug.Log("Край карты!");
            return;
        }

        // Проверка: нет ли там препятствия?
        if (obstaclesTilemap.HasTile(targetCell))
        {
            Debug.Log("Препятствие!");
            return;
        }

        // Прыгаем ровно в центр следующей клетки
        Vector3 targetPos = floorTilemap.GetCellCenterWorld(targetCell);
        playerTransform.position = new Vector3(targetPos.x, targetPos.y, playerTransform.position.z);
    }
}
