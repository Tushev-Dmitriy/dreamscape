using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomObject : MonoBehaviour
{
    public Transform target; // Цель (персонаж)
    public Transform head; // Точка фокусировки (голова персонажа)
    public float zoomSpeed = 2.0f; // Скорость изменения зума
    public float minDistance = 2.0f; // Минимальное расстояние от камеры до персонажа
    public float maxDistance = 10.0f; // Максимальное расстояние от камеры до персонажа

    private float currentDistance; // Текущее расстояние от камеры до персонажа
    private Vector3 offset; // Смещение камеры

    private Vector3 startPosition;

    void Start()
    {
        currentDistance = Vector3.Distance(transform.position, target.position); // Начальное расстояние
        offset = transform.position - head.position;

        startPosition = transform.position; // Смещение от головы персонажа
    }

    void Update()
    {
        // Изменение расстояния камеры при прокрутке колесика мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0) // Если был скролл
        {
            // Обновляем текущее расстояние
            currentDistance -= scroll * zoomSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance); // Ограничиваем минимальное и максимальное расстояние

            // Обновляем позицию камеры
            Vector3 desiredPosition = head.position + offset.normalized * currentDistance;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        }

    }
}
