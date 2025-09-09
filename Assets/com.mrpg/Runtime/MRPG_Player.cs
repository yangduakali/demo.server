using actor;

namespace mrpg.server {
    public class MRPG_Player : Player {
        public MRPG_NetworkEntity NetworkEntity { get; internal set; }
        public InputReciverModule InputReciverModule { get; internal set; }
    }
}
