using ElevatorSystemTest;
using System.IO;

public class Program
{
    public static void Main()
    {
        Console.Write("Введите количество этажей (с 1 по 24): ");
        int floorCount = GetValidInput(1, 24);
        if (floorCount <= 5)
        {
            Console.WriteLine("Установка лифтов для здания меньше 6 этажей не является разумной.");
            return;
        }
        Console.Write("Введите количество лифтов (с 1 по 4): ");
        int elevatorCount = GetValidInput(1, 4);

        // Создаем и открываем файл для записи
        using (StreamWriter writer = new StreamWriter("simulation_output.txt"))
        {
            Building building = new Building(floorCount, writer);

            // Добавляем лифты
            for (int i = 1; i <= elevatorCount; i++)
            {
                int capacity = (i == elevatorCount || (elevatorCount >= 3 && i >= elevatorCount - 1)) ? 10 : 4;
                building.AddElevator(new Elevator(i, capacity, floorCount, writer));
            }

            Console.WriteLine("Введите количество часов работы лифта:");
            int hours = int.Parse(Console.ReadLine());

            // Передаем writer в симуляцию
            ElevatorSimulation simulation = new ElevatorSimulation(building, writer);
            simulation.StartSimulation(hours); // Запуск симуляции
            Console.WriteLine("Все данные записаны в файле");
            Console.WriteLine("Все данные о работе лифта записаны в файл simulation_output.txt");
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
