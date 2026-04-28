using Celeste.Mod.DashCountMod.UI;
using Monocle;
using static Celeste.Mod.DashCountMod.DashCountModSettings;

namespace Celeste.Mod.DashCountMod.Features {
    public static class DisplayGroundTouchCountInLevel {
        private static ShowCountInGameOptions showGroundTouchCountInGame = ShowCountInGameOptions.None;

        public static void SetValue(ShowCountInGameOptions value) {
            bool wasEnabled = (showGroundTouchCountInGame != ShowCountInGameOptions.None);
            bool isEnabled = (value != ShowCountInGameOptions.None);

            // (un)hook methods
            if (isEnabled && !wasEnabled) {
                Logger.Log("DashCountMod", "Hooking level enter to add in-game ground touch counter");
                On.Celeste.Level.Begin += onLevelBegin;

            } else if (!isEnabled && wasEnabled) {
                Logger.Log("DashCountMod", "Unhooking level enter to stop adding in-game ground touch counter");
                On.Celeste.Level.Begin -= onLevelBegin;
            }

            // add/remove/update the ground touch count accordingly if we already are in a level.
            if (Engine.Scene is Level level) {
                GroundTouchCountDisplayInLevel currentDisplay = level.Entities.FindFirst<GroundTouchCountDisplayInLevel>();

                if (value == ShowCountInGameOptions.None) {
                    currentDisplay?.RemoveSelf();
                } else if (currentDisplay != null) {
                    currentDisplay.SetFormat(value);
                } else {
                    level.Add(new GroundTouchCountDisplayInLevel(level.Session, value));
                }
            }

            showGroundTouchCountInGame = value;
        }

        private static void onLevelBegin(On.Celeste.Level.orig_Begin orig, Level self) {
            orig(self);
            self.Add(new GroundTouchCountDisplayInLevel(self.Session, showGroundTouchCountInGame));
        }

    }
}
