using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTour
{
    public class MouseLock : MonoBehaviour
    {
        private bool _isPress = false;
        private bool _isLocked = false;
        [SerializeField] private GameObject _panelExplorer;

        private void Start()
        {
            SetCursorLock(true);
        }

        private void SetCursorLock(bool isLocked)
        {
            _isLocked = isLocked;
            if (_isLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            if (!_isLocked) Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = !isLocked;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SetCursorLock(!_isLocked);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isPress = !_isPress;
                _panelExplorer.SetActive(_isPress);
            }
       
        }
    }
}