namespace Corby.Frameworks.UI
{
    public class WidgetBox<T>
        where T : WWidget
    {
        public T Instance { get; set; }
        public bool IsNull => Instance == null;
    }
}