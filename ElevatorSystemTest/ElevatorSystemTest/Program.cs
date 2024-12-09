using ElevatorSystemTest;

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

        Building building = new Building(floorCount);

        // Добавляем лифты
        for (int i = 1; i <= elevatorCount; i++)
        {
            int capacity = (i == elevatorCount || (elevatorCount >= 3 && i >= elevatorCount - 1)) ? 10 : 4;
            building.AddElevator(new Elevator(i, capacity, floorCount));
        }

        building.DisplayInfo();


        building.GeneratePassengers(building.GeneratePassengerCount(floorCount));

        Console.WriteLine("\nРаспределение работы лифтов:");
        building.DispatchElevators();

        Console.Write("\n");
        building.DisplayInfo();
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