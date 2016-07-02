using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace MyRuntimeComponent
{
    public sealed class MyBackgroundTask : IBackgroundTask
    {
        volatile bool _cancelRequested = false; // прервана ли задача

        public async void Run(IBackgroundTaskInstance taskInstance)
        { 
            // оценка стоимости выполнения задачи для приложения
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            if (cost == BackgroundWorkCostValue.High)
                return;
            
            // обрабатываем прерывание задачи
            var cancel = new CancellationTokenSource();
            taskInstance.Canceled += (s, e) =>
            {
                cancel.Cancel();
                cancel.Dispose();
                _cancelRequested = true;
            };

            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();
            
            await DoWork(taskInstance);

            _deferral.Complete();
        }

        private async Task DoWork(IBackgroundTaskInstance taskInstance)
        {
            // получаем локальные настройки приложения
            var settings = ApplicationData.Current.LocalSettings;
            string Message = null;
            var triggerDetails = taskInstance.TriggerDetails as ApplicationTriggerDetails;
            if (triggerDetails != null)
            {
                object tileMessage = null;
                if (triggerDetails.Arguments.TryGetValue("Message", out tileMessage))
                {
                    if (tileMessage is string)
                    {
                        Message = (string)tileMessage;                        
                    }
                }
            }          

            int number = (int)settings.Values["number"];
            uint result = 1;
            for (uint progress = 1; progress <= number; progress++)
            {
                if (_cancelRequested) // если задача прервана, выходим из цикла
                {
                    break;
                }

                result *= progress;
                await Task.Delay(1500); // имитация долгого выполнения
                                        // рассчет процентов выполнения
                taskInstance.Progress = (uint)(progress * 100 / number); // 1 * 100 / 6
            }

            settings.Values["factorial"] = result.ToString()+" "+Message;
        }
    }
}
