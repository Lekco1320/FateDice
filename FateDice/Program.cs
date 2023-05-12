using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;

internal class Person
{
    public string Name { get; }
    public int Times { get => _times; }
    private int _times = 0;

    public Person(string name)
        => Name = name;

    public void TimesPlus()
        => _times++;

    public void Reset()
        => _times = 0;
}

internal class PersonComparer : IComparer
{
    public int Compare(object? x, object? y)
    {
        if (x is not Person || y is not Person)
        {
            return -1;
        }
        else
        {
            return ((Person)x).Times > ((Person)y).Times ? -1 : 1;
        }
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        FateDice fateDice = new FateDice();
        while (true)
        {
            fateDice.Dice();
        }
    }
}

internal class FateDice
{
    private readonly Person[] People;
    private const int OFFSET = 8;
    internal FateDice()
    {
        int n;
        while (true)
        {
            Console.Clear();
            Split('*');
            Center("FateDice | Powered by Lekco");
            Split('*');
            Center("初始化名单");
            Split('*');
            Format(WriteFormat.InputFormat, "人员数量 = ");
            bool canParse = int.TryParse(Console.ReadLine(), out n);
            if (canParse && n > 0)
            {
                break;
            }
        }
        People = new Person[n];
        for (int i = 0; i < n; i++)
        {
            Format(i + 1);
            People[i] = new Person(Console.ReadLine() ?? $"person{i + 1}");
        }
        Split('*');
        Format(WriteFormat.InfoFormat, "按任意键以继续...");
        Console.ReadKey();
    }

    public void Dice()
    {
        int n;
        foreach (Person person in People)
        {
            person.Reset();
        }
        while (true)
        {
            Console.Clear();
            Split('*');
            Center("FateDice | Powered by Lekco");
            Split('*');
            Center("抽取人员");
            Split('*');
            Format(WriteFormat.InputFormat, "抽取次数 = ");
            bool canParse = int.TryParse(Console.ReadLine(), out n);
            if (canParse && n > 0)
            {
                break;
            }
        }
        Split('*');
        for (int i = 0; i < People.Length; i++)
        {
            Format(i + 1, $"{People[i].Name}: {People[i].Times}");
            Console.WriteLine();
        }
        Split('*');
        Person[] temp = new Person[People.Length];
        for (int i = 0; i < n; i++)
        {
            RandomDice(i + 1);
            RankShow();
        }
        int more = 0;
        while (People.Length > 1 && temp[0].Times == temp[1].Times)
        {
            Format(WriteFormat.InputFormat, $"抽取次数 = {n} + {++more}", 5);
            RandomDice(n + more);
            RankShow();
        }
        Format(WriteFormat.OutputFormat, $"中签者: {temp[0].Name}", OFFSET + People.Length);
        Console.WriteLine();
        Format(WriteFormat.InfoFormat, "按任意键以继续...");
        Console.ReadKey();

        void RankShow()
        {
            People.CopyTo(temp, 0);
            Array.Sort(temp, 0, People.Length, new PersonComparer());
            for (int j = 0; j < People.Length; j++)
            {
                Format(j + 1, $"{temp[j].Name}: {temp[j].Times}", OFFSET + j - 1);
            }
            Console.WriteLine();
        }
    }

    private void RandomDice(int id)
    {
        Random random = new();
        int instance = random.Next(0, People.Length);
        Format(WriteFormat.InfoFormat, $"第{id}次结果: {People[instance].Name}", OFFSET + People.Length);
        People[instance].TimesPlus();
        Console.SetCursorPosition(0, OFFSET + People.Length + 2);
        Thread.Sleep(150);
    }

    private static void Split(char ch)
        => Console.WriteLine(new string(ch, 38));

    private static void Format(WriteFormat format, string content = "", int? line = null)
    {
        string prefix = format switch
        {
            WriteFormat.OutputFormat => ">>> ",
            WriteFormat.InputFormat => "<<< ",
            WriteFormat.InfoFormat or _ => "* ",
        };
        if (line is not null)
        {
            ClearLine(line ?? 0);
        }
        Console.Write($"{prefix}{content}");
    }

    private static void Format(int index, string s = "", int? line = null)
    {
        if (line is not null)
        {
            ClearLine(line ?? 0);
        }
        Console.Write($"* {(index > 0 && index < 10 ? "0" : "")}{index}| {s}");
    }

    private static void ClearLine(int line)
    {
        Console.SetCursorPosition(0, line);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, line);
    }

    private enum WriteFormat { OutputFormat, InputFormat, InfoFormat }

    private static void Center(string title)
    {
        Regex regex = new Regex(@"^[\u4E00-\u9FA5]{0,}$");
        int ch_count = 0;
        foreach (char c in title)
        {
            if (regex.IsMatch(c.ToString()))
            {
                ch_count++;
            }
        }
        int len = title.Length + ch_count;
        int right = (36 - len) / 2;
        int left = 36 - len - right;
        Console.WriteLine($"*{new string(' ', left)}{title}{new string(' ', right)}*");
    }
}