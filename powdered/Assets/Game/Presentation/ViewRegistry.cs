using System.Collections.Generic;
using Presentation.Game.Presentation;
using UnityEngine;

namespace Game.Presentation
{
    public class ViewRegistry : MonoBehaviour
    {
        public List<IGameView> views = new();
    }
}