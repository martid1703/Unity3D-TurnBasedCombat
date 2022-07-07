using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnfrozenTestWork
{
    public class BattleQueuePresenter : MonoBehaviour
    {
        [SerializeField]
        public Transform _battleQueueContainerContent;

        [SerializeField]
        private Transform _unitIconPrefab;

        private int _maxIconsQty;

        private UnitModelProvider _unitModelProvider;
        private Dictionary<UnitModel, UnitIcon> _unitIconsDic;
        private Queue<UnitModel> _iconsUnitQueue;

        private void Awake()
        {
            _unitModelProvider = FindObjectOfType<UnitModelProvider>();
            _unitIconsDic = new Dictionary<UnitModel, UnitIcon>();
            _iconsUnitQueue = new Queue<UnitModel>();
            _maxIconsQty = _battleQueueContainerContent.childCount;
        }

        public IEnumerator AddUnitIcons(Queue<UnitModel> newBattleQueue)
        {
            foreach (var icon in _battleQueueContainerContent.GetComponentsInChildren<UnitIcon>())
            {
                GameObject.Destroy(icon.gameObject);
            }

            yield return null;

            foreach (var unit in newBattleQueue)
            {
                AddUnitIcon(unit);
            }
        }

        public void TakeTurn(UnitModel unit)
        {
            RemoveUnitIcon(unit);
            AddUnitIcon(unit);
        }

        public void AddUnitIcon(UnitModel unit)
        {
            _iconsUnitQueue.Enqueue(unit);
            ShowNextUnitIcon();
        }

        public void RemoveUnitIcon(UnitModel unit)
        {
            if (unit != null)
            {
                if (!_unitIconsDic.TryGetValue(unit, out var icon))
                {
                    Debug.LogWarning("Cannot find icon to remove.");
                    return;
                }
                _unitIconsDic.Remove(unit);
                GameObject.Destroy(icon.gameObject);
            }

            ShowNextUnitIcon();
        }

        private void ShowNextUnitIcon()
        {
            if (_battleQueueContainerContent.childCount >= _maxIconsQty || _iconsUnitQueue.Count == 0)
            {
                return;
            }

            var unit = _iconsUnitQueue.Dequeue();
            if (!unit.IsAlive)
            {
                return;
            }
            var icon = GameObject
                        .Instantiate(_unitIconPrefab, Vector3.zero, Quaternion.identity, _battleQueueContainerContent)
                        .GetComponent<UnitIcon>();
            SetIconImage(unit, icon);
            _unitIconsDic.Add(unit, icon);
        }

        private UnitIcon SetIconImage(UnitModel unit, UnitIcon unitIcon)
        {
            Texture2D unitImage = _unitModelProvider.GetUnitIconImage(unit.UnitData.Type);
            if (unit.UnitData.Belonging == UnitBelonging.Player1)
            {
                unitIcon.GetComponent<Image>().color = Color.blue;
            }
            else
            {
                unitIcon.GetComponent<Image>().color = Color.red;
            }
            unitIcon.transform.Find("UnitImage").GetComponent<Image>().sprite = Sprite.Create(unitImage, new Rect(0.0f, 0.0f, unitImage.width, unitImage.height), new Vector2(0.5f, 0.5f), 100.0f);
            return unitIcon;
        }
    }
}