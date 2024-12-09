using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class WorkDetailsDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text workTitleText;
    public TMP_Text workTypeText;
    public TMP_Text likesCountText;
    public Button likeButton;

    [Header("Dependencies")]
    public UserData userGameData;
    public ConnectData connectData;

    private Work workData;

    private void Start()
    {
        if (userGameData.LikedWorks.Contains(workData.WorkID))
        {
            DisableLikeButton();
        }

        likeButton.onClick.AddListener(OnLikeButtonClicked);
    }

    public void SetWorkData(Work work)
    {
        workData = work;

        workTitleText.text = work.WorkTitle;
        workTypeText.text = work.WorkType;
        likesCountText.text = $"{work.LikesCount} Likes";

        if (userGameData.LikedWorks.Contains(work.WorkID))
        {
            DisableLikeButton();
        }
    }

    public Work GetWorkData()
    {
        return workData;
    }

    private void OnLikeButtonClicked()
    {
        if (!userGameData.LikedWorks.Contains(workData.WorkID))
        {
            StartCoroutine(SendLikeRequest(workData.WorkID));
        }
    }

    private IEnumerator SendLikeRequest(int workId)
    {
        string likeUrl = connectData.GetLikeWorkUrl(workId);

        UnityWebRequest request = UnityWebRequest.PostWwwForm(likeUrl, "");
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Successfully liked work {workId}");

            userGameData.LikedWorks.Add(workId);
            workData.LikesCount++;

            likesCountText.text = $"{workData.LikesCount} Likes";

            DisableLikeButton();
        }
        else
        {
            Debug.LogError($"Error liking work {workId}: {request.error}");
        }
    }

    private void DisableLikeButton()
    {
        likeButton.interactable = false;
        likeButton.GetComponentInChildren<TMP_Text>().text = "Liked";
    }
}
