using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _speed;
    [SerializeField] private LayerMask _cylinderLayerMask;
    [SerializeField] private ParticleSystem _gunParticles;
    [SerializeField] private ParticleSystem _bulletParticles;
    [SerializeField] private int _totalBullets;
    [SerializeField] private TextMeshProUGUI _bulletCounter;

    private ParticleSystem.MainModule _bulletModule;
    private ParticleSystem.MainModule _gunParticlesModule;

    private Vector2 _lastTouchPosition;

    private int _currentColorId;
    private bool _canShoot;
    private bool _isMoving = false;

    private int _currentBullets;
    private float _movementThreshold = 0.01f;

    public Action OnNoBullets;

    private void Start()
    {
        _gunParticlesModule = _gunParticles.main;
        _bulletModule = _bulletParticles.main;
        UpdateColor();
    }
    

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isMoving = false;
            _lastTouchPosition = _mainCamera.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            float movementDelta = _mainCamera.ScreenToViewportPoint(Input.mousePosition).x - _lastTouchPosition.x;
            if(Mathf.Abs(movementDelta) > _movementThreshold)
            {
                _isMoving = true;
                RotateAroundCenter(movementDelta);
                _lastTouchPosition = _mainCamera.ScreenToViewportPoint(Input.mousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0) && !_isMoving && _canShoot && _currentBullets >= 0)
        {
            TryShooting();
        }
    }

    public void StartGame()
    {
        _canShoot = true;
        _currentBullets = _totalBullets;
        _bulletCounter.text = _currentBullets.ToString();
        _bulletCounter.gameObject.SetActive(true);
        _gunParticles.Play();
    }

    public void RotateAroundCenter(float angle)
    {
        transform.RotateAround(Vector3.up * transform.position.y, Vector3.up, angle * _speed);
    }

    private void TryShooting()
    {
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, _cylinderLayerMask))
        {
            var cylinder = hit.transform.GetComponent<Cylinder>();
            if (cylinder != null && cylinder.Active)
            {
                Shoot(cylinder);
            }
        }
    }

    private void Shoot(Cylinder cylinder)
    {
        _canShoot = false;
        _currentBullets--;
        if(_currentBullets == 0) OnNoBullets?.Invoke();
        _bulletCounter.text = _currentBullets.ToString();
        _bulletModule.startColor = _gunParticlesModule.startColor;
        _gunParticles.Stop();
        _bulletParticles.Play();
        _bulletParticles.transform.position = _gunParticles.transform.position;
        LeanTween.value(gameObject, _bulletParticles.transform.position, cylinder.transform.position, 0.5f)
            .setOnUpdate((Vector3 position) => _bulletParticles.transform.position = position)
            .setOnComplete(() => DestroyCylinder(cylinder));
    }

    private void DestroyCylinder(Cylinder cylinder)
    {
        if(cylinder.ColorId == _currentColorId)
            cylinder.Explode();

        _bulletParticles.Stop();
        _gunParticles.Play();
        UpdateColor();
        _canShoot = true;
    }

    private void UpdateColor()
    {
        _currentColorId = Random.Range(0, GameSettings.Instance.Colors.ColorCount);
        var colorBlock = GameSettings.Instance.Colors.GetColorBlock(_currentColorId);
        _gunParticlesModule.startColor = colorBlock.GetColor("_Color");
    }

    public void Disable()
    {
        _bulletCounter.gameObject.SetActive(false);
        _bulletParticles.Stop();
        _gunParticles.Stop();
        _canShoot = false;
    }

    public void Reset()
    {
        Disable();
        transform.position = new Vector3(transform.position.x, 5f, transform.position.z);
    }
}
