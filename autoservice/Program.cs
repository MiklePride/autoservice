using System;
using System.Collections.Generic;

namespace autoservice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Autoservice autoservice = new Autoservice();
            autoservice.Start();
        }
    }
}

class Autoservice
{
    private Queue<Car> _cars = new Queue<Car>();
    private Warehouse _warehouse = new Warehouse();

    private int _money = 50000;
    private int _pricePerJob = 3000;
    private int _carsCount = 0;

    public Autoservice()
    {
        int carCount = 10;

        for (int i = 0; i < carCount; i++)
        {
            _cars.Enqueue(new Car());
        }
    }

    public void Start()
    {
        while (_cars.Count > 0)
        {
            ShowInfo();
            ServiceCar();
        }

        Console.ReadLine();
    }

    public void ShowInfo()
    {
        Console.WriteLine($"баланс: {_money}\n машин в очереди: {_cars.Count}\n\n\n");
    }

    private void ServiceCar()
    {
        _carsCount++;

        Car car = _cars.Dequeue();

        car.ShowInfo(_carsCount);

        RepairCarPart(car.GetCarPart());
    }

    private void RepairCarPart(CarPart damagedCarPart)
    {
        bool isNeedReplacement = damagedCarPart.IsDamaged == true;
        bool isFound = _warehouse.TryGetCarPart(ref damagedCarPart);

        if (isFound && isNeedReplacement)
        {
            Console.WriteLine($"{damagedCarPart.Name} успешно заменено");
            AcceptPayment(damagedCarPart.Price);
        }
        else if (isFound && isNeedReplacement == false)
        {
            Console.WriteLine("Вы заменили ненужную деталь!");
            CompensateForDamage(damagedCarPart.Price);
        }
        else
        {
            Console.WriteLine($"{damagedCarPart.Name} закончились на складе!");
            PayFine();
        }
    }

    private void AcceptPayment(int priceCarPart)
    {
        int totalPayment = priceCarPart + _pricePerJob;
        _money += totalPayment;

        Console.WriteLine($"+{totalPayment}р. за успешный ремонт.");
    }

    private void CompensateForDamage(int priceCarPart)
    {
        _money -= priceCarPart;

        Console.WriteLine($"-{priceCarPart}р. в качестве компенсации.");
    }

    private void PayFine()
    {
        _money -= _pricePerJob;

        Console.WriteLine($"штраф: -{_pricePerJob}р.");
    }
}

class Warehouse
{
    private static Random _random = new Random();

    private List<CarPart> _CarParts = new List<CarPart>();

    private int _capacity = 30;

    public Warehouse()
    {
        for (int i = 0; i < _capacity; i++)
        {
            AddRandomCarPart();
        }
    }

    public bool TryGetCarPart(ref CarPart carPart)
    {
        bool isFound = false;

        for (int i = 0; i < _CarParts.Count; i++)
        {
            if (_CarParts[i].Name == carPart.Name)
            {
                carPart = _CarParts[i];

                _CarParts.RemoveAt(i);

                isFound = true;

                break;
            }
        }

        return isFound;
    }

    private void AddRandomCarPart()
    {
        CarPart[] carParts = new CarPart[]
        {
            new Engine(),
            new Transmission(),
            new ExhaustSystem(),
            new Windshield(),
            new Wheels()
        };

        int randomNumber = _random.Next(carParts.Length);

        _CarParts.Add(carParts[randomNumber].GetClone());
    }
}

class Car
{
    private static Random _random = new Random();

    private List<CarPart> _carParts = new List<CarPart>();

    public Car()
    {
        _carParts.Add(new Engine());
        _carParts.Add(new Transmission());
        _carParts.Add(new ExhaustSystem());
        _carParts.Add(new Windshield());
        _carParts.Add(new Wheels());

        BreakDownRandomCarPart();
    }

    public void ShowInfo(int numberInQueue)
    {
        int carPartCount = 0;

        Console.WriteLine($"____________Машина№ {numberInQueue}____________");

        foreach (var carPart in _carParts)
        {
            carPartCount++;

            Console.Write(carPartCount);

            carPart.ShowInfo();
        }

        Console.WriteLine("\n");
    }

    public CarPart GetCarPart()
    {
        bool isWork = true;

        Console.Write($"Введите номер детали, которую необходимо заменить:");

        int userInput = 0;

        while (isWork)
        {
            userInput = UserUtils.GetNumber();

            Console.WriteLine();

            userInput--;

            if (userInput < _carParts.Count && userInput >= 0)
            {
                isWork = false;
            }
            else
            {
                Console.WriteLine("Такой детали нет! Попробуйте снова.");
            }
        }

        return _carParts[userInput];
    }

    private void BreakDownRandomCarPart()
    {
        int index = _random.Next(_carParts.Count);

        _carParts[index].BreakDown();
    }
}

abstract class CarPart
{
    protected string _condition;

    public CarPart()
    {
        IsDamaged = false;
        _condition = "Отличное";
    }

    public string Name { get; protected set; }
    public int Price { get; protected set; }
    public bool IsDamaged { get; protected set; }

    public abstract CarPart GetClone();

    public void ShowInfo()
    {
        Console.WriteLine($"Деталь: {Name}  |  Цена: {Price}  |  Состояние:{_condition}");
    }

    public void BreakDown()
    {
        IsDamaged = true;
        _condition = "Требуется ремонт!";
    }
}

class Engine : CarPart
{
    public Engine() : base()
    {
        Name = "Двигатель";
        Price = 35000;
    }

    public override CarPart GetClone()
    {
        return new Engine();
    }
}

class Transmission : CarPart
{
    public Transmission() : base()
    {
        Name = "Коробка передач";
        Price = 18000;
    }

    public override CarPart GetClone()
    {
        return new Transmission();
    }
}

class ExhaustSystem : CarPart
{
    public ExhaustSystem() : base()
    {
        Name = "Выхлопная система";
        Price = 12000;
    }

    public override CarPart GetClone()
    {
        return new ExhaustSystem();
    }
}

class Windshield : CarPart
{
    public Windshield() : base()
    {
        Name = "Лобовое стекло";
        Price = 8500;
    }

    public override CarPart GetClone()
    {
        return new Windshield();
    }
}

class Wheels : CarPart
{
    public Wheels() : base()
    {
        Name = "Колеса";
        Price = 6850;
    }

    public override CarPart GetClone()
    {
        return new Wheels();
    }
}

static class UserUtils
{
    public static int GetNumber()
    {
        bool isNumberWork = true;
        int userNumber = 0;

        while (isNumberWork)
        {
            bool isNumber;
            string userInput = Console.ReadLine();

            if (isNumber = int.TryParse(userInput, out int number))
            {
                userNumber = number;
                isNumberWork = false;
            }
            else
            {
                Console.WriteLine($"Не правильный ввод данных!!!  Повторите попытку");
            }
        }
        return userNumber;
    }
}