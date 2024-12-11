using System;
using System.Collections;
using System.IO;
using Events;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    [Serializable]
    public class Work
    {
        public string title;
        public string workType;
        public string filePath;
    }
    
    public class UIAddWork : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private InputField workTitle;
        [SerializeField] private Dropdown workTypeDropdown;
        [SerializeField] private InputField fileNameText;

        [Header("Popup")] 
        [SerializeField] private UIPopup SuccessPopup;
        [SerializeField] private UIPopup ServerErrorPopup;
        [SerializeField] private UIPopup InputErrorPopup;

        [Header("Events")]
        [SerializeField] private VoidEventChannelSO successEventChannel;
        [SerializeField] private BoolEventChannelSO serverErrorEventChannel;
        [SerializeField] private WorkUploadHandlerSO _workUploadHandler;
        
        private string filePath;

        public UnityAction Closed;

        private void OnEnable()
        {
            successEventChannel.OnEventRaised += ShowSuccessPopup;
            serverErrorEventChannel.OnEventRaised += ShowServerErrorPopup;
        }

        private void OnDisable()
        {
            successEventChannel.OnEventRaised -= ShowSuccessPopup;
            serverErrorEventChannel.OnEventRaised -= ShowServerErrorPopup;
        }

        public void OpenFileExplorer()
        {
            FileBrowser.SetFilters(false, new FileBrowser.Filter("Images", ".png"), new FileBrowser.Filter("Music", ".mp3"), new FileBrowser.Filter("Models", ".fbx"));
            FileBrowser.AddQuickLink("Users", "C:\\Users", null);

            StartCoroutine(ShowLoadDialogCoroutine());
        }
        
        IEnumerator ShowLoadDialogCoroutine()
        {
            yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, true, null, null, "Select Files", "Load");

            Debug.Log(FileBrowser.Success);

            if (FileBrowser.Success)
                OnFilesSelected(FileBrowser.Result);
        }

        void OnFilesSelected(string[] filePaths)
        {
            for (int i = 0; i < filePaths.Length; i++)
                Debug.Log(filePaths[i]);

            filePath = filePaths[0];

            if (!string.IsNullOrEmpty(filePath))
            {
                fileNameText.text = $"Selected File: {Path.GetFileName(filePath)}";
            }
            else
            {
                fileNameText.text = "No file selected";
            }

        }

        private Work CreateWork()
        {
            Work work = new Work();

            if (string.IsNullOrEmpty(workTitle.text))
            {
                ShowInputErrorPopup();
            }
            else
            {
                work.title = workTitle.text;
            }
            
            string workType = "";
            switch (workTypeDropdown.value)
            {
                case 0:
                    workType = "Image";
                    break;
                case 1:
                    workType = "Music";
                    break;
                case 2:
                    workType = "Model";
                    break;
            }
            
            work.workType = workType;

            if (string.IsNullOrEmpty(filePath))
            {
                ShowInputErrorPopup();
            }
            else
            {
                work.filePath = filePath;
            }

            return work;
        }

        public void UploadFile()
        {
            var work = CreateWork();
            _workUploadHandler.RaiseEvent(work);
        }

        private void ShowSuccessPopup()
        {
            SuccessPopup.ClosePopupAction += HideSuccessPopup;
            successEventChannel.OnEventRaised -= ShowSuccessPopup;
            SuccessPopup.gameObject.SetActive(true);
        }

        private void HideSuccessPopup(bool arg0)
        {
            SuccessPopup.ConfirmationResponseAction -= HideSuccessPopup;
            SuccessPopup.gameObject.SetActive(false);
        }

        private void ShowServerErrorPopup(bool arg)
        {
            ServerErrorPopup.ClosePopupAction += HideServerErrorPopup;
            serverErrorEventChannel.OnEventRaised -= ShowServerErrorPopup;
            ServerErrorPopup.gameObject.SetActive(true);
        }

        private void HideServerErrorPopup(bool arg0)
        {
            ServerErrorPopup.ConfirmationResponseAction -= HideServerErrorPopup;
            ServerErrorPopup.gameObject.SetActive(false);
        }

        private void ShowInputErrorPopup()
        {
            InputErrorPopup.ClosePopupAction += HideInputErrorPopup;
            InputErrorPopup.gameObject.SetActive(true);
        }

        private void HideInputErrorPopup(bool arg0)
        {
            InputErrorPopup.ConfirmationResponseAction -= HideInputErrorPopup;
            InputErrorPopup.gameObject.SetActive(false);
        }

        public void Close()
        {
            Closed?.Invoke();
        }
    }
}