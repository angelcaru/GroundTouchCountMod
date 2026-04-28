using Monocle;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.DashCountMod.Features {
    public static class CustomGroundTouchCounting {
        public static void Load() {
            On.Celeste.Player.Update += onPlayerUpdate;
            On.Celeste.SaveData.RegisterCompletion += onSaveDataRegisterCompletion;
        }

        public static void Unload() {
            On.Celeste.Player.Update -= onPlayerUpdate;
            On.Celeste.SaveData.RegisterCompletion -= onSaveDataRegisterCompletion;
        }

        private static DashCountModSaveData ModSaveData {
            get {
                return (DashCountModSaveData) DashCountModModule.Instance._SaveData;
            }
        }

        private static DashCountModSession ModSession {
            get {
                return (DashCountModSession) DashCountModModule.Instance._Session;
            }
        }

        public static void AddTouch(Entity entityInScene) {
            ModSession.GroundTouchCount++;

            if (entityInScene.Scene != null) {
                AreaKey area = entityInScene.SceneAs<Level>().Session.Area;

                if (ModSaveData.GroundTouchCountPerLevel.TryGetValue(area.GetSID(), out Dictionary<AreaMode, int> groundTouchCounts)) {
                    if (groundTouchCounts.TryGetValue(area.Mode, out int _)) {
                        // area and mode stats exist, we should increment it
                        groundTouchCounts[area.Mode]++;
                    } else {
                        // area stats exist, mode stats don't
                        groundTouchCounts[area.Mode] = 1;
                    }
                } else {
                    // area stats don't exist, create them
                    Dictionary<AreaMode, int> areaStats = new Dictionary<AreaMode, int> {
                        [area.Mode] = 1
                    };
                    ModSaveData.GroundTouchCountPerLevel[area.GetSID()] = areaStats;
                }
            }
        }

        private static void onPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self) {
            if (self.InControl) ModSession.Introing = false;
            bool onGround = (bool)self.GetType().GetField("onGround", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self) && !ModSession.Introing;
            if (!ModSession.WasGrounded && onGround) {
                AddTouch(self);
            }
            ModSession.WasGrounded = onGround;
            orig(self);
        }

        private static void onSaveDataRegisterCompletion(On.Celeste.SaveData.orig_RegisterCompletion orig, SaveData self, Session session) {
            orig(self, session);
            if (!session.StartedFromBeginning) return;

            int? oldGroundTouchCount = null;

            if (ModSaveData.BestGroundTouchCountPerLevel.TryGetValue(session.Area.GetSID(), out Dictionary<AreaMode, int> groundTouchCounts)) {
                if (groundTouchCounts.TryGetValue(session.Area.Mode, out int _)) {
                    // area and mode stats exist
                    oldGroundTouchCount = groundTouchCounts[session.Area.Mode]++;
                }
            } else {
                // area stats don't exist, create them
                ModSaveData.BestGroundTouchCountPerLevel[session.Area.GetSID()] = new Dictionary<AreaMode, int>();
            }

            if (oldGroundTouchCount == null || oldGroundTouchCount > ModSession.JumpCount) {
                // create or update the best groundTouch count
                ModSaveData.BestGroundTouchCountPerLevel[session.Area.GetSID()][session.Area.Mode] = ModSession.GroundTouchCount;
            }
        }
    }
}
