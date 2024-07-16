namespace GWK.Kart {
    class BoostTier {
        private int tier;
        private BoostTier(int tier) => this.tier = tier;

        public static readonly BoostTier None = new BoostTier(0);
        public static readonly BoostTier Normal = new BoostTier(1);
        public static readonly BoostTier Super = new BoostTier(2);
        public static readonly BoostTier Ultra = new BoostTier(3);

        public static explicit operator float(BoostTier tier) => tier.tier switch {
            0 => 0f,
            1 => .9f,
            2 => 1f,
            3 => 1.15f,
            _ => 1f
        };
    }
}