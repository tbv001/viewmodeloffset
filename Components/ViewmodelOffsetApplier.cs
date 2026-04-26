using UnityEngine;

namespace ViewmodelOffset;

public class ViewmodelOffsetApplier : MonoBehaviour
{
    private readonly Vector3 _gameBaseOffset = Vector3.up * 0.05f; // From SetCameraTransform() method in PlayerCamera class
    private const float _lerpSpeed = 15f;
    private PlayerCamera _playerCam;
    private PlayerMain _playerMain;
    private Transform _skinTransform;
    private Vector3 _currentOffset;

    private void Flip()
    {
        if (_playerMain?.arms == null)
            return;

        Vector3 curScale = _playerMain.arms.transform.localScale;
        curScale.x = curScale.x * -1f;
        _playerMain.arms.transform.localScale = curScale;
    }

    private void LateUpdate()
    {
        if (_playerCam == null)
        {
            if (PlayerCamera.instance == null)
                return;
            _playerCam = PlayerCamera.instance;
            _playerMain = _playerCam.playerMain;
        }

        if (_playerMain == null)
            return;

        if (_playerCam.mode != PlayerCamera.Mode.FirstPerson)
        {
            if (_playerMain.arms != null && _playerMain.arms.transform.localScale.x < 0f)
            {
                Flip();
            }
            return;
        }

        if (_playerMain.SpawnedSkin == null)
            return;

        if (_skinTransform == null)
            _skinTransform = _playerMain.SpawnedSkin.transform;

        if (_playerMain.arms == null)
            return;

        bool isADS = _playerMain.arms.ads;

        if (isADS || ViewmodelOffset.viewmodelOffset == Vector3.zero)
        {
            _currentOffset = Vector3.Lerp(_currentOffset, _gameBaseOffset, Time.deltaTime * _lerpSpeed);
        }
        else
        {
            Transform parent = _skinTransform.parent;
            Vector3 worldOffset = _playerCam.transform.TransformDirection(ViewmodelOffset.viewmodelOffset);
            Vector3 localOffset = (parent != null) ? parent.InverseTransformDirection(worldOffset) : worldOffset;

            _currentOffset = Vector3.Lerp(_currentOffset, _gameBaseOffset + localOffset, Time.deltaTime * _lerpSpeed);
        }

        _skinTransform.localPosition = _currentOffset;

        if (_playerMain.arms.transform.localScale.x > 0f && ViewmodelOffset.shouldFlip)
        {
            Flip();
        }
        else if (_playerMain.arms.transform.localScale.x < 0f && !ViewmodelOffset.shouldFlip)
        {
            Flip();
        }
    }
}
