using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnValues : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    private List<BlockType> _spawnableBlockTypes = new List<BlockType>();
    private void OnEnable()
    {
        _spawnableBlockTypes.Clear();

        foreach (BlockType blockType in gameManager.types)
        {
            if (blockType.value >= DataManager.Instance.minSpawnValue && blockType.value <= DataManager.Instance.maxSpawnValue)
            {
                _spawnableBlockTypes.Add(blockType);
            }
            if (_spawnableBlockTypes.Count == 6) break;
        }

        for (int i = 0; i < 6; i++)
        {
            transform.GetChild(i).GetComponent<Image>().color = _spawnableBlockTypes[i].color;
            transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = _spawnableBlockTypes[i].valueString;
        }
    }
}
