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
                TargetFloor = targetFloor;
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
            // Определяем направление движения
            bool goingUp = TargetFloor > CurrentFloor;

            // Создаем список всех этажей, которые нужно обработать, и сортируем их по направлению
            var orderedFloors = Passengers.Where(p => !p.IsDelivered)
                                          .Select(p => p.DesiredFloor)
                                          .Distinct()
                                          .OrderBy(f => goingUp ? f : -f)  // Сортируем по направлению
                                          .ToList();

            // Выводим информацию о желаемых этажах
            Console.WriteLine($"Лифт №{Number} обрабатывает желаемые этажи: {string.Join(", ", orderedFloors)}");

            // Обрабатываем этажи
            while (orderedFloors.Any())
            {
                // Проходим по этажам в порядке движения лифта
                foreach (var floor in orderedFloors.ToList())
                {
                    if ((goingUp && floor > CurrentFloor) || (!goingUp && floor < CurrentFloor))
                    {
                        MoveToFloor(floor); // Лифт едет на этаж

                        // Доставляем пассажиров на текущем этаже
                        var deliveredPassengers = Passengers.Where(p => p.DesiredFloor == CurrentFloor && !p.IsDelivered).ToList();
                        foreach (var passenger in deliveredPassengers)
                        {
                            passenger.IsDelivered = true; // Пассажир доставлен
                            Passengers.Remove(passenger); // Удаляем из лифта
                            Load--; // Уменьшаем загрузку лифта
                            Console.WriteLine($"Пассажир {passenger.Id} вышел на этаже {CurrentFloor}. Загрузка: {Load}/{Capacity}");
                        }

                        // Удаляем этаж после того, как он был обработан
                        orderedFloors.Remove(floor);
                    }
                }

                // Если лифт еще не доставил всех пассажиров, меняем направление
                if (orderedFloors.Any())
                {
                    goingUp = !goingUp; // Меняем направление
                                        // Пересчитываем этажи для нового направления
                    orderedFloors = Passengers.Where(p => !p.IsDelivered)
                                              .Select(p => p.DesiredFloor)
                                              .Distinct()
                                              .OrderBy(f => goingUp ? f : -f)
                                              .ToList();
                }
            }
        }


        public override string ToString()
        {
            return $"Лифт №{Number} ({Type}), Вместимость: {Capacity} человек, Загрузка: {Load} человек, Текущий этаж: {CurrentFloor}";
        }
    }
}
