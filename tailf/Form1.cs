using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace tailf
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = @"\\MARKETSPEED005\log\" + DateTime.Now.ToString("yyyyMMdd") + "cs.txt";
        }

        private void Tail(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            using (FileStream stream = fi.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Boolean state = false;
                int size = (int)fi.Length;
                stream.Seek(size, SeekOrigin.Current);
                using (FileSystemWatcher fw = new FileSystemWatcher(fi.DirectoryName, fi.Name))
                {
                    Action<Object, FileSystemEventArgs> ReadText = (sender, e) =>
                    {
                        fi.Refresh();
                        Byte[] al = new Byte[] { };
                        int RemainingSize = (int)fi.Length - size;
                        if (RemainingSize <= 0) return;
                        Array.Resize<Byte>(ref al, RemainingSize);
                        int result = stream.Read(al, 0, RemainingSize);
                        size = (int)fi.Length;
                        Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText(Encoding.GetEncoding("sjis").GetString(al));
                        }));
                    };
                    fw.InternalBufferSize = 4*4096;
                    fw.NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.LastAccess;
                    fw.Changed += new FileSystemEventHandler(ReadText);
                    fw.EnableRaisingEvents = true;
                    while (!state) ;
                }
            }
        }

        private void TailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = textBox1.Text;
            richTextBox1.AppendText(path+Environment.NewLine);

            if (File.Exists(path))
            {
                Task.Run(() => Tail(textBox1.Text));
            } else
            {
                richTextBox1.AppendText("File not found.");
            }

        }
    }
}
