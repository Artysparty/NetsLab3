using System.Collections;
using System.Threading;

namespace NetsReal3
{
    public class FourthThread
    {
        private PostToThirdWT _post;

        private BitArray[] _receivedMessages;
        private readonly Semaphore _receiveSemaphore;
        private readonly Semaphore _sendSemaphore;

        public FourthThread(ref Semaphore sendSemaphore, ref Semaphore receiveSemaphore)
        {
            _sendSemaphore = sendSemaphore;
            _receiveSemaphore = receiveSemaphore;
        }

        public void FourthThreadMain(object obj)
        {
            _post = (PostToThirdWT) obj;
            ConsoleHelper.WriteToConsole("4 поток", "Начинаю работу.");

            //обработка файла
            while (true)
            {
                _receiveSemaphore.WaitOne();
                ConsoleHelper.WriteToConsole("4 поток", "Ожидаю кадр.");

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
                    ConsoleHelper.WriteToConsole("4 поток", "Получен конец");
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