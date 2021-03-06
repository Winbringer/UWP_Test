﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BackGroundTasks
{
   public sealed class AudioPlayback : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            taskInstance.Task.Completed += TaskCompleted;
        }

        void TaskCompleted(object sender, object args)
        {
            _deferral.Complete();
        }
    }
}
