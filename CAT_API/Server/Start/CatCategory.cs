using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionsServer
{
    public class CatCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Rus_name { get; set; }

        public string Category(string words)
        {
            List<CatCategory> availableCategories = new List<CatCategory>
            {
                new CatCategory { Id = 5, Name = "boxes" , Rus_name = "коробка"},
                new CatCategory { Id = 6, Name = "caturday", Rus_name = "суббота"},
                new CatCategory { Id = 15, Name = "clothes", Rus_name = "одежда" },
                new CatCategory { Id = 9, Name = "dream", Rus_name = "мечта" },
                new CatCategory { Id = 3, Name = "funny", Rus_name = "забавный" },
                new CatCategory { Id = 1, Name = "hats", Rus_name = "шляпа" },
                new CatCategory { Id = 14, Name = "sinks", Rus_name = "раковина" },
                new CatCategory { Id = 2, Name = "space", Rus_name = "космос" },
                new CatCategory { Id = 4, Name = "sunglasses", Rus_name = "очки" },
                new CatCategory { Id = 7, Name = "ties", Rus_name = "галстук" }
            };

            string[] word = words.Split(' ');

            foreach (string i in word)
            {
                int dlina = (i.Length - i.Length / 2 - 1);
                //Console.WriteLine(dlina);
                string searchword = i.Substring(0, i.Length - dlina);
                //Console.WriteLine(searchword);

                //string searchword = i;

                foreach (CatCategory category in availableCategories)
                {
                    if ((category.Name.ToLower().Contains(searchword.ToLower()) || category.Rus_name.ToLower().Contains(searchword.ToLower())) && searchword.Length > 2)
                    {
                        Console.WriteLine(category.Id);
                        return category.Id.ToString();

                    }
                }
            }
            return "Ошибка"; 
        }

    }
}
