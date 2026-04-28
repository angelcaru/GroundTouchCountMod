namespace Celeste.Mod.DashCountMod {
    public class DashCountModSession : EverestModuleSession {
        public int JumpCount { get; set; } = 0;
        public int JumpCountAtLevelStart { get; set; } = 0;

        public int GroundTouchCount { get; set; } = 0;
        //[YamlIgnore]
        public bool Introing { get; set; } = true;
        public bool WasGrounded { get; set; } = true;
    }
}
