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
        private void AddPassengers(int floor, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (CurrentPassengers < Capacity)
                {
                    var targetFloor = random.Next(1, 21); // Предположим, 20 этажей
                    Passengers.Add(new Passenger(floor, targetFloor));
                    Console.WriteLine($"Лифт №{Number} добавил пассажира с целью на этаж {targetFloor}.");
                    CurrentPassengers++;
                }
                else
                {
                    Console.WriteLine($"Лифт №{Number} заполнен. Нельзя добавить больше пассажиров.");
                }
            }
        }
        // Обработка запросов лифта
        public void ProcessRequests(Dictionary<int, int> passengersOnFloors)
        {
            var currentFloors = passengersOnFloors.Where(p => p.Value > 0 && Requests.Contains(p.Key))
                                                  .Select(p => p.Key)
                                                  .OrderBy(f => f)
                                                  .ToList();

            if (currentFloors.Count == 0)
            {
                Console.WriteLine("Нет пассажиров на этажах. Лифт завершил обработку.");
                return;
            }

            var visitedFloors = new HashSet<int>();

            // Основная обработка запросов
            foreach (var currentTarget in currentFloors)
            {
                while (CurrentFloor != currentTarget)
                {
                    // Проверяем, есть ли желаемый этаж между текущим положением и следующим целевым этажом
                    var desiredInPath = Passengers.Where(p =>
                                            (p.TargetFloor > CurrentFloor && p.TargetFloor <= currentTarget) ||
                                            (p.TargetFloor < CurrentFloor && p.TargetFloor >= currentTarget))
                                                  .OrderBy(p => Math.Abs(p.TargetFloor - CurrentFloor))
                                                  .FirstOrDefault();

                    if (desiredInPath != null)
                    {
                        // Едем на ближайший желаемый этаж
                        Console.WriteLine($"Лифт №{Number} едет на желаемый этаж {desiredInPath.TargetFloor}...");
                        CurrentFloor = desiredInPath.TargetFloor;
                        Console.WriteLine($"Лифт №{Number} приехал на {CurrentFloor} этаж (желаемый для пассажира).");

                        // Высаживаем пассажиров
                        var exitingPassengers = Passengers.Where(p => p.TargetFloor == CurrentFloor).ToList();
                        if (exitingPassengers.Count > 0)
                        {
                            Console.WriteLine($"Лифт №{Number} высадил {exitingPassengers.Count} пассажиров на {CurrentFloor} этаже.");
                            foreach (var passenger in exitingPassengers)
                            {
                                Passengers.Remove(passenger);
                                Console.WriteLine($"Пассажир с целью на этаж {CurrentFloor} выходит из лифта.");
                            }
                        }
                    }
                    else
                    {
                        // Если нет подходящих желаемых этажей, едем на текущий целевой этаж
                        Console.WriteLine($"Лифт №{Number} едет на {currentTarget} этаж...");
                        CurrentFloor = currentTarget;
                        Console.WriteLine($"Лифт №{Number} приехал на {CurrentFloor} этаж (текущий целевой).");

                        // Добавляем новых пассажиров
                        if (passengersOnFloors.ContainsKey(currentTarget))
                        {
                            AddPassengers(currentTarget, passengersOnFloors[currentTarget]);
                            passengersOnFloors[currentTarget] = 0;
                        }
                    }
                }
                visitedFloors.Add(currentTarget);
            }

            Console.WriteLine("Обработка запросов завершена.");
        }
        public override string ToString()
        {
            return $"Лифт №{Number} ({Type}), Вместимость: {Capacity} человек, Текущий этаж: {CurrentFloor}, Пассажиров: {CurrentPassengers}";
        }
    }

}
