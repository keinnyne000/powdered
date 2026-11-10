using System.Collections.Generic;
using Presentation.Game.Presentation;
using UnityEngine;

namespace Game.Presentation
{
    public class ViewRegistry : MonoBehaviour
    {
        public IReadOnlyList<IGameView> views => GetComponentsInChildren<IGameView>(true);
    }
}