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
    private readonly List<ulong> _noClipPlayers = [];

    public override void Unload(bool hotReload)
    {
        _noClipPlayers.Clear();
    }

    [ConsoleCommand("css_noclip", "noclip command")]
    [RequiresPermissions("@css/cheats")]
    public void OnCmdNoclip(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null || player.Team == CsTeam.Spectator || player.Team == CsTeam.None)
            return;

        CBasePlayerPawn? playerPawn = player.Pawn.Get();
        if (playerPawn == null)
        {
            return;
        }
        SwitchNoClip(playerPawn);
    }
    
    private void SwitchNoClip(CBasePlayerPawn playerPawn)
    {
        var playerId = playerPawn.Controller.Get()?.SteamID;
        if (playerPawn.MoveType == MoveType_t.MOVETYPE_NOCLIP)
        {
            SetNormalWalk(playerPawn);
            if (playerId.HasValue)
            {
                _noClipPlayers.Remove(playerId.Value);
            }
        }
        else
        {
            SetNoClip(playerPawn);
            if (playerId.HasValue)
            {
                _noClipPlayers.Add(playerId.Value);
            }
        }
    }

    private void SetNormalWalk(CBasePlayerPawn? playerPawn)
    {
        if (playerPawn == null)
        {
            return;
        }
        playerPawn.MoveType = MoveType_t.MOVETYPE_WALK;
        Schema.SetSchemaValue(playerPawn.Handle, "CBaseEntity", "m_nActualMoveType", 2); // walk
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_MoveType");
    }

    private void SetNoClip(CBasePlayerPawn? playerPawn)
    {
        if (playerPawn == null)
        {
            return;
        }
        playerPawn.MoveType = MoveType_t.MOVETYPE_NOCLIP;
        Schema.SetSchemaValue(playerPawn.Handle, "CBaseEntity", "m_nActualMoveType", 8); // noclip
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_MoveType");
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        foreach (var player in Utilities.GetPlayers().Where(player => _noClipPlayers.Contains(player.SteamID)))
        {
            SetNoClip(player.Pawn.Value?.As<CCSPlayerPawn>());
        }

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnBotTakeOver(EventBotTakeover @event, GameEventInfo info)
    {
        var controller = @event.Userid;
        var bot = @event.Botid;
        if (controller != null && _noClipPlayers.Contains(controller.SteamID))
        {
            SetNoClip(bot?.Pawn.Value);
        }

        return HookResult.Continue;
    }
}
