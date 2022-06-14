using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO.Ports;


using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace serial_assistant
{
 
    public partial class MainWindow : Window
    {
        private SerialPort serialPort;

        public int txcount = 0;


        private void InitSerialPort()
        {
            serialPort.DataReceived += SerialPort_DataReceived;
            InitCheckTimer();
        }
        /// <summary>
        /// 初始化当前机器上可用的串口列表并添加到串口的下拉列表框中
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void InitializeSerialPortList()
        {
            // 获取本机串口的数组
            var portsList = SerialPort.GetPortNames();
            //绑定到串口对应下拉列表框中
            cbPort.ItemsSource = portsList;
            // 选择列表中第一个项
            cbPort.SelectedIndex = 0;
            cbPort.Text = portsList[0];
        }
        /// <summary>
        /// 关闭串口的方法
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool CloseSerialPort()
        {
            bool flag = false;
            try
            {
                serialPort.Close();
                flag = true;
            }
            catch (Exception)
            {

                throw;
            }


            return flag;
        }

        /// <summary>
        /// 打开串口的方法
        /// </summary>
        /// <returns></returns>
        private bool OpenSerialPort()
        {
            bool flag = false;

            try
            {
                // 创建串口对象
                serialPort = new SerialPort();
                // 设置串口名称
                serialPort.PortName = cbPort.Text;
                //设置波特率
                serialPort.BaudRate = int.Parse(cbBaudRate.Text);
                // 设置串口的数据位
                serialPort.DataBits = int.Parse(cbDataBits.Text);
                // 设置串口停止位
                serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cbStopBits.Text);
                // 设置串口校验位
                serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), cbParity.Text);

                //serialPort.Encoding = Encoding.GetEncoding("GB2312");
                serialPort.Open();
                InitSerialPort();
                flag = true;
            }
            catch (Exception)
            {

                throw;
            }

            return flag;
        }

       /// <summary>
       /// <summary>
       /// 定义打开串口的事件
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>

       private Encoding GetSelectedEncoding()
       {
           string select = encodingComboBox.Text;

           Encoding enc = Encoding.Default;

           if (select.Contains("UTF-8"))
           {
               enc = Encoding.UTF8;
           }
           else if (select.Contains("ASCII"))
           {
               enc = Encoding.ASCII;
           }
           else if (select.Contains("Unicode"))
           {
               enc = Encoding.Unicode;
           }

           return enc;
       }
        private bool SerialPortWrite(string textData)
        {
            SerialPortWrite(textData, false);
            return false;
        }

        private string appendContent = "\n";
        private bool SerialPortWrite(string textData, bool reportEnable)
        {
            if (serialPort == null)
            {
                MessageBox.Show("串口未打开，无法发送数据。");

                return false;
            }

            if (serialPort.IsOpen == false)
            {
                MessageBox.Show("串口未打开，无法发送数据。");
                return false;
            }

            try
            {
                //serialPort.DiscardOutBuffer();
                //serialPort.DiscardInBuffer();

                if (sendMode == SendMode.Character)
                {
                    serialPort.Write(textData + appendContent);
                }
                else if (sendMode == SendMode.Hex)
                {
                    string[] grp = textData.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    List<byte> list = new List<byte>();

                    foreach (var item in grp)
                    {
                        list.Add(Convert.ToByte(item, 16));
                    }

                    serialPort.Write(list.ToArray(), 0, list.Count);
                }

                if (reportEnable)
                {
                    // 报告发送成功的消息，提示用户。
                    //Information(string.Format("成功发送：{0}。", textData));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        #region 定时器
        // 需要一个定时器用来，用来保证即使缓冲区没满也能够及时将数据处理掉，防止一直没有到达
        // 阈值，而导致数据在缓冲区中一直得不到合适的处理。
        private DispatcherTimer checkTimer = new DispatcherTimer();
        /// <summary>
        /// 超时时间为50ms
        /// </summary>
        private const int TIMEOUT = 50;
        private void InitCheckTimer()
        {
            // 如果缓冲区中有数据，并且定时时间达到前依然没有得到处理，将会自动触发处理函数。
            checkTimer.Interval = new TimeSpan(0, 0, 0, 0, TIMEOUT);
            checkTimer.IsEnabled = false;
            checkTimer.Tick += CheckTimer_Tick;
        }

        private void StartCheckTimer()
        {
            checkTimer.IsEnabled = true;
            checkTimer.Start();
        }

        private void StopCheckTimer()
        {
            checkTimer.IsEnabled = false;
            checkTimer.Stop();
        }
        #endregion


        private bool SendData()
        {
            string textToSend = sendDataTextBox.Text;
            string Data_String = sendDataTextBox.Text.Replace(" ", ""); //获取字符串并去除空格

            //支持中文发送，应该有其他的更好地处理方法，这种方法比较传统
            Encoding gb = System.Text.Encoding.GetEncoding("gb2312");
            byte[] bytes = gb.GetBytes(sendDataTextBox.Text);
            gb.GetBytes(sendDataTextBox.Text);

            if (serialPort==null)
            {
                
                MessageBox.Show("串口未打开！！！");
                return false;

            }

            if (string.IsNullOrEmpty(textToSend))
            {
                //Alert("要发送的内容不能为空！");
                MessageBox.Show("要发送的内容不能为空\r\n");
                return false;
            }

            if (autoSendEnableCheckBox.IsChecked == true)
            {
                //return SerialPortWrite(textToSend, false);
                //字符串模式
                if (sendMode == SendMode.Character)
                {
                    serialPort.Write(bytes, 0, bytes.Length);
                    txcount += bytes.Length;
                    TxCount.Text = txcount.ToString();

                    return true;
                }
                else
                {
                    txcount += Data_String.Length / 2;
                    TxCount.Text = txcount.ToString();
                    return SerialPortWrite(textToSend, false);
                }
            }
            else
            {
                //字符串模式
                if(sendMode == SendMode.Character)
                {
                    serialPort.Write(bytes, 0, bytes.Length);
                    txcount += bytes.Length;
                    TxCount.Text = txcount.ToString();

                    return true;
                }
                else
                {
                    txcount += Data_String.Length/2;
                    TxCount.Text = txcount.ToString();
                    return SerialPortWrite(textToSend, false);
                }
            }
        }

        private void AutoSendData()
        {
            bool ret = SendData();

            if (ret == false)
            {
                return;
            }

            // 启动自动发送定时器
            StartAutoSendDataTimer(GetAutoSendDataInterval());

            // 提示处于自动发送状态
            //progressBar.Visibility = Visibility.Visible;
            //Information("串口自动发送数据中...");
        }
        private void RecvDataBoxAppend(string textData)
        {
            this.recvDataRichTextBox.AppendText(textData);
            this.recvDataRichTextBox.ScrollToEnd();
        }
        private void BtnScanPort_Click(object sender, RoutedEventArgs e)
        {
            // 初始化可用的串口列表
            InitializeSerialPortList();
        }

        private void TX_CountClear_Click(object sender, RoutedEventArgs e)
        {
            TxCount.Clear();
            txcount = 0;
        }


        private void sendDataTextBoxClr_Click(object sender, RoutedEventArgs e)
        {
            sendDataTextBox.Clear();
            TxCount.Clear();
            txcount = 0;
        }
    }

}
