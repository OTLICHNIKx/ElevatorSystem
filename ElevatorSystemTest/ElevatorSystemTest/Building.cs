using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystemTest
{
    public class Building
    {
        public int FloorCount { get; set;} // Количество этажей
        private int passengerIdCounter = 1; // Глобальный счетчик пассажиров
        public List<Elevator> Elevators { get; set; } // Список лифтов
        private Random random = new Random(); // Генератор случайных чисел
        public List<Passenger> Passengers { get; private set; } = new List<Passenger>();
        public Building(int floorCount)
        {
            FloorCount = floorCount;
            Elevators = new List<Elevator>();
        }

        public void AddElevator(Elevator elevator)
        {
            Elevators.Add(elevator);
        }
        // Генерация пассажиров с правильными текущими этажами
        public void GeneratePassengers(int passengerCount)
        {
            Passengers.Clear(); // Очистим старый список пассажиров

            Console.WriteLine("Генерация пассажиров:");
            for (int i = 0; i < passengerCount; i++)
            {
                int currentFloor = random.Next(1, FloorCount + 1);
                int desiredFloor = random.Next(1, FloorCount + 1);

                // Создаем нового пассажира с текущим и желаемым этажами
                var passenger = new Passenger(passengerIdCounter++, currentFloor, desiredFloor);
                Passengers.Add(passenger);
                Console.WriteLine(passenger); // Вывод информации о пассажире
            }
        }

        public void DispatchElevators()
        {
            // Сортировка пассажиров по текущему этажу
            var sortedPassengers = Passengers.OrderBy(p => p.CurrentFloor).ToList();

            foreach (var passenger in sortedPassengers)
            {
                var closestElevator = Elevators
                    .OrderBy(e => Math.Abs(e.CurrentFloor - passenger.CurrentFloor)) // Выбираем ближайший лифт
                    .FirstOrDefault();

                if (closestElevator != null)
                {
                    closestElevator.MoveToFloor(passenger.CurrentFloor); // Лифт едет на текущий этаж пассажира
                    closestElevator.PickUpPassenger(Passengers, Elevators, passenger.CurrentFloor); // Пассажир садится в лифт
                }
            }

            // Теперь обрабатываем пассажиров с учётом направления движения лифта
            foreach (var elevator in Elevators)
            {
                // Если лифт движется вниз
                if (elevator.CurrentFloor > elevator.TargetFloor)
                {
                    var passengersGoingDown = Passengers
                        .Where(p => p.CurrentFloor > elevator.CurrentFloor)
                        .OrderByDescending(p => p.CurrentFloor) // Пассажиры по убыванию этажей
                        .ToList();

                    foreach (var passenger in passengersGoingDown)
                    {
                        elevator.MoveToFloor(passenger.CurrentFloor); // Лифт едет на этаж пассажира
                        elevator.PickUpPassenger(Passengers, Elevators, passenger.CurrentFloor); // Пассажир садится в лифт
                    }
                }
                // Если лифт движется вверх
                else if (elevator.CurrentFloor < elevator.TargetFloor)
                {
                    var passengersGoingUp = Passengers
                        .Where(p => p.CurrentFloor > elevator.CurrentFloor)
                        .OrderBy(p => p.CurrentFloor) // Пассажиры по возрастанию этажей
                        .ToList();

                    foreach (var passenger in passengersGoingUp)
                    {
                        elevator.MoveToFloor(passenger.CurrentFloor); // Лифт едет на этаж пассажира
                        elevator.PickUpPassenger(Passengers, Elevators, passenger.CurrentFloor); // Пассажир садится в лифт
                    }
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
            Console.WriteLine($"Количество этажей: {FloorCount}");
            if (Elevators.Count == 0)
            {
                Console.WriteLine("В здании нет лифтов.");
                return;
            }
            Console.WriteLine($"Количество лифтов: {Elevators.Count}\n");
            Console.WriteLine("Информация о лифтах:\n");
            foreach (var elevator in Elevators)
            {
                Console.WriteLine(elevator);
            }
        }
    }
}
