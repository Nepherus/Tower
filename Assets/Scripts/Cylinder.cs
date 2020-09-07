using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cylinder : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Collider _collider;

    private bool _active;
    public bool Active => _active;
    
    private int _colorId = -1;
    public int ColorId => _colorId;

    public Action<Cylinder> OnDestroy;

    public void ToggleKinematic(bool enabled) => _rigidbody.isKinematic = enabled;

    public void Activate()
    {
        _active = true;
        ToggleKinematic(false);
        SetActiveColor();
    }

    public void Deactivate()
    {
        _active = false;
        ToggleKinematic(true);
        SetInactiveColor();
    }

    public void SetRandomColor()
    {
        _colorId = Random.Range(0, GameSettings.Instance.Colors.ColorCount);
        SetActiveColor();
    }

    public void Explode()
    {
        float force = 200f;
        float radius = 3f;
        var cylinderColliders =
            Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Cylinders"));
        foreach (var collider in cylinderColliders)
        {
            if (collider != null)
            {
                var cylinder = collider.GetComponent<Cylinder>();
                if (cylinder != null)
                {
                    cylinder.AddExplosionForce(force, transform.position, radius);
                }
            }
        }
        Vibration.Vibrate(100);
        
        OnDestroy?.Invoke(this);
    }

    public void StartFloating()
    {
        _rigidbody.useGravity = false;
        var forceDirection = (transform.position - Vector3.zero).normalized;
        _rigidbody.AddForce(forceDirection * 10f);
    }

    public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        _rigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
    }

    private void SetActiveColor() => _meshRenderer.SetPropertyBlock(GameSettings.Instance.Colors.GetColorBlock(_colorId));
    private void SetInactiveColor() => _meshRenderer.SetPropertyBlock(GameSettings.Instance.Colors.GetDisabledColorBlock());
}
