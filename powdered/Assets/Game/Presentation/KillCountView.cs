using Presentation.Game.Presentation;
using TMPro;
using UnityEngine;

namespace Game.Presentation
{
    /// <summary>
    /// Kill counter. Listens to GameState.KillCount and updates the label.
    /// </summary>
    public class KillCountView : MonoBehaviour, IGameView
    {
        [SerializeField] TMP_Text label;

        ViewContext _ctx;

        public void Bind(ViewContext viewContext)
        {
            _ctx = viewContext;
            _ctx.GameState.KillCount.Subscribe(Refresh);
            Refresh(_ctx.GameState.KillCount.Value);
        }

        public void Unbind()
        {
            if (_ctx != null)
                _ctx.GameState.KillCount.Unsubscribe(Refresh);
        }

        void OnDestroy() => Unbind();

        void Refresh(int kills)
        {
            label.text = "Kills: " + kills.ToString();
        }
    }
}