using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WardrobeApp.Interfaces;

namespace WardrobeApp.Models
{
    public abstract class Clothing : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Color {  get; set; }
        public string Size { get; set; }

        protected Clothing(int id, string name, string category, string color, string size)
        {
            Id = id;
            Name = name;
            Category = category;
            Color = color;
            Size = size;
        }

        public abstract string GetDisplayText();
    }
}
