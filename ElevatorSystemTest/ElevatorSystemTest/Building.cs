using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystemTest
{
    public class Building
    {
        public int FloorCount { get; set; } // Количество этажей
        private int passengerIdCounter = 1; // Глобальный счетчик пассажиров
        public List<Elevator> Elevators { get; set; } // Список лифтов
        private Random random = new Random(); // Генератор случайных чисел
        public List<Passenger> Passengers { get; private set; } = new List<Passenger>();
        private StreamWriter writer; // Добавление StreamWriter

        public Building(int floorCount, StreamWriter writer)
        {
            FloorCount = floorCount;
            Elevators = new List<Elevator>();
            this.writer = writer;
        }

        public void AddElevator(Elevator elevator)
        {
            Elevators.Add(elevator);
        }

        // Генерация пассажиров с правильными текущими этажами
        public void GeneratePassengers(int count)
        {
            Passengers.Clear();
            for (int i = 0; i < count; i++)
            {
                int currentFloor = random.Next(1, FloorCount + 1);
                int desiredFloor;
                do
                {
                    desiredFloor = random.Next(1, FloorCount + 1);
                } while (desiredFloor == currentFloor);

                Passengers.Add(new Passenger(i + 1, currentFloor, desiredFloor)
                {
                    IsPickedUp = false,
                    IsDelivered = false
                });
            }

            writer.WriteLine("Сгенерированы пассажиры:");
            foreach (var passenger in Passengers)
            {
                writer.WriteLine(passenger);
            }
        }

        public void DispatchElevators()
        {
            // Текущие этажи пассажиров
            var currentFloors = Passengers
                .Where(p => !p.IsPickedUp && !p.IsDelivered)
                .Select(p => p.CurrentFloor)
                .Distinct()
                .OrderBy(f => f);

            writer.WriteLine($"ТЕКУЩИЕ ЭТАЖИ (пассажиры ждут лифт): {string.Join(", ", currentFloors)}");

            foreach (var passenger in Passengers.Where(p => !p.IsPickedUp && !p.IsDelivered).ToList())
            {
                var closestElevator = Elevators
                    .Where(e => e.Load < e.Capacity)
                    .OrderBy(e => Math.Abs(e.CurrentFloor - passenger.CurrentFloor))
                    .FirstOrDefault();

                if (closestElevator != null)
                {
                    closestElevator.MoveToFloor(passenger.CurrentFloor);
                    closestElevator.PickUpPassenger(Passengers, Elevators, passenger);
                }
            }

            // Желанные этажи пассажиров
            foreach (var elevator in Elevators)
            {
                if (elevator.Passengers.Any(p => !p.IsDelivered))
                {
                    writer.WriteLine($"Перед обработкой: Лифт №{elevator.Number}, ЖЕЛАЕМЫЕ ЭТАЖИ: {string.Join(", ", elevator.Passengers.Select(p => p.DesiredFloor).Distinct().OrderBy(f => f))}");
                    elevator.DeliverPassengers();
                    writer.WriteLine($"После обработки: Лифт №{elevator.Number}, Загрузка: {elevator.Load}/{elevator.Capacity}");
                }
            }
        }

        public int GeneratePassengerCount(int floorCount)
        {
            if (floorCount <= 12)
                return random.Next(100) < 95 ? random.Next(0, 6) : random.Next(7, 9);
            else return random.Next(100) < 95 ? random.Next(0, 9) : random.Next(9, 11);
        }

        public void DisplayInfo()
        {
            writer.WriteLine($"Количество этажей: {FloorCount}");
            if (Elevators.Count == 0)
            {
                writer.WriteLine("В здании нет лифтов.");
                return;
            }
            writer.WriteLine($"Количество лифтов: {Elevators.Count}\n");
            writer.WriteLine("Информация о лифтах:\n");
            foreach (var elevator in Elevators)
            {
                writer.WriteLine(elevator);
            }
        }
    }
}
