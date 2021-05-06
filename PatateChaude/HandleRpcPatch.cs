using HardelAPI.Utility;
using HarmonyLib;
using Hazel;
using System.Linq;

namespace PatateChaud {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class HandleRpcPatch {

        public static bool Prefix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader) {
            if (callId == (byte) CustomRPC.SendPatato) {
                PlayerControl player = PlayerControlUtils.FromPlayerId(reader.ReadByte());

                if (player != null) {
                    Button.allPlayersTargetable = PlayerControl.AllPlayerControls.ToArray().ToList();
                    Button.allPlayersTargetable.RemovePlayer(player);
                    HotPatatoes.Instance.AllPlayers = new System.Collections.Generic.List<PlayerControl>();
                    HotPatatoes.Instance.AllPlayers.Add(PlayerControlUtils.FromPlayerId(reader.ReadByte()));
                }

                return false;
            }

            return true;
        }
    }
}
