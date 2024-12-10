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
        public int TargetFloor { get; set; } // Целевой этаж (для направления движения)
        public string Type => Capacity <= 4 ? "Пассажирский" : "Грузовой"; // Тип лифта
        public int Load { get; set; } // Начальная загрузка
        private static Random Random = new Random(); // Статический объект Random для использования в классе
        public List<Passenger> Passengers { get; private set; } = new List<Passenger>();
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
                Console.WriteLine($"Лифт №{Number} движется с {CurrentFloor} на этаж {targetFloor}.");
                CurrentFloor = targetFloor;
            }
        }

        public void PickUpPassenger(List<Passenger> passengers, List<Elevator> allElevators, Passenger passengerToPickUp)
        {
            if (passengerToPickUp == null)
                return;

            if (Load < Capacity)
            {
                Passengers.Add(passengerToPickUp);
                passengerToPickUp.IsPickedUp = true; // Отмечаем, что пассажир забран
                Load++;
                Console.WriteLine($"Лифт №{Number} забрал пассажира {passengerToPickUp.Id} на этаже {passengerToPickUp.CurrentFloor}. Загрузка: {Load}/{Capacity}");
            }
            else
            {
                Console.WriteLine($"Лифт №{Number} переполнен! Передача запроса другому лифту.");

                // Поиск ближайшего свободного лифта
                Elevator nearestElevator = allElevators
                    .Where(e => e != this && e.Load < e.Capacity) // Лифт не текущий и не переполнен
                    .OrderBy(e => Math.Abs(e.CurrentFloor - passengerToPickUp.CurrentFloor)) // Находим ближайший
                    .FirstOrDefault();

                if (nearestElevator != null)
                {
                    Console.WriteLine($"Запрос передан лифту №{nearestElevator.Number}.");
                    nearestElevator.MoveToFloor(passengerToPickUp.CurrentFloor);
                    nearestElevator.PickUpPassenger(passengers, allElevators, passengerToPickUp);
                }
                else
                {
                    Console.WriteLine($"Нет доступных лифтов для забора пассажира на этаже {passengerToPickUp.CurrentFloor}.");
                }
            }
        }
        public void DeliverPassengers()
        {
            var delivered = Passengers.Where(p => p.DesiredFloor == CurrentFloor).ToList();
            foreach (var passenger in delivered)
            {
                passenger.IsDelivered = true; // Помечаем пассажира как доставленного
                Passengers.Remove(passenger); // Удаляем из текущего списка лифта
                Load--;
                Console.WriteLine($"Пассажир {passenger.Id} вышел на этаже {CurrentFloor}. Загрузка: {Load}/{Capacity}");
            }
        }
        public override string ToString()
        {
            return $"Лифт №{Number} ({Type}), Вместимость: {Capacity} человек, Загрузка: {Load} человек, Текущий этаж: {CurrentFloor}";
        }
    }
}
