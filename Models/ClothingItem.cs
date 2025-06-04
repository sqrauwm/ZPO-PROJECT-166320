using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WardrobeApp.Models
{
    public class ClothingItem : Clothing
    {
        public string Material {  get; set; }
        public bool IsFavourite { get; set; }

        public ClothingItem(int id, string name, string category, string color, string size, 
            string material, bool isFavourite) : base(id, name, category, color, size) 
        {
            Material = material;
            IsFavourite = isFavourite;
        }

        public override string GetDisplayText()
        {
            string fav = IsFavourite ? " (Ulubiony" : "";
            return $"{Name} [{Category}] - {Color}, rozmiar {Size}, materiał: {Material}{fav}";
        }
    }
}
