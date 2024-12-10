using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        private StreamWriter _writer; // Для записи логов

        public Elevator(int number, int capacity, int floorCount, StreamWriter writer)
        {
            Number = number;
            Capacity = capacity;
            CurrentFloor = Random.Next(1, floorCount + 1); // Генерация случайного этажа
            Load = 0; // Начальная загрузка
            _writer = writer; // Переданный StreamWriter
        }

        public void MoveToFloor(int targetFloor)
        {
            if (CurrentFloor != targetFloor)
            {
                int floorDifference = Math.Abs(CurrentFloor - targetFloor);
                double timePerFloor = Random.NextDouble() * (2.5 - 1.7) + 1.7;
                double timeToMove = timePerFloor * floorDifference;

                string direction = CurrentFloor < targetFloor ? "вверх" : "вниз";
                string logMessage = $"Лифт №{Number} ({Type}) движется с {CurrentFloor} на этаж {targetFloor} ({direction}). Время в пути: {timeToMove:F1} секунд.";
                _writer.WriteLine(logMessage);

                TargetFloor = targetFloor;
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
                string logMessage = $"Лифт №{Number} забрал пассажира {passengerToPickUp.Id} на этаже {passengerToPickUp.CurrentFloor}. Загрузка: {Load}/{Capacity}";
                _writer.WriteLine(logMessage);
            }
            else
            {
                string logMessage = $"Лифт №{Number} переполнен! Передача запроса другому лифту.";
                _writer.WriteLine(logMessage);

                Elevator nearestElevator = allElevators
                    .Where(e => e != this && e.Load < e.Capacity)
                    .OrderBy(e => Math.Abs(e.CurrentFloor - passengerToPickUp.CurrentFloor))
                    .FirstOrDefault();

                if (nearestElevator != null)
                {
                    logMessage = $"Запрос передан лифту №{nearestElevator.Number}.";
                    _writer.WriteLine(logMessage);

                    nearestElevator.MoveToFloor(passengerToPickUp.CurrentFloor);
                    nearestElevator.PickUpPassenger(passengers, allElevators, passengerToPickUp);
                }
                else
                {
                    logMessage = $"Нет доступных лифтов для забора пассажира на этаже {passengerToPickUp.CurrentFloor}.";
                    _writer.WriteLine(logMessage);
                }
            }
        }

        public void DeliverPassengers()
        {
            bool goingUp = TargetFloor > CurrentFloor;
            var orderedFloors = Passengers.Where(p => !p.IsDelivered)
                                          .Select(p => p.DesiredFloor)
                                          .Distinct()
                                          .OrderBy(f => goingUp ? f : -f)
                                          .ToList();

            string logMessage = $"Лифт №{Number} обрабатывает желаемые этажи: {string.Join(", ", orderedFloors)}";
            _writer.WriteLine(logMessage);

            while (orderedFloors.Any())
            {
                foreach (var floor in orderedFloors.ToList())
                {
                    if ((goingUp && floor > CurrentFloor) || (!goingUp && floor < CurrentFloor))
                    {
                        MoveToFloor(floor);

                        var deliveredPassengers = Passengers.Where(p => p.DesiredFloor == CurrentFloor && !p.IsDelivered).ToList();
                        foreach (var passenger in deliveredPassengers)
                        {
                            passenger.IsDelivered = true;
                            Passengers.Remove(passenger);
                            Load--;
                            logMessage = $"Пассажир {passenger.Id} вышел на этаже {CurrentFloor}. Загрузка: {Load}/{Capacity}";
                            _writer.WriteLine(logMessage);
                        }

                        orderedFloors.Remove(floor);
                    }
                }

                if (orderedFloors.Any())
                {
                    goingUp = !goingUp;
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
