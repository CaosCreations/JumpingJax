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

    public static float GroundAcceleration = 15f;
    public static float AirAcceleration = 1000f;
    public static float AirAccelerationCap = .7f;

    public static float StopSpeed = 6f;
    public static float Friction = 10f;
    public static float MinimumSpeedCutoff = 0.5f; // This is the speed after which the player is immediately stopped due to friction
    public static float NormalSurfaceFriction = 1f;

    public static float StandingPlayerHeight = 1.6f;
    public static Vector3 StandingCameraOffset = new Vector3(0, -0.25f, 0);

    public static float CrouchingPlayerHeight = 0.8f;
    public static Vector3 CrouchingCameraOffset = new Vector3(0, -0.65f, 0);


    public static float portalWidth = 3f;
    public static float portalHeight = 3f;
    public static float PortalRaycastDistance = 250;


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

    public static string ResetPortals = "ResetPortals";
    public static string ResetPortalsDefault = "Q";
    public static string ResetPortalsTooltip = "Remove all active portals.";

    public static string FirstPersonGhost = "First Person Ghost";
    public static string FirstPersonGhostDefault = "G";
    public static string FirstPersonGhostTooltip = "First person ghost perspective";

    public static string ToggleUI = "ToggleUI";
    public static string ToggleUIDefault = "Z";
    public static string ToggleUITooltip = "Toggles off all UI.";

    public static string LevelEditorSpeedIncrease = "Level Editor Speed Increase";
    public static string LevelEditorSpeedIncreaseDefault = "KeypadPlus";
    public static string LevelEditorSpeedIncreaseTooltip = "Increases editor movement speed";

    public static string LevelEditorSpeedDecrease = "Level Editor Speed Decrease";
    public static string LevelEditorSpeedDecreaseDefault = "KeypadMinus";
    public static string LevelEditorSpeedDecreaseTooltip = "Decreases editor movement speed";

    public static string LevelEditorSelectXAxis = "Level Editor Select X Axis [Modified]";
    public static string LevelEditorSelectXAxisDefault = "A";
    public static string LevelEditorSelectXAxisTooltip = "Selects the x axis of an object's transform in the inspector";

    public static string LevelEditorSelectYAxis = "Level Editor Select Y Axis [Modified]";
    public static string LevelEditorSelectYAxisDefault = "S";
    public static string LevelEditorSelectYAxisTooltip = "Selects the y axis an object's transform in the inspector";

    public static string LevelEditorSelectZAxis = "Level Editor Select Z Axis [Modified]";
    public static string LevelEditorSelectZAxisDefault = "D";
    public static string LevelEditorSelectZAxisTooltip = "Selects the z axis of an object's transform in the inspector";

    public static string LevelEditorSelectSnap = "Level Editor Select Snap [Modified]";
    public static string LevelEditorSelectSnapDefault = "F";
    public static string LevelEditorSelectSnapTooltip = "Selects the snap field of the currently inspected object's transform";

    public static string LevelEditorPlayTest = "Level Editor Play Test [Modified]";
    public static string LevelEditorPlayTestDefault = "R";
    public static string LevelEditorPlayTestTooltip = "Switches to/from play test mode in the level editor";
    
    public static string LevelEditorSelectPosition = "Level Editor Select Position [Modified]";
    public static string LevelEditorSelectPositionDefault = "Q";
    public static string LevelEditorSelectPositionTooltip = "Selects the position manipulation type in the inspector";
    
    public static string LevelEditorSelectRotation = "Level Editor Select Rotation [Modified]";
    public static string LevelEditorSelectRotationDefault = "W";
    public static string LevelEditorSelectRotationTooltip = "Selects the rotation manipulation type in the inspector";

    public static string LevelEditorSelectScale = "Level Editor Select Scale [Modified]";
    public static string LevelEditorSelectScaleDefault = "E";
    public static string LevelEditorSelectScaleTooltip = "Selects the scale manipulation type in the inspector";

    public static string ModifierKey = "Modifier Key";
    public static string ModifierKeyDefault = "LeftShift";
    public static string ModifierKeyTooltip = "When pressed at the same time, modifies the action of other keys";


    // Tooltips
    public static string SensitivityTooltip = "Mouse Sensitivity.";
    public static string MasterVolumeTooltip = "Master Volume.";
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
    public static string DeleteLevelDataTooltip = "Clears all level times and ghosts. Does not impact leaderboards.";
    public static string DeleteLevelEditorDataTooltip = "Clears all level editor data. This cannot be undone.";

    public static List<string> UnitOfSpeedOptions = new List<string>()
    {
        "Metres per second",
        "Kilometres per hour",
        "Miles per hour"
    };

    public static string HotKeyPattern = "(?<=\\[)[^\\]]*(?=\\])";


    // Non-changeable hotkeys
    public static string MouseX = "Mouse X";
    public static string MouseY = "Mouse Y";
    public static KeyCode PauseMenu = KeyCode.Escape;
    public static KeyCode DebugMenu = KeyCode.BackQuote;
    public static KeyCode NextTutorial = KeyCode.Tab;

    public static KeyCode LevelEditor_PlayTest = KeyCode.Return;

    public static KeyCode WinMenu_NextLevel = KeyCode.E;
    public static KeyCode WinMenu_RetryLevel = KeyCode.R;
    public static KeyCode WinMenu_MainMenu = KeyCode.Q;


    // Game Constants
    public static int MainMenuSceneIndex = 0;
    public static int LevelEditorSceneIndex = 27;
    public static int CreditsSceneIndex = 28;

    public static Vector3 PlayerSpawnOffset = new Vector3(0, 1.1f, 0);
    public static string levelCompletionTimeFormat = "mm':'ss'.'fff";
    public static int PlayerLayer = 12;
    public static int PortalMaterialLayer = 10;
    public static int PortalLayer = 11;
    public static int GizmoLayer = 15;
    public static int GhostLayer = 16;
    public static int GhostPortalLayer = 17;
    public static int CloneLayer = 18;
    public static string PortalWallTag = "PortalWall";
    public static string PlayerTag = "Player";
    public static string PortalTag = "Portal";

    // UI Constants
    public static Color activeColor = new Color(.58f, .93f, .76f);
    public static Color inactiveColor = new Color(0.5f, 0.5f, 0.5f);
    public static Color hoverColor = new Color(1, 1, 1);
	
	public static string DiscordURL = "https://discord.gg/nSkNTMn";

}
