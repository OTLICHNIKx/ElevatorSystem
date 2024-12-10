using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorSystemTest
{
    public class ElevatorSimulation
    {
        private Building building;
        private int hours;
        private Random random; // Экземпляр Random для генерации случайных значений
        private int iterations;
        private StreamWriter writer; // Для записи в файл

        public ElevatorSimulation(Building building, StreamWriter writer)
        {
            this.building = building;
            this.writer = writer;
            this.random = new Random(); // Инициализируем объект Random
            this.iterations = random.Next(8, 15); // Случайное количество интервалов за час
        }

        public void StartSimulation(int hours)
        {
            this.hours = hours;

            writer.WriteLine($"Запуск симуляции на {hours} часов.");
            for (int i = 0; i < hours; i++)
            {
                writer.WriteLine($"  --------------------------------------------------------------------");
                writer.WriteLine($" |                               Час {i + 1}                                |");
                writer.WriteLine($"  --------------------------------------------------------------------");
                for (int j = 0; j < iterations; j++)
                {
                    int minutesPassed = random.Next(4, 8); // Случайное количество минут между 4 и 7
                    writer.WriteLine($"  --------------------------------------------------------------------");
                    writer.WriteLine($" |                          Прошло {minutesPassed} минут                            |");
                    writer.WriteLine($"  --------------------------------------------------------------------");

                    // Генерация пассажиров
                    int passengerCount = building.GeneratePassengerCount(building.FloorCount);
                    building.GeneratePassengers(passengerCount);

                    // Вывод распределения работы лифтов
                    writer.WriteLine("\nРаспределение работы лифтов:");
                    building.DispatchElevators();
                    building.DisplayInfo(); // Передача writer в метод DisplayInfo

                    // Симуляция завершена для этого интервала
                    writer.WriteLine("\n");
                }

                // После завершения работы за 1 час выводим итог
                writer.WriteLine($"  -------------------------------------------------------------------");
                writer.WriteLine($" |                        Прошёл час времени                         |");
                writer.WriteLine($"  -------------------------------------------------------------------");
            }

            writer.WriteLine($"  -------------------------------------------------------------------");
            writer.WriteLine($" |                        Симуляция завершена                        |");
            writer.WriteLine($"  -------------------------------------------------------------------");

            writer.Flush(); // Явно записать буфер на диск
        }
    }
}
