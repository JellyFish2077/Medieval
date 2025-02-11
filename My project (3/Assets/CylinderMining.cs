using UnityEngine;

public class CylinderMining : MonoBehaviour
{
    public int hitsToBreak = 4; // Количество нажатий для уничтожения
    private int currentHits = 0; // Текущее количество нажатий

    void Update()
    {
        // Проверяем, наведена ли камера на цилиндр
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            // Если нажата ЛКМ
            if (Input.GetMouseButtonDown(0))
            {
                currentHits++;
                Debug.Log("Нажатий: " + currentHits);

                // Если нажатий достаточно, уничтожаем цилиндр
                if (currentHits >= hitsToBreak)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}