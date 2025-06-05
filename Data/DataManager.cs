using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using WardrobeApp.Exceptions;
using WardrobeApp.Models;

namespace WardrobeApp.Data
{
    public class DataManager
    {
        private readonly List<Clothing> _wardrobe = new List<Clothing>();

        public void AddClothing(Clothing item)
        {
            if (item == null)
                throw new WardrobeException("Próba dodania pustego elementu garderoby");

            if (_wardrobe.Any(c => c.Id == item.Id))
                throw new WardrobeException($"Element o Id={item.Id} już istnieje.");
            
            _wardrobe.Add(item);
        }

        public void RemoveClothing(int id)
        {
            var item = _wardrobe.FirstOrDefault(c => c.Id == id);
            if (item == null)
                throw new WardrobeException($"Nie znaleziono elementu o Id={id}.");

            _wardrobe.Remove(item);
        }

        public List<Clothing> GetAllClothing()
        {
            return _wardrobe.ToList();
        }

        public List<Clothing> SearchByName(string fragmentName)
        {
            if (string.IsNullOrEmpty(fragmentName))
                return GetAllClothing();

            return _wardrobe
                .Where(c => c.Name.IndexOf(fragmentName, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }

        public List<Clothing> FilterByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return GetAllClothing();

            return _wardrobe
                .Where(c => c.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();  
        }

        public void SaveToJson(string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(_wardrobe, options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new DataLoadException("Błąd podczas zapisywania danych do pliku JSON.", ex);
            }
        }

        public void LoadFromJson(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return;

                string json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                    return;

                var list = JsonSerializer.Deserialize<List<ClothingItem>>(json);
                if (list == null)
                    return;

                _wardrobe.Clear();
                foreach (var item in list)
                {
                    _wardrobe.Add(item);
                }
            }
            catch (Exception ex) when (!(ex is DataLoadException))
            {
                throw new DataLoadException("Błąd podczas wczytywania danych z pliku JSON", ex);
            }
        }

    }
}
