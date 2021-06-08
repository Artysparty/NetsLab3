using System;
using System.Collections;
using System.Threading;

namespace NetsReal3
{
    public delegate void PostToFirstWT(BitArray[] message);

    public delegate void PostToSecondWT(BitArray[] message);
    
    public delegate void PostToThirdWT(BitArray[] message);
    
    public delegate void PostToFourthWT(BitArray[] message);

    internal class Program
    {
        private static void Main(string[] args)
        {
            ConsoleHelper.WriteToConsole("Главный поток", "");

            var firstReceiveSemaphore = new Semaphore(0, 1);
            var secondReceiveSemaphore = new Semaphore(0, 1);
            var thirdReceiveSemaphore = new Semaphore(0, 1);
            var fourthReceiveSemaphore = new Semaphore(0, 1);

            var firstThread = new FirstThread(ref secondReceiveSemaphore, ref firstReceiveSemaphore);
            var secondThread = new SecondThread(ref firstReceiveSemaphore, ref secondReceiveSemaphore);
            var thirdThread = new ThirdThread(ref fourthReceiveSemaphore, ref thirdReceiveSemaphore);
            var fourthThread = new FourthThread(ref thirdReceiveSemaphore, ref fourthReceiveSemaphore);

            var threadFirst = new Thread(firstThread.FirstThreadMain);
            var threadSecond = new Thread(secondThread.SecondThreadMain);
            var threadThree = new Thread(thirdThread.ThirdThreadMain);
            var threadFour = new Thread(fourthThread.FourthThreadMain);

            PostToFirstWT postToFirstWt = firstThread.ReceiveData;
            PostToSecondWT postToSecondWt = secondThread.ReceiveData;
            PostToThirdWT postToThirdWt = thirdThread.ReceiveData;
            PostToFourthWT postToFourthWt = fourthThread.ReceiveData;

            threadFirst.Start(postToSecondWt);
            threadSecond.Start(postToFirstWt);
            threadThree.Start(postToFourthWt);
            threadFour.Start(postToThirdWt);

            Console.ReadLine();
        }
    }
}