using Hiyazcool;
using Hiyazcool.Unity;
public class GameHandler : MonoBehaviourSingleton<GameHandler>
{
    /*
     * Need Save Function Low Priority
     * Finish Implementing basic Gameplay mechanics
     * 
     */
    private TickSystem tickSystem;
    void Start()
    {        
        tickSystem = ISingleton<TickSystem>.Create();
    }
    void Update()
    {
        tickSystem?.ProgressTick();
    }
}
