using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VirtualTour
{
    public class MachineText : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _textNote;
        [SerializeField] private UITextScroll _uITextScroll;
        [SerializeField] private Canvas _canvasMachineText;
        [SerializeField] private FirstPersonController _fpsController;

        [SerializeField] private string _nameMachineFile;


       [SerializeField] private bool _inTriggerMachine = false;

        private void Start()
        {
            TextScrollTxt.Instance.ShowLoadData(TextStorage.CheckSaveData(_nameMachineFile));
            Debug.Log(TextStorage.CheckSaveData(_nameMachineFile));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                _textNote.enabled = true;
                // _uITextScroll.enabled = false;
                
                 _inTriggerMachine = true;
                
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                _textNote.enabled = false;

                // OffScroll();
                _canvasMachineText.gameObject.SetActive(false);
                _inTriggerMachine = false;
                _fpsController.cameraCanMove = true;
            }
        }

        public void OffScroll()
        {
          //  _uITextScroll.enabled = false;
          //  _uITextScroll.StartPos();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I) && _inTriggerMachine)
            {
                _textNote.enabled = !_textNote.enabled;
                _canvasMachineText.gameObject.SetActive(true);
                _fpsController.cameraCanMove = false;
                // _uITextScroll.enabled = !_uITextScroll.enabled;
                // _uITextScroll.StartPos();
            }
        }
    }
}