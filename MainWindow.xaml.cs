using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace serial_assistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort  serialPort;

        public MainWindow()
        {
            InitializeComponent();
            // 初始化可用的串口列表
            InitializeSerialPortList();
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
        /// <summary>
        /// 定义打开串口的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOpenSerialPort_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                // 关闭串口
                if (CloseSerialPort())
                {
                    BtnOpenSerialPort.Content = "Connect";
                    BtnOpenSerialPort.Background = Brushes.Red;
                }
            }
            else
            {
                // 打开串口
                if (OpenSerialPort())
                {
                    BtnOpenSerialPort.Content = "Disconnect";
                    BtnOpenSerialPort.Background = Brushes.Green;
                }
            }
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
                serialPort.Open();
                serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived); 
                flag = true;
            }
            catch (Exception)
            {

                throw;
            }

            return flag;
        }
        
        /// <summary>
        /// 串口数据的自动接收数据的事件方法逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // 获取串口缓冲区的字节数
            int count = serialPort.BytesToRead;
            // 实例接收串口中数据的缓冲区
            byte[] buffer = new byte[count];
            // 把串口中的数据读取到缓冲区
            serialPort.Read(buffer, 0, count);
            // 需要把读取的结果转换为一个字符串
            string receivedData = Encoding.UTF8.GetString(buffer);
            // 切换到UI线程给对应接收数据的文本框赋值
            Dispatcher.Invoke(() =>
            {
                txtReceive.Text = receivedData;
            });
        }

        /// <summary>
        /// 消息发送事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //获取文本框中发送的数据
                var datas = Encoding.Default.GetBytes(txtSend.Text);
                //向指定串口写入数据
                serialPort.Write(datas, 0, datas.Length);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 消息清空事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClearTx_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 清空发送区数据
      
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
