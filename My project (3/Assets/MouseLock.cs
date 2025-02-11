using UnityEngine;

public class MouseLok : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Чувствительность мыши
    public Transform playerBody; // Ссылка на объект игрока

    private float xRotation = 0f; // Угол поворота камеры по оси X

    void Start()
    {
        // Скрываем и фиксируем курсор в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Получаем ввод от мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Вращение камеры по оси X (вверх-вниз)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Ограничиваем угол, чтобы камера не переворачивалась

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Вращение игрока по оси Y (влево-вправо)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}