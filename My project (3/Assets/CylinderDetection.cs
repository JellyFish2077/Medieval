using UnityEngine;
using UnityEngine.UI;

public class CylinderDetection : MonoBehaviour
{
    public Text cylinderText; // Ссылка на UI Text
    public float rayDistance = 5f; // Дистанция луча

    void Update()
    {
        // Создаем луч из центра камеры
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Проверяем, попал ли луч в цилиндр
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Cylinder"))
            {
                cylinderText.enabled = true; // Показываем текст
                cylinderText.text = "Это цилиндр";
            }
            else
            {
                cylinderText.enabled = false; // Скрываем текст
            }
        }
        else
        {
            cylinderText.enabled = false; // Скрываем текст
        }
    }
}