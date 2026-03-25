using System;
using System.Collections.Generic;

namespace ConsoleApp_FirstApp
{
    /// <summary>
    /// Главный класс консольного приложения «Galaxy News».
    /// </summary>
    class Program
    {
        /// <summary>
        /// Точка входа в программу. Выводит приветствие и информацию о галактиках.
        /// </summary>
        /// <param name="args">Аргументы командной строки (не используются).</param>
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Galaxy News!");
            IterateThroughList();
            Console.ReadKey();
        }

        /// <summary>
        /// Перебирает список галактик и выводит сведения о каждой на консоль.
        /// </summary>
        private static void IterateThroughList()
        {
            var theGalaxies = new List<Galaxy>
            {
                new Galaxy() { Name="Tadpole",               MegaLightYears=400,  GalaxyType=new GType('S')},
                new Galaxy() { Name="Pinwheel",              MegaLightYears=25,   GalaxyType=new GType('S')},
                new Galaxy() { Name="Cartwheel",             MegaLightYears=500,  GalaxyType=new GType('L')},
                new Galaxy() { Name="Small Magellanic Cloud",MegaLightYears=.2,   GalaxyType=new GType('I')},
                new Galaxy() { Name="Andromeda",             MegaLightYears=3,    GalaxyType=new GType('S')},
                new Galaxy() { Name="Maffei 1",              MegaLightYears=11,   GalaxyType=new GType('E')}
            };

            foreach (Galaxy theGalaxy in theGalaxies)
            {
                Console.WriteLine(theGalaxy.Name + "  " + theGalaxy.MegaLightYears + ",  " + theGalaxy.GalaxyType);
            }

            // Ожидаемый вывод:
            //  Tadpole  400,  Spiral
            //  Pinwheel  25,  Spiral
            //  Cartwheel  500,  Lenticular
            //  Small Magellanic Cloud  .2,  Irregular
            //  Andromeda  3,  Spiral
            //  Maffei 1  11,  Elliptical
        }
    }

    /// <summary>
    /// Представляет галактику с именем, расстоянием и типом.
    /// </summary>
    public class Galaxy
    {
        /// <summary>Название галактики.</summary>
        public string Name { get; set; }

        /// <summary>Расстояние до галактики в мегасветовых годах.</summary>
        public double MegaLightYears { get; set; }

        /// <summary>Тип галактики (объект <see cref="GType"/>).</summary>
        public object GalaxyType { get; set; }
    }

    /// <summary>
    /// Определяет тип галактики по символьному коду.
    /// </summary>
    public class GType
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GType"/> по символьному коду.
        /// </summary>
        /// <param name="type">
        /// Символ типа: 'S' — Spiral, 'E' — Elliptical, 'I' — Irregular, 'L' — Lenticular.
        /// </param>
        public GType(char type)
        {
            switch (type)
            {
                case 'S':
                    MyGType = Type.Spiral;
                    break;
                case 'E':
                    MyGType = Type.Elliptical;
                    break;
                // ИСПРАВЛЕНИЕ: было 'l' (строчная L) — не совпадало с передаваемым 'I' (заглавная I).
                // Это приводило к тому, что MyGType оставался null, и в консоль выводилась пустая строка.
                case 'I':
                    MyGType = Type.Irregular;
                    break;
                case 'L':
                    MyGType = Type.Lenticular;
                    break;
                default:
                    break;
            }
        }

        /// <summary>Текущий тип галактики.</summary>
        public object MyGType { get; set; }

        /// <summary>Перечисление допустимых типов галактик.</summary>
        private enum Type { Spiral, Elliptical, Irregular, Lenticular }
    }
}