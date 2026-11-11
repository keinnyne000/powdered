using Game.Core;
using Presentation.Game.Presentation;
using UnityEngine;

namespace Game.Presentation
{
    /// <summary>
    /// Health bar. Listens to GameState.PlayerHealth and resizes the fill to match.
    /// </summary>
    public class HealthBarView : MonoBehaviour, IGameView
    {
        [Tooltip("The bar's fill. Its anchors must stretch (Min 0,0 / Max 1,1) with all offsets 0")]
        [SerializeField] RectTransform fill;

        ViewContext _ctx;

        public void Bind(ViewContext viewContext)
        {
            _ctx = viewContext;
            _ctx.GameState.PlayerHealth.Subscribe(Refresh);
            Refresh(_ctx.GameState.PlayerHealth.Value);
        }

        public void Unbind()
        {
            if (_ctx != null)
                _ctx.GameState.PlayerHealth.Unsubscribe(Refresh);
        }

        void OnDestroy() => Unbind();

        void Refresh(float health)
        {
            float fraction = Mathf.Clamp01(health / GameState.PlayerMaxHealth);
            fill.anchorMax = new Vector2(fraction, fill.anchorMax.y);
        }
    }
}