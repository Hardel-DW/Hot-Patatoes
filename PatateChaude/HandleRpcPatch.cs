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
                PlayerControl parent = PlayerControlUtils.FromPlayerId(reader.ReadByte());

                if (player != null) {
                    Button.button.allPlayersTargetable = PlayerControl.AllPlayerControls.ToArray().ToList();
                    if (HotPatatoes.DontParent.GetValue())
                        Button.button.allPlayersTargetable.RemovePlayer(parent);
                    
                    HotPatatoes.Instance.AllPlayers = new System.Collections.Generic.List<PlayerControl>() { player };
                    HotPatatoes.Instance.DefineVisibleByWhitelist();
                }
                return false;
            }

            return true;
        }
    }
}
