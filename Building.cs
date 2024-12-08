using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorSystem;


    public class Building
    {
        public int FloorCount { get; set; } // Количество этажей
        private int passengerIdCounter = 1; // Глобальный счетчик пассажиров
        public List<Elevator> Elevators { get; set; } // Список лифтов
        private Random random = new Random(); // Генератор случайных чисел
        private Dictionary<int, int> PassengersOnFloors; // Пассажиры на каждом этаже

        public Building(int floorCount)
        {
            FloorCount = floorCount;
            Elevators = new List<Elevator>();
            PassengersOnFloors = new Dictionary<int, int>();
        }

        public void AddElevator(Elevator elevator)
        {
            Elevators.Add(elevator);
        }

        public void CallElevator(int floor)
        {
            Console.WriteLine($"Пассажир на этаже {floor} нажал кнопку вызова лифта.");

            // Разделяем вызовы на выше и ниже
            var elevatorsOnFloor = Elevators.Where(e => e.CurrentFloor == floor).ToList();
            var elevator = Elevators.OrderBy(e => Math.Abs(e.CurrentFloor - floor)).First();
            elevator.AddRequest(floor);
        }

        public void ProcessAllElevators()
        {
            foreach (var elevator in Elevators)
            {
                if (elevator.Requests.Count > 0) // Лифт должен обрабатывать запросы, только если они есть
                    elevator.ProcessRequests(PassengersOnFloors);
            }
        }

        public void GeneratePassengers()
        {
            int totalPassengerCount = GeneratePassengerCount(FloorCount); // Общее количество пассажиров
            if (totalPassengerCount == 0) return; // Если пассажиров нет, ничего не делаем
            HashSet<int> selectedFloors = GenerateUniqueFloors(random.Next(FloorCount / 4, FloorCount));

            foreach (int floor in selectedFloors)
            {
                int passengersOnThisFloor = random.Next(1, totalPassengerCount / 2 + 1);
                if (!PassengersOnFloors.ContainsKey(floor))
                    PassengersOnFloors[floor] = 0;
                PassengersOnFloors[floor] += passengersOnThisFloor;

                Console.WriteLine($"\nНа этаже {floor} появились {passengersOnThisFloor} пассажира(ов).\n");

                // Генерация запросов для каждого пассажира
                for (int i = 0; i < passengersOnThisFloor; i++)
                {
                    int destinationFloor;
                    do
                    {
                        destinationFloor = random.Next(1, FloorCount + 1); // Случайный этаж назначения
                    } while (destinationFloor == floor); // Этаж назначения не должен совпадать с текущим

                    // Добавляем запрос для лифта
                    var nearestElevator = Elevators
                        .OrderBy(e => Math.Abs(e.CurrentFloor - floor))
                        .First();
                    Console.WriteLine($"Пассажир на {floor} хочет поехать на {destinationFloor} этаж");
                    // Сначала добавляем запрос на этаж с пассажиром
                    nearestElevator.AddRequest(floor);
                    nearestElevator.CurrentPassengers++;
                    // Затем добавляем запрос на этаж назначения
                    nearestElevator.AddRequest(destinationFloor);
                }

                totalPassengerCount -= passengersOnThisFloor;
                if (totalPassengerCount <= 0) break;
            }
        }

        public void ProcessElevatorRequests()
        {
            foreach (var kvp in PassengersOnFloors)
            {
                var nearestElevator = Elevators
                    .OrderBy(e => Math.Abs(e.CurrentFloor - kvp.Key)) // Сортировка лифтов по ближайшему этажу
                    .First();

                // Сначала лифт едет на этаж, где находится пассажир, затем на этаж назначения
                nearestElevator.AddRequest(kvp.Key); // Добавляем запрос на этаж с пассажиром
            }

            foreach (var elevator in Elevators)
            {
                elevator.ProcessRequests(PassengersOnFloors);
            }
        }

        private HashSet<int> GenerateUniqueFloors(int count)
        {
            HashSet<int> selectedFloors = new HashSet<int>();
            while (selectedFloors.Count < count)
            {
                int floor = random.Next(1, FloorCount + 1);
                selectedFloors.Add(floor);
            }
            Console.WriteLine($"Сгенерированы этажи: {string.Join(", ", selectedFloors)}");
            return selectedFloors;
        }

        private int GeneratePassengerCount(int floorCount)
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

        // Асинхронная обработка запросов лифтов с добавлением параметра totalFloors
        public async Task ProcessAllElevatorsAsync()
        {
            var tasks = new List<Task>();

            foreach (var elevator in Elevators)
            {
                if (elevator.Requests.Count > 0)
                {

                }
            }

            foreach (var elevator in Elevators)
            {
                // Передаем totalFloors в метод ProcessRequestsAsync

            }

            await Task.WhenAll(tasks); // Ожидание завершения всех задач
        }
    }

