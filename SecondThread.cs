using System.Collections;
using System.Threading;

namespace NetsReal3
{
    public class SecondThread
    {
        private PostToFirstWT _post;

        private BitArray[] _receivedMessages;
        private readonly Semaphore _receiveSemaphore;
        private readonly Semaphore _sendSemaphore;

        public SecondThread(ref Semaphore sendSemaphore, ref Semaphore receiveSemaphore)
        {
            _sendSemaphore = sendSemaphore;
            _receiveSemaphore = receiveSemaphore;
        }

        public void SecondThreadMain(object obj)
        {
            _post = (PostToFirstWT) obj;
            ConsoleHelper.WriteToConsole("2 поток", "Начинаю работу.");

            _receiveSemaphore.WaitOne();

            var frame = Frame.Parse(_receivedMessages[0]);

            var controlBytes = new byte[2];
            frame.Control.CopyTo(controlBytes, 0);

            //200 - запрос на подключение, ответ 201, 
            //иначе ответ 202, программа заканчивает работу
            //30 - кадр с данными
            //31 - квитанция о получении
            //32 - квитанция об отсутствии или повреждении данных

            if (controlBytes[0] == 200)
            {
                var resp = Utils.CreateTrueConnection();
                _post(new[] {resp.ToBitArray()});
                _sendSemaphore.Release();
            }

            //обработка файла
            while (true)
            {
                _receiveSemaphore.WaitOne();
                ConsoleHelper.WriteToConsole("2 поток", "Ожидаю кадр.");

                for (var i = 0; i < _receivedMessages.Length; i++)
                    ConsoleHelper.WriteToConsoleArray("Кадр", _receivedMessages[i]);

                var response = new Frame();

                response.Control = new BitArray(16);
                response.Control.Write(0, Utils.DecimalToBinary(31));
                response.Checksum = Utils.DecimalToBinary(0);
                response.Data = new BitArray(16);

                _post(new[] {response.ToBitArray()});
                _sendSemaphore.Release();

                var receipt = Frame.Parse(_receivedMessages[0]);

                var control = new byte[2];
                receipt.Control.CopyTo(control, 0);

                if (control[0] == 90)
                {
                    ConsoleHelper.WriteToConsole("2 поток", "Получен конец");
                    break;
                }
            }
        }

        public void ReceiveData(BitArray[] arrays)
        {
            _receivedMessages = arrays;
        }
    }
}