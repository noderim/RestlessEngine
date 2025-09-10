using static RestlessEngine.Application.ApplicationManager;

namespace RestlessEngine.Application
{
    public interface IApplicationManager
    {
        public AppState CurrentAppState { get; }
        public void QuitGame();
    }
}
