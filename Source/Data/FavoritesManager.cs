using System.Collections.Generic;
using BuyAnything.Source.Settings;
using Verse;

namespace BuyAnything.Source.Data
{
    public static class FavoritesManager
    {
        private static List<string> Favorites
        {
            get
            {
                if (ModMain.Settings.favoriteItems == null)
                {
                    ModMain.Settings.favoriteItems = new List<string>();
                }

                return ModMain.Settings.favoriteItems;
            }
        }

        public static bool IsFavorite(ThingDef def)
        {
            return def != null && Favorites.Contains(def.defName);
        }

        public static void ToggleFavorite(ThingDef def)
        {
            if (def == null)
                return;

            if (Favorites.Contains(def.defName))
            {
                Favorites.Remove(def.defName);
            }
            else
            {
                Favorites.Add(def.defName);
            }

            ModMain.Instance.WriteSettings();
        }

        public static IEnumerable<string> AllFavorites
        {
            get
            {
                return Favorites;
            }
        }
    }
}