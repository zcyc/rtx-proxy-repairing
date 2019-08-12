using SharpConfig;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace rtx自动修复代理工具
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        String UserPath = "";
        String RtxPath = "";
        DirectoryInfo dirD;
        FileSystemInfo[] files;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
            {
                try
                {
                    string str = openFolderDialog.SelectedPath;
                    string tSt;
                    int i = 14;
                    tSt = str.Substring(str.Length - i);
                    if (tSt == "RTXC File List")
                    {
                        but1.Content = openFolderDialog.SelectedPath;
                        UserPath = openFolderDialog.SelectedPath;
                    }
                    else
                    {
                        but1.Content = openFolderDialog.SelectedPath + "\\RTXC File List";
                        UserPath = openFolderDialog.SelectedPath + "\\RTXC File List";
                    }
                    DirectoryInfo dir = new DirectoryInfo(UserPath);

                    //判断所指的文件夹/文件是否存在  
                    if (!dir.Exists)
                        return;
                    dirD = dir as DirectoryInfo;
                    files = dirD.GetFileSystemInfos();
                    UserPath = files[0].FullName + "\\Accounts";
                    var str1 = files[0].Name.Replace("_", "\\");
                    var str2 = str1.Insert(1, ":");
                    RtxPath = str2 + "\\Accounts";
                    //MessageBox.Show(UserPath);
                    //MessageBox.Show(RtxPath);
                    But3_Click(RtxPath, UserPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                //foreach (FileSystemInfo FSys in files) {
                //    Console.WriteLine(FSys.FullName);
                //}

            }
        }

        private void But3_Click(String RtxPath1, String UserPath1)
        {
            try
            {
                string str = "mklink /J \"" + RtxPath1 + "\" \"" + UserPath1 + "\"";
                Console.WriteLine(str);
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.Start();
                p.StandardInput.WriteLine(str + "&exit");
                p.StandardInput.AutoFlush = true;
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
                setproxy(RtxPath1,proxyip.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog openFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
                if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
                {
                    canelproxy.Content = openFolderDialog.SelectedPath + "\\Accounts";
                    setproxy(openFolderDialog.SelectedPath+ "\\Accounts", "");
                }    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void setproxy(String RtxPath1,String proip) {
            String cfgfile = RtxPath1 + "\\rtx.cfg";
            //MessageBox.Show(cfgfile);
            Configuration config = Configuration.LoadFromFile(cfgfile);
            string ip = proip;
            foreach (SharpConfig.Section item in config)
            {
                if (item.Name == "ProxyConfig")
                {
                    item["ServerAddr"].StringValue = ip.Replace(" ", "");
                }
            }
            config.SaveToFile(cfgfile, Encoding.UTF8);
        }
    }
}
