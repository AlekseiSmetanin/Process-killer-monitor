using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ProcessKillerMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length!=3)
            {
                Console.WriteLine("Аргументов командной строки должно быть 3. Работа программы завершается. Нажмите любую клавишу.");
                Console.Read();

                return;
            }

            string name=args[0];
            int lifetime;
            int frequency;

            try
            {
                //lifetime = Int32.Parse(args[1]);
                //frequency = Int32.Parse(args[2]);

                lifetime=Int32.Parse(args[1])*60000;
                frequency= Int32.Parse(args[2])*60000;

                if (lifetime <= 0 || frequency <= 0)
                    throw new ArgumentOutOfRangeException();

                Monitoring(name, lifetime, frequency);
            }
            catch(Exception exc)
            {
                Console.WriteLine("Введены недопустимые значения lifetime или frequency. Возникло исключение {0} Работа программы завершается. Нажмите любую клавишу.", exc.Message);
                Console.Read();

                return;
            }
        }

        /// <summary>
        /// Останавливает процесс с именем name, если он проработал больше, чем lifetime мс
        /// Проверка осуществляется каждые frequency мс
        /// </summary>
        /// <param name="name">Имя останавливаемого процесса</param>
        /// <param name="lifetime">Максимально допустимое время жизни процесса в миллисекундах</param>
        /// <param name="frequency">Частота проверки в миллисекундах</param>
        private static void Monitoring(string name, int lifetime, int frequency)
        {
            for (; ; )
            {
                Process[] processes = Process.GetProcesses();

                for (int i = 0; i < processes.Length; i++)
                {
                    if (processes[i].ProcessName == name)
                    {
                        try
                        {
                            if ((DateTime.Now - processes[i].StartTime).TotalMilliseconds > lifetime)
                            {
                                processes[i].Kill();
                                WriteLog(String.Format("Процесс {0} проработал {1}  и был остановлен", processes[i].ProcessName, DateTime.Now - processes[i].StartTime));
                            }
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("Не удалось остановить процесс {0}, так как возникло исключение {1}", processes[i].ProcessName, exc.Message);
                        }
                    }
                }

                Thread.Sleep(frequency);
            }
        }

        /// <summary>
        /// Записывает строку logMessage в файл log.txt, находящийся в той же папке, что и исполняемый файл
        /// </summary>
        /// <param name="logMessage">Записываемая строка</param>
        private static void WriteLog(string logMessage)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter("log.txt", true);
                streamWriter.WriteLine(logMessage);
                streamWriter.Close();
            }
            catch(Exception exc)
            {
                Console.WriteLine("Не удалось записать лог, так как возникло исключение {0}", exc.Message);
            }
        }
    }
}
