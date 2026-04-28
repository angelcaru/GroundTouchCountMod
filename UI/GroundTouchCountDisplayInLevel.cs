using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.DashCountMod.UI {
    public class GroundTouchCountDisplayInLevel : AbstractCountDisplayInLevel {
        public GroundTouchCountDisplayInLevel(Session session, DashCountModSettings.ShowCountInGameOptions format)
            : base(session, format, GFX.Gui["DashCountMod/groundTouchIcon"], -10f) {
        }

        protected override int GetCountForSession() {
            return ((DashCountModSession) DashCountModModule.Instance._Session).GroundTouchCount;
        }

        protected override int GetCountForChapter() {
            return GetCountFromSaveData(ModSaveData.GroundTouchCountPerLevel);
        }
        protected override int GetCountForFile() {
            return ModSaveData.GroundTouchCountPerLevel.Values
                .Select(level => level.Values.Sum())
                .Sum();
        }
        protected override IEnumerator<AbstractCountDisplayInLevel> EnumeratePreviousCountDisplays() {
            foreach (DashCountDisplayInLevel e in Scene.Tracker.GetEntities<DashCountDisplayInLevel>().OfType<DashCountDisplayInLevel>()) {
                yield return e;
            }
        }
    }
}
