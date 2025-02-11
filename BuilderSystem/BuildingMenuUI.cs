using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuUI : MonoBehaviour
{
    public GameObject buildingsPanel; // ������ ��� ������
    public GameObject decorationsPanel; // ������ ��� ���������
    public GameObject roadsPanel; // ������ ��� �������

    public Button buildingsButton; // ������ ��� ������
    public Button decorationsButton; // ������ ��� ���������
    public Button roadsButton; // ������ ��� �������

    void Start()
    {
        // ��������� ����������� ��� ������
        buildingsButton.onClick.AddListener(() => ShowBuildings());
        decorationsButton.onClick.AddListener(() => ShowDecorations());
        roadsButton.onClick.AddListener(() => ShowRoads());

        // �� ��������� ���������� ������ ������
        ShowBuildings();
    }

    // �������� ������ ������
    private void ShowBuildings()
    {
        buildingsPanel.SetActive(true);
        decorationsPanel.SetActive(false);
        roadsPanel.SetActive(false);
    }

    // �������� ������ ���������
    private void ShowDecorations()
    {
        buildingsPanel.SetActive(false);
        decorationsPanel.SetActive(true);
        roadsPanel.SetActive(false);
    }

    // �������� ������ �������
    private void ShowRoads()
    {
        buildingsPanel.SetActive(false);
        decorationsPanel.SetActive(false);
        roadsPanel.SetActive(true);
    }
}