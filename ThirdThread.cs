using System;
using System.Collections;
using System.IO;
using System.Threading;

namespace NetsReal3
{
    public class ThirdThread
    {
        private PostToFourthWT _post;

        private BitArray[] _receivedMessages;
        private readonly Semaphore _receiveSemaphore;
        private readonly Semaphore _sendSemaphore;

        public ThirdThread(ref Semaphore sendSemaphore, ref Semaphore receiveSemaphore)
        {
            _sendSemaphore = sendSemaphore;
            _receiveSemaphore = receiveSemaphore;
        }

        public void ThirdThreadMain(object obj)
        {
            _post = (PostToFourthWT) obj;

            ConsoleHelper.WriteToConsole("3 поток", "Начинаю работу.");

            var fileBytes =
                File.ReadAllBytes("C:\\Users\\artem\\Dropbox\\Мой ПК (LAPTOP-V6M1QK29)\\Desktop\\Nets\\text.txt");

            var split = Utils.SplitFile(fileBytes);
            //отправка файла


            foreach (var s in split)
            {
                var frame = new Frame();

                frame.Control = new BitArray(16);

                frame.Data = new BitArray(s);

                frame.Checksum = Utils.DecimalToBinary(Utils.CheckSum(frame.Data));

                var frameBitArr = frame.ToBitArray();
                _post(new[] {frameBitArr});
                _sendSemaphore.Release();

                _receiveSemaphore.WaitOne();

                ConsoleHelper.WriteToConsole("3 поток", "Ожидаю квитанцию.");

                var fileReceipt = Frame.Parse(_receivedMessages[0]);

                var fileControl = new byte[2];
                fileReceipt.Control.CopyTo(fileControl, 0);

                if (fileControl[0] == 31)
                    ConsoleHelper.WriteToConsole("3 поток", "Квитанция true получена.");
                else if (fileControl[0] == 32) ConsoleHelper.WriteToConsole("3 поток", "Квитанция false получена.");
            }

            var endFrame = new Frame();

            endFrame.Control = new BitArray(16);
            endFrame.Control.Write(0, Utils.DecimalToBinary(90));
            endFrame.Checksum = Utils.DecimalToBinary(0);
            endFrame.Data = new BitArray(16);

            _post(new[] {endFrame.ToBitArray()});

            _sendSemaphore.Release();
        }

        public void ReceiveData(BitArray[] arrays)
        {
            _receivedMessages = arrays;
        }
    }
}