using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuUI : MonoBehaviour
{
    public GameObject buildingsPanel; // Панель для зданий
    public GameObject decorationsPanel; // Панель для декораций
    public GameObject roadsPanel; // Панель для дорожек

    public Button buildingsButton; // Кнопка для зданий
    public Button decorationsButton; // Кнопка для декораций
    public Button roadsButton; // Кнопка для дорожек

    void Start()
    {
        // Назначаем обработчики для кнопок
        buildingsButton.onClick.AddListener(() => ShowBuildings());
        decorationsButton.onClick.AddListener(() => ShowDecorations());
        roadsButton.onClick.AddListener(() => ShowRoads());

        // По умолчанию показываем панель зданий
        ShowBuildings();
    }

    // Показать панель зданий
    private void ShowBuildings()
    {
        buildingsPanel.SetActive(true);
        decorationsPanel.SetActive(false);
        roadsPanel.SetActive(false);
    }

    // Показать панель декораций
    private void ShowDecorations()
    {
        buildingsPanel.SetActive(false);
        decorationsPanel.SetActive(true);
        roadsPanel.SetActive(false);
    }

    // Показать панель дорожек
    private void ShowRoads()
    {
        buildingsPanel.SetActive(false);
        decorationsPanel.SetActive(false);
        roadsPanel.SetActive(true);
    }
}