using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class ScaffholdingScript : MonoBehaviour
{
    [SerializeField] private GameObject _indicator;

    private Dictionary<Collider, Vector3> _playerDirections = new();
    private Dictionary<Collider, List<GameObject>> _playerIndicators = new();
    public void OnTriggerStayRelayed(Collider other)
    {
        Vector3 direction = transform.position - other.transform.position;

        // Ignore diagonals
        if (Mathf.Abs(direction.x) > 0.4f && Mathf.Abs(direction.z) > 0.4f)
        {
            if (_playerDirections.ContainsKey(other))
            {
                _playerDirections.Remove(other);
                RemoveIndicators(other);
            }
            return;
        }

        direction = ConvertDirectionToCardinal(direction);

        if (!_playerDirections.ContainsKey(other) || _playerDirections[other] != direction)
        {
            _playerDirections[other] = direction;
            RemoveIndicators(other);
            GenerateIndicators(other, direction);
        }
    }

    private void GenerateIndicators(Collider player, Vector3 direction)
    {
        List<GameObject> indicators = new();
        int counter = 1;

        for (int i = 0; i < 100; i++)
        {
            Vector3 indicatorPosition = transform.position + (direction * counter);

            if (Mathf.Abs(indicatorPosition.x) > GameManager.Instance.Width / 2 ||
                Mathf.Abs(indicatorPosition.z) > GameManager.Instance.Height / 2)
            {
                break;
            }

            indicators.Add(Instantiate(_indicator, indicatorPosition, Quaternion.identity));
            counter++;
        }

        _playerIndicators[player] = indicators;
    }

    public void OnTriggerExitRelayed(Collider other)
    {
        _playerDirections.Remove(other);
        RemoveIndicators(other);
    }
    private void RemoveIndicators(Collider player)
    {
        if (_playerIndicators.TryGetValue(player, out List<GameObject> indicators))
        {
            foreach (var indicator in indicators)
            {
                Destroy(indicator);
            }
            _playerIndicators.Remove(player);
        }
    }

    private Vector3 ConvertDirectionToCardinal(Vector3 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            return new Vector3(Mathf.Sign(direction.x), 0, 0);
        }
        else
        {
            return new Vector3(0,0,Mathf.Sign(direction.z));
        }
    }
    public void CollapseScaffholding(Vector3 direction)
    {
        direction = ConvertDirectionToCardinal(direction);

        int counter = 1;

        for (int i = 0; i < 100; i++)
        {
            Vector3 wallPosition = transform.position + (direction * counter) + Vector3.up * (counter + 10);

            if (Mathf.Abs(wallPosition.x) > GameManager.Instance.Width / 2 ||
                Mathf.Abs(wallPosition.z) > GameManager.Instance.Height / 2)
            {
                break;
            }

            GameManager.Instance.DropWall(wallPosition);
            counter++;
        }
        foreach (var player in _playerIndicators.Keys.ToList())
        {
            RemoveIndicators(player);
        }
        Destroy(gameObject);
    }
}
