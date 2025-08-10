using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.Lame;

namespace Mp3CutMerge
{
    public partial class MainForm : Form
    {
        private ListBox fileListBox;
        private Button clearButton;
        private Button cutByTimeButton;
        private Button cutBySegmentsButton;
        private Button mergeButton;
        private TextBox startTimeTextBox;
        private TextBox endTimeTextBox;
        private NumericUpDown segmentsNumericUpDown;
        private TextBox outputPathTextBox;
        private Button browseOutputButton;
        private ProgressBar progressBar;
        private Label statusLabel;

        public MainForm()
        {
            InitializeComponent();
            SetupDragDrop();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // 主窗体设置
            this.Text = "MP3剪切合并工具";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AllowDrop = true;

            // 文件列表框
            fileListBox = new ListBox();
            fileListBox.Location = new System.Drawing.Point(12, 12);
            fileListBox.Size = new System.Drawing.Size(760, 150);
            fileListBox.AllowDrop = true;
            fileListBox.SelectionMode = SelectionMode.MultiExtended;
            this.Controls.Add(fileListBox);

            // 清空按钮
            clearButton = new Button();
            clearButton.Text = "清空列表";
            clearButton.Location = new System.Drawing.Point(12, 175);
            clearButton.Size = new System.Drawing.Size(100, 30);
            clearButton.Click += ClearButton_Click;
            this.Controls.Add(clearButton);

            // 时间剪切组
            var timeGroupBox = new GroupBox();
            timeGroupBox.Text = "按时间剪切";
            timeGroupBox.Location = new System.Drawing.Point(12, 220);
            timeGroupBox.Size = new System.Drawing.Size(370, 120);
            this.Controls.Add(timeGroupBox);

            var startLabel = new Label();
            startLabel.Text = "开始时间 (mm:ss):";
            startLabel.Location = new System.Drawing.Point(10, 25);
            startLabel.Size = new System.Drawing.Size(120, 20);
            timeGroupBox.Controls.Add(startLabel);

            startTimeTextBox = new TextBox();
            startTimeTextBox.Location = new System.Drawing.Point(140, 23);
            startTimeTextBox.Size = new System.Drawing.Size(80, 20);
            startTimeTextBox.Text = "00:00";
            timeGroupBox.Controls.Add(startTimeTextBox);

            var endLabel = new Label();
            endLabel.Text = "结束时间 (mm:ss):";
            endLabel.Location = new System.Drawing.Point(10, 55);
            endLabel.Size = new System.Drawing.Size(120, 20);
            timeGroupBox.Controls.Add(endLabel);

            endTimeTextBox = new TextBox();
            endTimeTextBox.Location = new System.Drawing.Point(140, 53);
            endTimeTextBox.Size = new System.Drawing.Size(80, 20);
            endTimeTextBox.Text = "01:00";
            timeGroupBox.Controls.Add(endTimeTextBox);

            cutByTimeButton = new Button();
            cutByTimeButton.Text = "按时间剪切";
            cutByTimeButton.Location = new System.Drawing.Point(250, 25);
            cutByTimeButton.Size = new System.Drawing.Size(100, 50);
            cutByTimeButton.Click += CutByTimeButton_Click;
            timeGroupBox.Controls.Add(cutByTimeButton);

            // 段数剪切组
            var segmentGroupBox = new GroupBox();
            segmentGroupBox.Text = "按段数剪切";
            segmentGroupBox.Location = new System.Drawing.Point(402, 220);
            segmentGroupBox.Size = new System.Drawing.Size(370, 120);
            this.Controls.Add(segmentGroupBox);

            var segmentLabel = new Label();
            segmentLabel.Text = "分割段数:";
            segmentLabel.Location = new System.Drawing.Point(10, 35);
            segmentLabel.Size = new System.Drawing.Size(80, 20);
            segmentGroupBox.Controls.Add(segmentLabel);

            segmentsNumericUpDown = new NumericUpDown();
            segmentsNumericUpDown.Location = new System.Drawing.Point(100, 33);
            segmentsNumericUpDown.Size = new System.Drawing.Size(60, 20);
            segmentsNumericUpDown.Minimum = 1;
            segmentsNumericUpDown.Maximum = 100;
            segmentsNumericUpDown.Value = 6;
            segmentGroupBox.Controls.Add(segmentsNumericUpDown);

            cutBySegmentsButton = new Button();
            cutBySegmentsButton.Text = "按段数剪切";
            cutBySegmentsButton.Location = new System.Drawing.Point(200, 25);
            cutBySegmentsButton.Size = new System.Drawing.Size(100, 50);
            cutBySegmentsButton.Click += CutBySegmentsButton_Click;
            segmentGroupBox.Controls.Add(cutBySegmentsButton);

            // 合并按钮
            mergeButton = new Button();
            mergeButton.Text = "合并音频";
            mergeButton.Location = new System.Drawing.Point(12, 360);
            mergeButton.Size = new System.Drawing.Size(120, 40);
            mergeButton.Click += MergeButton_Click;
            this.Controls.Add(mergeButton);

            // 输出路径
            var outputLabel = new Label();
            outputLabel.Text = "输出路径:";
            outputLabel.Location = new System.Drawing.Point(12, 420);
            outputLabel.Size = new System.Drawing.Size(80, 20);
            this.Controls.Add(outputLabel);

            outputPathTextBox = new TextBox();
            outputPathTextBox.Location = new System.Drawing.Point(100, 418);
            outputPathTextBox.Size = new System.Drawing.Size(550, 20);
            outputPathTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            this.Controls.Add(outputPathTextBox);

            browseOutputButton = new Button();
            browseOutputButton.Text = "浏览";
            browseOutputButton.Location = new System.Drawing.Point(660, 416);
            browseOutputButton.Size = new System.Drawing.Size(60, 25);
            browseOutputButton.Click += BrowseOutputButton_Click;
            this.Controls.Add(browseOutputButton);

            // 进度条
            progressBar = new ProgressBar();
            progressBar.Location = new System.Drawing.Point(12, 460);
            progressBar.Size = new System.Drawing.Size(760, 20);
            this.Controls.Add(progressBar);

            // 状态标签
            statusLabel = new Label();
            statusLabel.Text = "就绪";
            statusLabel.Location = new System.Drawing.Point(12, 490);
            statusLabel.Size = new System.Drawing.Size(760, 20);
            this.Controls.Add(statusLabel);

            this.ResumeLayout(false);
        }

        private void SetupDragDrop()
        {
            this.DragEnter += MainForm_DragEnter;
            this.DragDrop += MainForm_DragDrop;
            fileListBox.DragEnter += MainForm_DragEnter;
            fileListBox.DragDrop += MainForm_DragDrop;
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Any(f => Path.GetExtension(f).ToLower() == ".mp3"))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (Path.GetExtension(file).ToLower() == ".mp3")
                {
                    if (!fileListBox.Items.Contains(file))
                    {
                        fileListBox.Items.Add(file);
                    }
                }
            }
            statusLabel.Text = $"已添加 {fileListBox.Items.Count} 个文件";
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            fileListBox.Items.Clear();
            statusLabel.Text = "列表已清空";
        }

        private void CutByTimeButton_Click(object sender, EventArgs e)
        {
            if (fileListBox.SelectedItem == null)
            {
                MessageBox.Show("请选择一个MP3文件进行剪切", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string inputFile = fileListBox.SelectedItem.ToString();
            
            if (!TimeSpan.TryParseExact(startTimeTextBox.Text, @"mm\:ss", null, out TimeSpan startTime) ||
                !TimeSpan.TryParseExact(endTimeTextBox.Text, @"mm\:ss", null, out TimeSpan endTime))
            {
                MessageBox.Show("时间格式错误，请使用 mm:ss 格式", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (startTime >= endTime)
            {
                MessageBox.Show("开始时间必须小于结束时间", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CutAudioByTime(inputFile, startTime, endTime);
        }

        private void CutBySegmentsButton_Click(object sender, EventArgs e)
        {
            if (fileListBox.SelectedItem == null)
            {
                MessageBox.Show("请选择一个MP3文件进行剪切", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string inputFile = fileListBox.SelectedItem.ToString();
            int segments = (int)segmentsNumericUpDown.Value;

            CutAudioBySegments(inputFile, segments);
        }

        private void MergeButton_Click(object sender, EventArgs e)
        {
            if (fileListBox.Items.Count < 2)
            {
                MessageBox.Show("请至少选择两个MP3文件进行合并", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<string> files = fileListBox.Items.Cast<string>().ToList();
            MergeAudioFiles(files);
        }

        private void BrowseOutputButton_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.SelectedPath = outputPathTextBox.Text;
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    outputPathTextBox.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void CutAudioByTime(string inputFile, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                statusLabel.Text = "正在剪切音频...";
                progressBar.Value = 0;

                string fileName = Path.GetFileNameWithoutExtension(inputFile);
                string outputFile = Path.Combine(outputPathTextBox.Text, 
                    $"{fileName}_cut_{startTime:mm\\-ss}_to_{endTime:mm\\-ss}.mp3");

                using (var reader = new Mp3FileReader(inputFile))
                {
                    var startBytes = (long)(reader.TotalTime.TotalSeconds > 0 ? 
                        reader.Length * startTime.TotalSeconds / reader.TotalTime.TotalSeconds : 0);
                    var endBytes = (long)(reader.TotalTime.TotalSeconds > 0 ? 
                        reader.Length * endTime.TotalSeconds / reader.TotalTime.TotalSeconds : reader.Length);

                    reader.Position = startBytes;
                    var bytesToRead = endBytes - startBytes;

                    using (var writer = new LameMP3FileWriter(outputFile, reader.WaveFormat, LAMEPreset.STANDARD))
                    {
                        var buffer = new byte[4096];
                        long totalBytesRead = 0;

                        while (totalBytesRead < bytesToRead)
                        {
                            var bytesToReadNow = (int)Math.Min(buffer.Length, bytesToRead - totalBytesRead);
                            var bytesRead = reader.Read(buffer, 0, bytesToReadNow);
                            
                            if (bytesRead == 0) break;
                            
                            writer.Write(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;

                            progressBar.Value = (int)(totalBytesRead * 100 / bytesToRead);
                            Application.DoEvents();
                        }
                    }
                }

                progressBar.Value = 100;
                statusLabel.Text = $"剪切完成: {outputFile}";
                MessageBox.Show($"剪切完成！\n输出文件: {outputFile}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "剪切失败";
                MessageBox.Show($"剪切失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CutAudioBySegments(string inputFile, int segments)
        {
            try
            {
                statusLabel.Text = "正在按段剪切音频...";
                progressBar.Value = 0;

                using (var reader = new Mp3FileReader(inputFile))
                {
                    var totalDuration = reader.TotalTime;
                    var segmentDuration = TimeSpan.FromSeconds(totalDuration.TotalSeconds / segments);

                    string fileName = Path.GetFileNameWithoutExtension(inputFile);

                    for (int i = 0; i < segments; i++)
                    {
                        var startTime = TimeSpan.FromSeconds(segmentDuration.TotalSeconds * i);
                        var endTime = i == segments - 1 ? totalDuration : 
                            TimeSpan.FromSeconds(segmentDuration.TotalSeconds * (i + 1));

                        string outputFile = Path.Combine(outputPathTextBox.Text, 
                            $"{fileName}_segment_{i + 1:D2}.mp3");

                        var startBytes = (long)(reader.Length * startTime.TotalSeconds / totalDuration.TotalSeconds);
                        var endBytes = (long)(reader.Length * endTime.TotalSeconds / totalDuration.TotalSeconds);

                        reader.Position = startBytes;
                        var bytesToRead = endBytes - startBytes;

                        using (var writer = new LameMP3FileWriter(outputFile, reader.WaveFormat, LAMEPreset.STANDARD))
                        {
                            var buffer = new byte[4096];
                            long totalBytesRead = 0;

                            while (totalBytesRead < bytesToRead)
                            {
                                var bytesToReadNow = (int)Math.Min(buffer.Length, bytesToRead - totalBytesRead);
                                var bytesRead = reader.Read(buffer, 0, bytesToReadNow);
                                
                                if (bytesRead == 0) break;
                                
                                writer.Write(buffer, 0, bytesRead);
                                totalBytesRead += bytesRead;
                            }
                        }

                        progressBar.Value = (i + 1) * 100 / segments;
                        statusLabel.Text = $"正在处理第 {i + 1}/{segments} 段...";
                        Application.DoEvents();
                    }
                }

                progressBar.Value = 100;
                statusLabel.Text = $"按段剪切完成，共生成 {segments} 个文件";
                MessageBox.Show($"按段剪切完成！\n共生成 {segments} 个文件", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "按段剪切失败";
                MessageBox.Show($"按段剪切失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MergeAudioFiles(List<string> inputFiles)
        {
            try
            {
                statusLabel.Text = "正在合并音频文件...";
                progressBar.Value = 0;

                string outputFile = Path.Combine(outputPathTextBox.Text, 
                    $"merged_{DateTime.Now:yyyyMMdd_HHmmss}.mp3");

                using (var firstFile = new Mp3FileReader(inputFiles[0]))
                {
                    using (var writer = new LameMP3FileWriter(outputFile, firstFile.WaveFormat, LAMEPreset.STANDARD))
                    {
                        // 写入第一个文件
                        firstFile.CopyTo(writer);
                        
                        // 写入其余文件
                        for (int i = 1; i < inputFiles.Count; i++)
                        {
                            using (var reader = new Mp3FileReader(inputFiles[i]))
                            {
                                reader.CopyTo(writer);
                            }
                            
                            progressBar.Value = (i + 1) * 100 / inputFiles.Count;
                            statusLabel.Text = $"正在合并第 {i + 1}/{inputFiles.Count} 个文件...";
                            Application.DoEvents();
                        }
                    }
                }

                progressBar.Value = 100;
                statusLabel.Text = $"合并完成: {outputFile}";
                MessageBox.Show($"合并完成！\n输出文件: {outputFile}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "合并失败";
                MessageBox.Show($"合并失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
