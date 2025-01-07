using System.Diagnostics;
using System.Text;

namespace ConsoleApp14
{
    /*
    public class Herb
    {
        public int HerbValue()
        {
            return Grow + Productiviti + Protection;
        }
        public const int MaxValue = 33;
        public int Grow
        {
            get
            {
                return Grow;
            }
            set
            {
                if (Grow - 3 > value)
                    if (value <= 0)
                    {
                        Grow = 0;
                        return;
                    }
                    else if (value >= MaxValue)
                    {
                        Grow = MaxValue;
                        return;
                    }
            }
        }
        public int Productiviti
        {
            get
            {
                return Productiviti;
            }
            set
            {
                if (value <= 0)
                {
                    Productiviti = 0;
                    return;
                }
                else if (value >= MaxValue)
                {
                    Productiviti = MaxValue;
                    return;
                }
            }
        }
        public int Protection
        {
            get
            {
                return Protection;
            }
            set
            {
                if (value <= 0)
                {
                    Productiviti = 0;
                    return;
                }
                else if (value >= MaxValue)
                {
                    Productiviti = MaxValue;
                    return;
                }
            }
        }
    }
    */

    public class Gen
    {
        public Gen(bool zeroGen = false)
        {
            BestValue = 1000;
            MaxValue = 2000;
            MinValue = 0;
            if (zeroGen)
            {
                Value = 0;
                return;
            }
            Random rand = new Random();
            Value = rand.Next(800, 1200);
        }
        public Gen(int value)
        {
            Value = value;
            MaxValue = 2000;
            MinValue = 0;
        }
        public int Value { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public int BestValue { get; set; }
        public int WorthGen()
        {
            int minHarmfulVal = MinValue + BestValue / 2;
            int maxHarmfulVal = MaxValue + BestValue / 2;
            if(Value > maxHarmfulVal)
                return -1 * (Value - maxHarmfulVal);
            if (Value < minHarmfulVal)
                return -1 * (minHarmfulVal - Value);
            if (Value > BestValue)
                return maxHarmfulVal - Value;
            if (Value < BestValue)
                return Value - minHarmfulVal;
            return BestValue;
        }
        public int GetValue() 
        {
            return Value-1000; 
        }
    }
    public class Chromosome : IComparable<Chromosome>
    {
        public Gen[] Gens { get; set; }
        public Chromosome(int lenght)
        {
            Gens = new Gen[lenght];
            for (int i = 0; i < lenght;i++)
            {
                Gens[i] = new Gen(false);
            }
        }
        public int TempValue = 0;
        public Chromosome OneMaterSeleсtion(Chromosome secondParent, int mutationChance)
        {
            Random rand = new Random();
            Chromosome child = new Chromosome(Gens.Length);
            child.Gens = new Gen[Gens.Length];
            for (int i = 0; i < Gens.Length; i++)
            {
                int numberOfChanceTakeSecondParentGen = rand.Next(0,100);
                child.Gens[i] = new Gen(true);
                if (numberOfChanceTakeSecondParentGen > 50)
                {
                    int parentValueGen = secondParent.Gens[i].Value;
                    if(mutationChance != 0)
                    {
                        int numberOfChanceMutation = rand.Next(0, 100);
                        if (numberOfChanceMutation <= mutationChance)
                            parentValueGen += rand.Next(-200, 200);
                    }
                        
                    child.Gens[i].Value = parentValueGen;
                }
                else
                {
                    child.Gens[i].Value = Gens[i].Value;
                }
            }
            return child;
        }
        private int Value()
        {
            int answer = 0;
            foreach(var item in Gens)
            {
                answer += item.WorthGen();
            }
            TempValue = answer;
            return answer;
        }
        public int Value(bool force)
        {
            if (force)
                return Value();
            if(TempValue == 0)
            {
                return Value();
            }
            return TempValue;
        }
        public int CompareTo(Chromosome? other)
        {
            if(other is null) throw new ArgumentException($"Некорректное значение {nameof(other)}");
            return Value(false) - other.Value(false);
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Gen Value: {Value(false)} Gens: ");
            foreach (var item in Gens)
            {
                stringBuilder.Append($" {item.GetValue()} ");
            }
            return stringBuilder.ToString();
        }
    }
    public class Chromosomes
    {
        public List<Chromosome> ChildChromosomesArray { get; set; }
        public List<Chromosome> ChromosomesArray { get; set; }
        public int GensCount { get; private set; }
        public Chromosomes(int lenght, int countGen)
        {
            GensCount = countGen;
            MutationProccent = 25;
            ParentProccentToBe = 50;
            ChromosomesArray = new List<Chromosome>();
            for (int i = 0; i < lenght;i++)
            {
                ChromosomesArray.Add(new Chromosome(countGen));
            }
        }
        public Chromosome LuckyWheel()
        {
            Dictionary<int, Chromosome> luckList = new Dictionary<int, Chromosome>();
            int luckyChance = 1;
            int negativeLuck = 0;
            var orderList = ChromosomesArray.OrderBy(x => x).ToList();
            if(orderList.FirstOrDefault(x => x.TempValue < 0) is not null)
            {
                negativeLuck = orderList.FirstOrDefault().TempValue * -1;
            }
            foreach (var item in orderList)
            {
                luckyChance += item.TempValue + negativeLuck;
                luckList.Add(luckyChance, item);
            }
            Random rand = new Random();
            int choosePoint = rand.Next(luckList.Keys.Sum());
            if(choosePoint - luckList.First().Key > 0)
            {
                while(choosePoint - luckList.First().Key > 0)
                {
                    choosePoint -= luckList.First().Key;
                    luckList.Remove(luckList.First().Key);
                }
                return luckList.First().Value;
            }
            else
            {
                return luckList.First().Value;
            }
        }
        public void CreateChildChoromosomesArray()
        {
            ChildChromosomesArray = new List<Chromosome>();
            for (int i = 0; i < ChromosomesArray.Count();i++)
            {
                var firstParent = LuckyWheel();
                var secondParent = LuckyWheel();
                if (firstParent.Value(false) > secondParent.Value(false))
                    ChildChromosomesArray.Add(firstParent.OneMaterSeleсtion(secondParent, MutationProccent));
                else
                    ChildChromosomesArray.Add(secondParent.OneMaterSeleсtion(firstParent, MutationProccent));
            }
        }
        public void Selection()
        {
            if(ChildChromosomesArray.DefaultIfEmpty().Count() == 0)
            {
                CreateChildChoromosomesArray();
            }
            ChildChromosomesArray.Sort();
            int startIndex = (ChromosomesArray.Count() * 100 - ChromosomesArray.Count() * ParentProccentToBe)/100;
            for (int i = startIndex; i < ChromosomesArray.Count();i++)
            {
                ChromosomesArray[i] = ChildChromosomesArray[i];
            }
            SortByDescending();
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int iteration = 1;
            foreach (var item in ChromosomesArray)
            {
                stringBuilder.AppendLine($"Choromosome #{iteration++} {item.ToString()}");
            }
            return stringBuilder.ToString();
        }
        public void ToConsole()
        {
            int iteration = 1;
            foreach (var item in ChromosomesArray)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"Хромосомма #{iteration} полезность: {item.Value(false)}: ");
                foreach (var gen in item.Gens)
                {
                    if (gen.GetValue() > 0)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{gen.GetValue()} ");
                }
                Console.Write("\n");
                iteration++;
            }
        }
        public void ChildsToConsole()
        {
            int iteration = 1;
            if (ChildChromosomesArray is null)
                return;
            foreach (var item in ChildChromosomesArray)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"Хромосомма #{iteration} полезность: {item.Value(false)}: ");
                foreach (var gen in item.Gens)
                {
                    if (gen.GetValue() > 0)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{gen.GetValue()} ");
                }
                Console.Write("\n");
                iteration++;
            }
        }
        public void Sort()
        {
            ChromosomesArray = ChromosomesArray.OrderBy(x => x).ToList();
        }
        public void SortByDescending()
        {
            ChromosomesArray = ChromosomesArray.OrderByDescending(x => x).ToList();
        }
        private int parentProccentToBe;
        public int ParentProccentToBe
        {
            get
            {
                return parentProccentToBe;
            }
            set
            {
                if (value > 100)
                {
                    parentProccentToBe = 100;
                    return;
                }
                if (value < 0)
                {
                    parentProccentToBe = 0;
                    return;
                }
                parentProccentToBe = value;
            }
        }
        private int mutationProccent;
        public int MutationProccent
        {
            get
            {
                return mutationProccent;
            }
            set
            {
                if (value > 100)
                {
                    mutationProccent = 100;
                    return;
                }
                if (value < 0)
                {
                    mutationProccent = 0;
                    return;
                }
                mutationProccent = value;
            }
        }
    }

    internal class Program
    {
        
        static void Main(string[] args)
        {
            int lenght = 0;
            int gens = 0;
            int mutation = 25;
            int parents = 50;
            var genetics = new Chromosomes(0, 0);
            int key = 1;
            void MenuToConsole()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Генная модуляция:");

                Console.Write("Хромоссомы: ");
                if (lenght <= 0)
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(lenght);
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write(" Гены: ");
                if (gens <= 0)
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ForegroundColor = ConsoleColor.Green;

                Console.Write(gens);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();

                Console.Write("Процент родителей: ");
                if (parents == 50)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (parents > 50 && parents < 75  || parents < 50 && parents > 25)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else 
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.Write(parents);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" Шанс мутации: ");

                if (mutation == 25)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (mutation > 25 && mutation < 60 || mutation < 25 && mutation > 12)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.Write(mutation);
                Console.ForegroundColor = ConsoleColor.White;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine();
                Console.WriteLine("0 - Выход\n" +
                                  "1 - Инициализация/Реинициализация\n" +
                                  "2 - Установить шанс мутации\n" +
                                  "3 - Установить процент родителей\n" +
                                  "4 - Установить количество генов\n" +
                                  "5 - Установить количество хромосом\n" +
                                  "6 - Вывести хромосомы\n" +
                                  "7 - Вывести новые хромосомы\n" +
                                  "8 - Создание новых хоромосом\n" +
                                  "9 - Скрещивание\n");
            }
            
            
            while (key > 0)
            {
                MenuToConsole();
                var userKey = Console.ReadKey();
                if (int.TryParse(userKey.KeyChar.ToString(), out key))
                {
                    if (key == 0)
                        break;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    switch (key)
                    {
                        case 1:
                            genetics = new Chromosomes(lenght, gens);
                            break;
                        case 2:
                            Console.WriteLine("Введите шанс мутации: ");
                            int.TryParse(Console.ReadLine(), out mutation);
                            if (mutation > 100)
                                mutation = 100;
                            if (mutation < 0)
                                mutation = 0;
                            break;
                        case 3:
                            Console.WriteLine("Введите процент родителей: ");
                            int.TryParse(Console.ReadLine(), out parents);
                            if (parents > 100)
                                parents = 100;
                            if (parents < 0)
                                parents = 0;
                            break;
                        case 4:
                            Console.WriteLine("Введите количество генов: ");
                            int.TryParse(Console.ReadLine(), out gens);
                            break;
                        case 5:
                            Console.WriteLine("Введите количество хромосом: ");
                            int.TryParse(Console.ReadLine(), out lenght);
                            break;
                        case 6:
                            genetics.ToConsole();
                            Console.WriteLine("Нажмите кнопку для выхода");
                            Console.ReadKey();
                            break;
                        case 7:
                            genetics.ChildsToConsole();
                            Console.WriteLine("Нажмите кнопку для выхода");
                            Console.ReadKey();
                            break;
                        case 8:
                            try
                            {
                                genetics.CreateChildChoromosomesArray();
                            }
                            catch
                            {

                            }
                            break;
                        case 9:
                            try
                            {
                                genetics.Selection();
                            }
                            catch
                            {

                            }
                            break;
                    }
                    Console.Clear();
                }
                else
                {
                    key = 1;
                }
                Console.Clear();
            }
        }
    }
}
