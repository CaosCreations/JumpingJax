using System.Collections.Generic;
using UnityEngine;

public static class PlayerConstants
{
    public static float MoveSpeed = 6f;
    public static float NoClipMoveSpeed = 12f;
    public static float BackWardsMoveSpeedScale = 0.9f;
    public static float CrouchingMoveSpeed = 5f;
    public static float MaxVelocity = 60f;
    public static float MaxReasonableVelocity = 30f;
    public static float MaxFallSpeed = 40f;

    public static float Gravity = 15f;
    public static float JumpPower = 5.6f;
    public static float CrouchingJumpPower = 5f;

    public static float GroundAcceleration = 10f;
    public static float AirAcceleration = 1000f;
    public static float Overbounce = 1.001f;

    public static float StopSpeed = 6f;
    public static float Friction = 6f;
    public static float MinimumSpeedCutoff = 0.5f; // This is the speed after which the player is immediately stopped due to friction
    public static float NormalSurfaceFriction = 1f;

    public static float AirAccelerationCap = .7f;

    public static float StandingPlayerHeight = 1.6f;
    public static Vector3 StandingCameraOffset = new Vector3(0, -0.25f, 0);

    public static float CrouchingPlayerHeight = 0.8f;
    public static Vector3 CrouchingCameraOffset = new Vector3(0, -0.65f, 0);

    public static float TimeToCrouch = 0.5f;
    public static float groundCheckOffset = 0.05f;

    public static float portalWidth = 2f;
    public static float portalHeight = 2f;
    public static Vector3 PortalColliderExtents = new Vector3(1f, 1f, 1f);

    // Layer Masks
    public static LayerMask portalPlacementMask = new LayerMask();


    //HotKeys
    public static string Forward = "Forward";
    public static string ForwardDefault = "W";
    public static string ForwardTooltip = "Moves player forward.";

    public static string Back = "Back";
    public static string BackDefault = "S";
    public static string BackTooltip = "Moves player backward.";

    public static string Left = "Left";
    public static string LeftDefault = "A";
    public static string LeftTooltip = "Moves player left.";

    public static string Right = "Right";
    public static string RightDefault = "D";
    public static string RightTooltip = "Moves player right.";

    public static string Jump = "Jump";
    public static string JumpDefault = "Space";
    public static string JumpTooltip = "Makes player jump.";

    public static string Undo = "Undo";
    public static string UndoDefault = "U";
    public static string UndoTooltip = "Level Editor Undo button";

    public static string Redo = "Redo";
    public static string RedoDefault = "P";
    public static string RedoTooltip = "Level Editor Redo button";

    public static string Respawn = "Respawn";
    public static string RespawnDefault = "R";
    public static string RespawnTooltip = "Reset player to the last checkpoint.";

    public static string ResetLevel = "ResetLevel";
    public static string ResetLevelDefault = "T";
    public static string ResetTooltip = "Reset player to the first checkpoint, set timer back to zero, and clear portals.";

    public static string Crouch = "Crouch";
    public static string CrouchDefault = "LeftControl";
    public static string CrouchTooltip = "Makes player crouch.";

    public static string Portal1 = "Portal1";
    public static string Portal1Default = "Mouse0";
    public static string Portal1Tooltip = "Places the blue portal.";

    public static string Portal2 = "Portal2";
    public static string Portal2Default = "Mouse1";
    public static string Portal2Tooltip = "Places the pink portal.";

    public static string ToggleUI = "ToggleUI";
    public static string ToggleUIDefault = "Z";
    public static string ToggleUITooltip = "Toggles off all UI.";

    public static string LevelEditorSpeedIncrease = "Level Editor Speed Increase";
    public static string LevelEditorSpeedIncreaseDefault = "KeypadPlus";
    public static string LevelEditorSpeedIncreaseTooltip = "Increases editor movement speed";

    public static string LevelEditorSpeedDecrease = "Level Editor Speed Decrease";
    public static string LevelEditorSpeedDecreaseDefault = "KeypadMinus";
    public static string LevelEditorSpeedDecreaseTooltip = "Decreases editor movement speed";

    public static string SensitivityTooltip = "Mouse Sensitivity.";
    public static string MusicVolumeTooltip = "Music Volume.";
    public static string SoundEffectVolumeTooltip = "Sound Effect Volume.";

    public static string CrosshairTooltip = "Toggle Crosshair UI.";
    public static string SpeedTooltip = "Toggle Speed UI.";
    public static string TimeTooltip = "Toggle Time UI.";
    public static string KeyPressedTooltip = "Toggle KeyPressed UI.";
    public static string TutorialTooltip = "Toggle Tutorial UI.";
    public static string GhostTooltip = "Toggle Ghost Runner.";
    public static string ConsoleTooltip = "Toggle Dev Console (`).";

    public static string ResolutionTooltip = "Screen Resolution.";
    public static string GraphicsTooltip = "Graphics Quality.";
    public static string FullscreenTooltip = "Toggle Fullscreen.";
    public static string FOVTooltip = "Player Camera Field of View.";
    public static string UnitOfSpeedTooltip = "Speedometer Unit";

    public static List<string> UnitOfSpeedOptions = new List<string>()
    {
        "Metres per second",
        "Kilometres per hour",
        "Miles per hour"
    };


    // Non-changeable hotkeys
    public static string MouseX = "Mouse X";
    public static string MouseY = "Mouse Y";
    public static KeyCode PauseMenu = KeyCode.Escape;
    public static KeyCode DebugMenu = KeyCode.BackQuote;
    public static KeyCode NextTutorial = KeyCode.Tab;

    public static KeyCode WinMenu_NextLevel = KeyCode.E;
    public static KeyCode WinMenu_RetryLevel = KeyCode.R;
    public static KeyCode WinMenu_MainMenu = KeyCode.Q;


    // Game Constants
    public static int MainMenuSceneIndex = 0;
    public static int LevelEditorSceneIndex = 27;
    public static int CreditsSceneIndex = 28;

    public static Vector3 PlayerSpawnOffset = new Vector3(0, 0.81f, 0);
    public static string levelCompletionTimeFormat = "mm':'ss'.'fff";
    public static int PlayerLayer = 12;
    public static int PortalMaterialLayer = 10;
    public static int PortalLayer = 11;
    public static int GizmoLayer = 15;
    public static int GhostLayer = 16;
    public static string PortalWallTag = "PortalWall";

    // UI Constants
    public static Color activeColor = new Color(.58f, .93f, .76f);
    public static Color inactiveColor = new Color(0.5f, 0.5f, 0.5f);
    public static Color hoverColor = new Color(1, 1, 1);

}
