using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystemTest
{
    public class Elevator
    {
        public int Number { get; set; } // Номер лифта
        public int Capacity { get; set; } // Вместимость лифта
        public int CurrentFloor { get; set; } // Текущий этаж
        public string Type => Capacity <= 4 ? "Пассажирский" : "Грузовой"; // Тип лифта
        public int Load { get; set; } // Начальная загрузка
        public Elevator(int number, int capacity, int floorCount)
        {
            Number = number;
            Capacity = capacity;
            Random random = new Random();
            CurrentFloor = random.Next(1, floorCount + 1); // Генерация случайного этажа
            Load = 0; // Начальная загрузка
        }
        public void MoveToFloor(int targetFloor)
        {
            if (CurrentFloor != targetFloor)
            {
                Console.WriteLine($"Лифт №{Number} ({Type}) движется с {CurrentFloor} на этаж {targetFloor}");
                CurrentFloor = targetFloor;
            }
            else CurrentFloor = targetFloor;
        }
        public void PickUpPassenger(List<Elevator> allElevators, int targetFloor)
        {
            if (Load < Capacity)
            {
                Load++;
                Console.WriteLine($"Лифт №{Number} забрал пассажира на этаже {targetFloor}. Текущая загрузка: {Load}/{Capacity}");
            }
            else
            {
                Console.WriteLine($"Лифт №{Number} переполнен! Передача запроса другому лифту.");

                // Поиск ближайшего свободного лифта
                Elevator nearestElevator = allElevators
                    .Where(e => e != this && e.Load < e.Capacity) // Лифт не текущий и не переполнен
                    .OrderBy(e => Math.Abs(e.CurrentFloor - targetFloor)) // Находим ближайший
                    .FirstOrDefault();

                if (nearestElevator != null)
                {
                    Console.WriteLine($"Запрос передан лифту №{nearestElevator.Number}.");
                    nearestElevator.MoveToFloor(targetFloor);
                    nearestElevator.PickUpPassenger(allElevators, targetFloor);
                }
                else
                {
                    Console.WriteLine($"Нет доступных лифтов для забора пассажира на этаже {targetFloor}.");
                }
            }
        }
        public override string ToString()
        {
            return $"Лифт №{Number} ({Type}), Вместимость: {Capacity} человек, Загрузка: {Load} человек, Текущий этаж: {CurrentFloor}";
        }
    }
}
