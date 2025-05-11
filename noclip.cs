using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

public class Noclip : BasePlugin
{
    public override string ModuleName => "noclip";
    public override string ModuleVersion => "1.0.0+upstream.1.0.2";
    public override string ModuleAuthor => "exkludera, SNWCreations";
    public override string ModuleDescription => "";

    [ConsoleCommand("css_noclip", "noclip command")]
    [RequiresPermissions("@css/cheats")]
    public void OnCmdNoclip(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null || !player.PawnIsAlive || player.Team == CsTeam.Spectator || player.Team == CsTeam.None)
            return;

        CCSPlayerPawn? playerPawn = player.PlayerPawn.Value;
        if (playerPawn == null)
        {
            return;
        }
        if (playerPawn.MoveType == MoveType_t.MOVETYPE_NOCLIP)
        {
            playerPawn.MoveType = MoveType_t.MOVETYPE_WALK;
            Schema.SetSchemaValue(playerPawn.Handle, "CBaseEntity", "m_nActualMoveType", 2); // walk
            Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_MoveType");
        }
        else
        {
            playerPawn.MoveType = MoveType_t.MOVETYPE_NOCLIP;
            Schema.SetSchemaValue(playerPawn.Handle, "CBaseEntity", "m_nActualMoveType", 8); // noclip
            Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_MoveType");
        }
    }
    
}
