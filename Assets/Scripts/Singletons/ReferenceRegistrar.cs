using UnityEngine;

public class ReferenceRegistrar : Singleton<ReferenceRegistrar>
{
    public PlayerReference Player = new PlayerReference();
    public PortalPair portalPair;
    public InGameUI inGameUI;

    public void RegisterPlayer(GameObject player)
    {
        Player.PlayerMovement = player.GetComponent<PlayerMovement>();
        Player.PortalPlacement = player.GetComponent<PortalPlacement>();
        Player.PlayerProgress = player.GetComponent<PlayerProgress>();
        Player.PlayerPortalableController = player.GetComponent<PlayerPortalableController>();
        Player.Crosshair = player.GetComponent<Crosshair>();
        Player.PlayerGhostRun = player.GetComponent<GhostRunPlayback>();
        Player.TutorialTriggerGroup = player.GetComponent<TutorialTriggerGroup>();
        Player.CameraMove = player.GetComponent<CameraMove>();
        Player.CharacterController = player.GetComponent<CharacterController>();
    }

    public void RegisterPortalPair(PortalPair pair)
    {
        portalPair = pair;
    }

    public void RegisterInGameUI(InGameUI ui)
    {
        inGameUI = ui;
    }
}

public class PlayerReference
{
    public PlayerMovement PlayerMovement;
    public PortalPlacement PortalPlacement;
    public PlayerProgress PlayerProgress;
    public PlayerPortalableController PlayerPortalableController;
    public Crosshair Crosshair;
    public GhostRunPlayback PlayerGhostRun;
    public TutorialTriggerGroup TutorialTriggerGroup;
    public CameraMove CameraMove;
    public CharacterController CharacterController;
}
