using System;
using System.Collections.Generic;
using ElevatorSystem;
public class Program
{
    public static async Task Main()
    {
        Console.Write("Введите количество этажей (с 1 по 24): ");
        int floorCount = GetValidInput(1, 24);

        Console.Write("Введите количество лифтов (с 1 по 4): ");
        int elevatorCount = GetValidInput(0, 4);

        Building building = new Building(floorCount);

        for (int i = 1; i <= elevatorCount; i++)
        {
            int capacity = (i == elevatorCount || (elevatorCount >= 3 && i >= elevatorCount - 1)) ? 10 : 4;
            building.AddElevator(new Elevator(i, capacity, floorCount, new Random()));
        }

        Console.WriteLine("\nИнформация о здании:\n");
        building.DisplayInfo();

        Console.WriteLine("\nГенерация пассажиров:\n");
        building.GeneratePassengers();

        Console.WriteLine("\nОбработка запросов лифтов:\n");
        building.ProcessElevatorRequests();


        // После обработки запросов можно добавить финальный статус лифтов
        Console.WriteLine("\nСтатус лифтов после обработки запросов:");
        foreach (var elevator in building.Elevators)
        {
            Console.WriteLine($"Лифт №{elevator.Number}: Текущий этаж: {elevator.CurrentFloor}, Пассажиров: {elevator.CurrentPassengers}/{elevator.Capacity}");
        }
    }

    private static int GetValidInput(int min, int max)
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int result) && result >= min && result <= max)
            {
                return result;
            }
            Console.WriteLine($"Ошибка! Введите число от {min} до {max}.");
        }
    }
}