using UnityEngine;

namespace UnfrozenTestWork
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class UnitSelectionDisplayer : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private Color _target;
        private Color _attacker;
        private Color _highlighted;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _target = _spriteRenderer.color;
            _highlighted = Color.yellow;
            _attacker = Color.blue;
        }

        public void SelectAsTarget()
        {
            _spriteRenderer.color = _target;
            _spriteRenderer.enabled = true;
        }

        public void SelectAsAttacker()
        {
            _spriteRenderer.color = _attacker;
            _spriteRenderer.enabled = true;
        }

        public void Deselect()
        {
            _spriteRenderer.enabled = false;
        }

        public void Highlight()
        {
            _spriteRenderer.color = _highlighted;
            _spriteRenderer.enabled = true;
        }
    }
}
