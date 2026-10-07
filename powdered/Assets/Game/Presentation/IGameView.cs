namespace Presentation.Game.Presentation
{
    public interface IGameView
    {
        public void Bind(ViewContext viewContext);
        public void Unbind();
    }
}