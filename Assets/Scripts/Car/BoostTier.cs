namespace GWK.Kart {
    public enum BoostTier {
        None = 0,
        Normal = 1,
        Super = 2,
        Ultra = 3,
        Ultimate = 4,
    }
    public static class BoostTierOperations {
        public static BoostTier OneUp(BoostTier tier) {
            switch (tier) {
                case BoostTier.Ultra:
                case BoostTier.Ultimate:
                    return tier;
                default:
                    return tier + 1;
            }
        }

        public static float AsFloat(BoostTier tier) => tier switch {
            BoostTier.None => 0f,
            BoostTier.Normal => 1f,
            BoostTier.Super => 1.1f,
            BoostTier.Ultra => 1.2f,
            BoostTier.Ultimate => 1.3f,
            _ => 1f
        };

        public static string ToString(BoostTier tier) => tier switch {
            BoostTier.None => "None",
            BoostTier.Normal => "Normal",
            BoostTier.Super => "Super",
            BoostTier.Ultra => "Ultra",
            BoostTier.Ultimate => "Ultimate",
            _ => string.Empty,
        };
    }
}
