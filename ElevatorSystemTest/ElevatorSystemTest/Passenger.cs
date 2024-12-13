using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystemTest
{
    public class Passenger
    {
        public int Id { get; set; }
        public int CurrentFloor { get; set; } // Этаж, где находится пассажир
        public int DesiredFloor { get; set; } // Желаемый этаж
        public bool IsDelivered { get; set; } // Флаг: пассажир доставлен
        public bool IsPickedUp { get; set; } // Пассажир в лифте
        public Passenger(int id, int currentFloor, int floorCount)
        {
            Id = id;
            CurrentFloor = currentFloor;
            Random random = new Random();
            do
            {
                DesiredFloor = random.Next(1, floorCount + 1);
            } while (DesiredFloor == CurrentFloor); // Исключаем текущий этаж
            IsDelivered = false;
            IsPickedUp = false;
        }

        public override string ToString()
        {
            return $"Пассажир {Id}: Текущий этаж - {CurrentFloor}, Желаемый этаж - {DesiredFloor}.";
        }
    }

}
