using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScaffholdingScript : MonoBehaviour
{
    private const int StartDropHeight = 20;
    private const float WidthIncreasePerDistance = 0.5f;
    [SerializeField] private GameObject _indicator;

    [SerializeField, Range(0, 100)]
    private int _fallingWallChance = 50;

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

        GenerateConePattern(direction, (position, distance) =>
        {
            GameObject instance = Instantiate(_indicator, position, Quaternion.identity);
            indicators.Add(instance);
        });


        _playerIndicators[player] = indicators;
    }

    private static bool IsOutOfBounds(Vector3 indicatorPosition)
    {
        return Mathf.Abs(indicatorPosition.x) > GameManager.Instance.Width / 2 ||
                        Mathf.Abs(indicatorPosition.z) > GameManager.Instance.Height / 2;
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

        GenerateConePattern(direction, (position, distance) =>
        {
            position = new Vector3(position.x, StartDropHeight +  distance, position.z);

            if (Random.Range(0, 100 / _fallingWallChance) == 0)
            {
                GameManager.Instance.DropWall(position);
            }
        });

        foreach (var player in _playerIndicators.Keys.ToList())
        {
            RemoveIndicators(player);
        }

        GameManager.Instance.GridGenerate.wallPositions.Remove(transform.position);
        GameManager.Instance.GridGenerate.wallObjects.Remove(gameObject);
        GameManager.Instance.GemManager.ScaffholdingObjects.Remove(gameObject);
        GameManager.Instance.GemManager.EmptyTiles.Add(transform.position);
        Destroy(gameObject);
    }
    private void GenerateConePattern(Vector3 direction, System.Action<Vector3, int> action)
    {
        Vector3 perpendicularDirection = Vector3.Cross(Vector3.up, direction).normalized;

        int counter = 1;
        float maxWidth = 0;

        for (int distance = 0; distance < 100; distance++)
        {
            Vector3 centerPosition = transform.position + (direction * counter);

            if (IsOutOfBounds(centerPosition))
                break;

            action(centerPosition, distance);

            for (int width = 1; width <= (int)maxWidth; width++)
            {
                Vector3 sideOffset = perpendicularDirection * width;

                Vector3 leftPosition = centerPosition - sideOffset;
                Vector3 rightPosition = centerPosition + sideOffset;

                if (!IsOutOfBounds(leftPosition))
                    action(leftPosition, distance);
                if (!IsOutOfBounds(rightPosition))
                    action(rightPosition, distance);
            }

            counter++;
            maxWidth += WidthIncreasePerDistance;
        }
    }
}
