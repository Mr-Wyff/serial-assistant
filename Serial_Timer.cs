using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace serial_assistant
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 用于更新时间的定时器
        /// </summary>
        private DispatcherTimer clockTimer = new DispatcherTimer();

        /// <summary>
        /// 定时器初始化
        /// </summary>
        private void InitClockTimer()
        {
            clockTimer.Interval = new TimeSpan(0, 0, 1);
            clockTimer.IsEnabled = true;
            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Start();
        }

        /// <summary>
        /// 用于自动发送串口数据的定时器
        /// </summary>
        private DispatcherTimer autoSendDataTimer = new DispatcherTimer();

        private void InitAutoSendDataTimer()
        {
            autoSendDataTimer.IsEnabled = false;
            autoSendDataTimer.Tick += AutoSendDataTimer_Tick;
        }

        private void StartAutoSendDataTimer(int interval)
        {
            autoSendDataTimer.IsEnabled = true;
            autoSendDataTimer.Interval = TimeSpan.FromMilliseconds(interval);
            autoSendDataTimer.Start();
        }

        private void StopAutoSendDataTimer()
        {
            autoSendDataTimer.IsEnabled = false;
            autoSendDataTimer.Stop();
        }
        private int GetAutoSendDataInterval()
        {
            int interval = 1000;

            if (int.TryParse(autoSendIntervalTextBox.Text.Trim(), out interval) == true)
            {
                string select = timeUnitComboBox.Text.Trim();

                switch (select)
                {
                    case "毫秒":
                        break;
                    case "秒钟":
                        interval *= 1000;
                        break;
                    case "分钟":
                        interval = interval * 60 * 1000;
                        break;
                    default:
                        break;
                }
            }

            return interval;
        }
    }
}
