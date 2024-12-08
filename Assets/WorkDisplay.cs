using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WorkDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI workTitleText;
    public TextMeshProUGUI workTypeText;
    public Button deleteButton;

    private int workID;

    private UserWorksManager userWorksManager;

    public void SetWorkData(Work work, UserWorksManager manager)
    {
        workTitleText.text = work.WorkTitle;
        workTypeText.text = work.WorkType;

        workID = work.WorkID;
        userWorksManager = manager;

        deleteButton.onClick.AddListener(() => userWorksManager.DeleteWork(workID));
    }

}
