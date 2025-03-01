using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WorkDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text workIdText;
    public TMP_Text workTitleText;
    public TMP_Text workTypeText;
    public TMP_Text workLikeText;
    public TMP_Text workModerateText;
    public Button deleteButton;
    public Button addButton;

    private int workID;

    private UserWorksManager userWorksManager;

    public void SetWorkData(AllWork work, UserWorksManager manager)
    {
        workIdText.text = work.WorkID.ToString();
        workTitleText.text = work.WorkTitle;
        workTypeText.text = work.WorkType;
        workLikeText.text = work.LikesCount.ToString();
        workModerateText.text = work.IsModerated.ToString();

        workID = work.WorkID;
        userWorksManager = manager;

        deleteButton.onClick.AddListener(() => userWorksManager.DeleteWork(workID));
    }

}
