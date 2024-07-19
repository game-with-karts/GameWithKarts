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
            tier += 1;
            if (tier > BoostTier.Ultimate) {
                tier = BoostTier.Ultimate;
            }
            return tier;
        }

        public static float AsFloat(BoostTier tier) => tier switch {
            BoostTier.None => 0f,
            BoostTier.Normal => 1f,
            BoostTier.Super => 1.1f,
            BoostTier.Ultra => 1.2f,
            BoostTier.Ultimate => 1.25f,
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