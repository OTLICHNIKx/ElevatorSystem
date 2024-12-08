using System.Drawing;

namespace ElevatorSystem
{
    public class Elevator
    {
        public int Number { get; set; }
        public int Capacity { get; set; }
        public string Type => Capacity <= 4 ? "Пассажирский" : "Грузовой";
        public int CurrentFloor { get; private set; }
        public List<int> Requests { get; private set; }
        private int currentPassengers;
        public int CurrentPassengers
        {
            get { return currentPassengers; }
            set { currentPassengers = value; }
        }
        public int Direction { get; private set; } = 0; // 1: вверх, -1: вниз, 0: стоим
        private Random random;
        public List<Passenger> Passengers { get; set; } = new List<Passenger>();
        private List<int> passengersInside; // Список пассажиров в лифте

        public Elevator(int number, int capacity, int floorCount, Random random)
        {
            Number = number;
            Capacity = capacity;
            CurrentFloor = random.Next(1, floorCount + 1); // Генерация случайного начального этажа
            Requests = new List<int>();
            this.random = random;
            CurrentPassengers = 0; // Изначально лифт пуст
            passengersInside = new List<int>();
        }

        public void AddRequest(int destinationFloor)
        {
            // Проверяем, есть ли уже запрос на этот этаж
            if (!Requests.Contains(destinationFloor))
            {
                Requests.Add(destinationFloor);
                Console.WriteLine($"Лифт №{Number} добавил запрос на этаж {destinationFloor}.");
            }
        }

        // Обработка запросов лифта
        public void ProcessRequests(Dictionary<int, int> passengersOnFloors)
        {
            // 1. Собираем этажи, на которых есть пассажиры для текущего лифта
            var currentFloors = passengersOnFloors.Where(p => p.Value > 0 && Requests.Contains(p.Key)).Select(p => p.Key).ToList();

            if (currentFloors.Count == 0)
            {
                Console.WriteLine("Нет пассажиров на этажах. Лифт завершил обработку.");
                return; // Нет пассажиров, лифт завершает работу
            }

            // 2. Находим максимальный этаж среди текущих этажей
            int targetFloor = currentFloors.Max();

            // 3. Пассажиры не должны повторно добавляться на лифт, если этаж уже был обработан
            var visitedFloors = new HashSet<int>();

            // 4. Сначала едем на максимальный этаж
            Console.WriteLine($"Лифт №{Number} едет на {targetFloor} этаж...");
            visitedFloors.Add(targetFloor);

            // Пассажиры, которые находятся на текущем этаже лифта, входят в лифт
            var enteringPassengers = passengersOnFloors.ContainsKey(targetFloor) ? passengersOnFloors[targetFloor] : 0;
            if (enteringPassengers > 0)
            {
                AddPassengers(targetFloor, enteringPassengers);
                passengersOnFloors[targetFloor] -= enteringPassengers;
            }

            // Переезжаем на максимальный этаж
            CurrentFloor = targetFloor;

            // 5. Теперь двигаемся вниз, если есть запросы на этажи ниже
            var downRequests = Requests.Where(r => r < CurrentFloor).OrderByDescending(r => r).ToList();

            foreach (var downRequest in downRequests)
            {
                if (!visitedFloors.Contains(downRequest))
                {
                    Console.WriteLine($"Лифт №{Number} едет на {downRequest} этаж...");
                    visitedFloors.Add(downRequest);
                    CurrentFloor = downRequest;

                    // На этом этапе лифт уже забрал всех пассажиров на текущем этаже, можно начинать высадку
                    var exitingPassengers = Passengers.Where(p => p.TargetFloor == downRequest).ToList();
                    if (exitingPassengers.Count > 0)
                    {
                        Console.WriteLine($"Лифт №{Number} высадил {exitingPassengers.Count} пассажиров на {downRequest} этаже.");
                        foreach (var passenger in exitingPassengers)
                        {
                            Passengers.Remove(passenger); // Удаляем пассажира из лифта
                            Console.WriteLine($"Пассажир с целью на этаж {downRequest} выходит из лифта.");
                        }
                    }

                    // Пассажиры могут входить на этажах
                    var entering = passengersOnFloors.ContainsKey(downRequest) ? passengersOnFloors[downRequest] : 0;
                    if (entering > 0)
                    {
                        AddPassengers(downRequest, entering);
                        passengersOnFloors[downRequest] -= entering;
                    }
                }
            }

            // 6. Теперь двигаемся вверх по запросам
            var upRequests = Requests.Where(r => r > CurrentFloor).OrderBy(r => r).ToList();

            foreach (var upRequest in upRequests)
            {
                if (!visitedFloors.Contains(upRequest))
                {
                    Console.WriteLine($"Лифт №{Number} едет на {upRequest} этаж...");
                    visitedFloors.Add(upRequest);
                    CurrentFloor = upRequest;

                    // На этом этапе лифт уже забрал всех пассажиров на текущем этаже, можно начинать высадку
                    var exitingUpPassengers = Passengers.Where(p => p.TargetFloor == upRequest).ToList();
                    if (exitingUpPassengers.Count > 0)
                    {
                        Console.WriteLine($"Лифт №{Number} высадил {exitingUpPassengers.Count} пассажиров на {upRequest} этаже.");
                        foreach (var passenger in exitingUpPassengers)
                        {
                            Passengers.Remove(passenger); // Удаляем пассажира из лифта
                            Console.WriteLine($"Пассажир с целью на этаж {upRequest} выходит из лифта.");
                        }
                    }

                    // Пассажиры могут входить на этажах
                    var enteringUp = passengersOnFloors.ContainsKey(upRequest) ? passengersOnFloors[upRequest] : 0;
                    if (enteringUp > 0)
                    {
                        AddPassengers(upRequest, enteringUp);
                        passengersOnFloors[upRequest] -= enteringUp;
                    }
                }
            }

            // Итоговый вывод состояния
            Console.WriteLine($"\nЛифт №{Number} завершил обработку запросов.");
            Console.WriteLine($"Текущий этаж: {CurrentFloor}, Пассажиров в лифте: {CurrentPassengers}.\n");
        }


        private void ProcessDirection(List<int> sortedRequests, Dictionary<int, int> passengersOnFloors, bool isGoingUp)
        {
            foreach (var targetFloor in sortedRequests)
            {
                // Лифт едет на этаж назначения
                Console.WriteLine($"Лифт №{Number} едет на {targetFloor} этаж...");

                // 1. Пассажиры, которые находятся на текущем этаже лифта, входят в лифт
                if (CurrentFloor == targetFloor)
                {
                    var enteringPassengers = passengersOnFloors.ContainsKey(CurrentFloor) ? passengersOnFloors[CurrentFloor] : 0;
                    if (enteringPassengers > 0)
                    {
                        AddPassengers(CurrentFloor, enteringPassengers);
                        passengersOnFloors[CurrentFloor] -= enteringPassengers;
                    }
                }

                // Переезжаем на желаемый этаж
                CurrentFloor = targetFloor;

                // 2. Высадка пассажиров
                var exitingPassengers = Passengers.Where(p => p.TargetFloor == targetFloor).ToList();
                if (exitingPassengers.Count > 0)
                {
                    Console.WriteLine($"Лифт №{Number} высадил {exitingPassengers.Count} пассажиров на {targetFloor} этаже.");
                    foreach (var passenger in exitingPassengers)
                    {
                        Passengers.Remove(passenger); // Удаляем пассажира из лифта
                        Console.WriteLine($"Пассажир с целью на этаж {targetFloor} выходит из лифта.");
                    }
                }

                // 3. Пассажиры могут входить на этажах, если лифт не на целевом этаже
                if (CurrentFloor != targetFloor)
                {
                    var enteringPassengers = passengersOnFloors.ContainsKey(CurrentFloor) ? passengersOnFloors[CurrentFloor] : 0;
                    if (enteringPassengers > 0)
                    {
                        AddPassengers(CurrentFloor, enteringPassengers);
                        passengersOnFloors[CurrentFloor] -= enteringPassengers;
                    }
                }

                // Обновляем текущее количество пассажиров после высадки
                CurrentPassengers = Passengers.Count;
            }
        }


        private void AddPassengers(int targetFloor, int passengersToEnter)
        {
            int availableSpace = Capacity - CurrentPassengers;
            int enteringPassengersCount = Math.Min(availableSpace, passengersToEnter);

            for (int i = 0; i < enteringPassengersCount; i++)
            {
                Passengers.Add(new Passenger(targetFloor)); // Добавляем пассажира с нужным этажом назначения
            }

            CurrentPassengers += enteringPassengersCount;
            Console.WriteLine($"Лифт №{Number} теперь содержит {CurrentPassengers} пассажиров.");
        }

        public override string ToString()
        {
            return $"Лифт №{Number} ({Type}), Вместимость: {Capacity} человек, Текущий этаж: {CurrentFloor}, Пассажиров: {CurrentPassengers}";
        }
    }

}
