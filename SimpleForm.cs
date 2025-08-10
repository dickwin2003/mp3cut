using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using NAudio.Wave;
using NAudio.Lame;

namespace SimpleMp3Tool
{
    public partial class MainForm : Form
    {
        private ListBox fileListBox;
        private Button clearButton;
        private Button copyButton;
        private Button renameButton;
        private Button playlistButton;
        private TextBox outputPathTextBox;
        private Button browseOutputButton;
        private Label statusLabel;
        private ProgressBar progressBar;
        private Button cutByTimeButton;
        private Button cutBySegmentsButton;
        private Button mergeButton;
        private TextBox startTimeTextBox;
        private TextBox endTimeTextBox;
        private NumericUpDown segmentsNumericUpDown;

        public MainForm()
        {
            InitializeComponent();
            SetupDragDrop();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // 主窗体设置
            this.Text = "MP3文件管理工具";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AllowDrop = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.AutoScaleMode = AutoScaleMode.None;
            this.AutoSize = false;
            this.MinimumSize = new Size(900, 500);
            this.MaximumSize = new Size(900, 1200); // 只允许纵向拉伸

            // 文件列表框
            fileListBox = new ListBox();
            fileListBox.Location = new Point(12, 12);
            fileListBox.Size = new Size(860, 220);
            fileListBox.AllowDrop = true;
            fileListBox.SelectionMode = SelectionMode.MultiExtended;
            fileListBox.HorizontalScrollbar = true;
            fileListBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(fileListBox);

            // 清空按钮
            clearButton = new Button();
            clearButton.Text = "清空列表";
            clearButton.Location = new Point(12, 245);
            clearButton.Size = new Size(100, 35);
            clearButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            clearButton.Click += ClearButton_Click;
            this.Controls.Add(clearButton);

            // 操作按钮组
            var operationGroupBox = new GroupBox();
            operationGroupBox.Text = "文件操作";
            operationGroupBox.Location = new Point(12, 290);
            operationGroupBox.Size = new Size(860, 80);
            this.Controls.Add(operationGroupBox);

            copyButton = new Button();
            copyButton.Text = "复制文件";
            copyButton.Location = new Point(20, 25);
            copyButton.Size = new Size(100, 35);
            copyButton.Click += CopyButton_Click;
            operationGroupBox.Controls.Add(copyButton);

            renameButton = new Button();
            renameButton.Text = "重命名复制";
            renameButton.Location = new Point(140, 25);
            renameButton.Size = new Size(100, 35);
            renameButton.Click += RenameButton_Click;
            operationGroupBox.Controls.Add(renameButton);

            playlistButton = new Button();
            playlistButton.Text = "创建播放列表";
            playlistButton.Location = new Point(260, 25);
            playlistButton.Size = new Size(120, 35);
            playlistButton.Click += PlaylistButton_Click;
            operationGroupBox.Controls.Add(playlistButton);

            // 剪切功能区
            var cutGroupBox = new GroupBox();
            cutGroupBox.Text = "音频剪切";
            cutGroupBox.Location = new Point(12, 410);
            cutGroupBox.Size = new Size(860, 100);
            this.Controls.Add(cutGroupBox);

            var startLabel = new Label();
            startLabel.Text = "开始时间(mm:ss):";
            startLabel.Location = new Point(10, 25);
            startLabel.Size = new Size(110, 20);
            cutGroupBox.Controls.Add(startLabel);

            startTimeTextBox = new TextBox();
            startTimeTextBox.Location = new Point(120, 23);
            startTimeTextBox.Size = new Size(60, 20);
            startTimeTextBox.Text = "00:00";
            cutGroupBox.Controls.Add(startTimeTextBox);

            var endLabel = new Label();
            endLabel.Text = "结束时间(mm:ss):";
            endLabel.Location = new Point(200, 25);
            endLabel.Size = new Size(110, 20);
            cutGroupBox.Controls.Add(endLabel);

            endTimeTextBox = new TextBox();
            endTimeTextBox.Location = new Point(310, 23);
            endTimeTextBox.Size = new Size(60, 20);
            endTimeTextBox.Text = "01:00";
            cutGroupBox.Controls.Add(endTimeTextBox);

            cutByTimeButton = new Button();
            cutByTimeButton.Text = "按时间剪切";
            cutByTimeButton.Location = new Point(400, 20);
            cutByTimeButton.Size = new Size(100, 30);
            cutByTimeButton.Click += CutByTimeButton_Click;
            cutGroupBox.Controls.Add(cutByTimeButton);

            var segLabel = new Label();
            segLabel.Text = "分段数:";
            segLabel.Location = new Point(10, 60);
            segLabel.Size = new Size(60, 20);
            cutGroupBox.Controls.Add(segLabel);

            segmentsNumericUpDown = new NumericUpDown();
            segmentsNumericUpDown.Location = new Point(70, 58);
            segmentsNumericUpDown.Size = new Size(50, 20);
            segmentsNumericUpDown.Minimum = 2;
            segmentsNumericUpDown.Maximum = 99;
            segmentsNumericUpDown.Value = 5;
            cutGroupBox.Controls.Add(segmentsNumericUpDown);

            cutBySegmentsButton = new Button();
            cutBySegmentsButton.Text = "按段数剪切";
            cutBySegmentsButton.Location = new Point(140, 55);
            cutBySegmentsButton.Size = new Size(100, 30);
            cutBySegmentsButton.Click += CutBySegmentsButton_Click;
            cutGroupBox.Controls.Add(cutBySegmentsButton);

            // 合并按钮
            mergeButton = new Button();
            mergeButton.Text = "合并音频";
            mergeButton.Location = new Point(260, 55);
            mergeButton.Size = new Size(100, 30);
            mergeButton.Click += MergeButton_Click;
            cutGroupBox.Controls.Add(mergeButton);

            // 输出路径
            var outputLabel = new Label();
            outputLabel.Text = "输出路径:";
            outputLabel.Location = new Point(12, 525);
            outputLabel.Size = new Size(80, 20);
            outputLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.Controls.Add(outputLabel);

            outputPathTextBox = new TextBox();
            outputPathTextBox.Location = new Point(100, 523);
            outputPathTextBox.Size = new Size(650, 25);
            outputPathTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            outputPathTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(outputPathTextBox);

            browseOutputButton = new Button();
            browseOutputButton.Text = "浏览";
            browseOutputButton.Location = new Point(760, 521);
            browseOutputButton.Size = new Size(60, 28);
            browseOutputButton.Click += BrowseOutputButton_Click;
            browseOutputButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.Controls.Add(browseOutputButton);

            // 进度条
            progressBar = new ProgressBar();
            progressBar.Location = new Point(12, 560);
            progressBar.Size = new Size(860, 20);
            progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(progressBar);

            // 状态标签
            statusLabel = new Label();
            statusLabel.Text = "就绪 - 请拖拽MP3文件到上方列表";
            statusLabel.Location = new Point(12, 590);
            statusLabel.Size = new Size(860, 20);
            statusLabel.ForeColor = Color.Green;
            statusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
            int addedCount = 0;
            
            foreach (string file in files)
            {
                if (Path.GetExtension(file).ToLower() == ".mp3")
                {
                    if (!fileListBox.Items.Contains(file))
                    {
                        fileListBox.Items.Add(file);
                        addedCount++;
                    }
                }
            }
            
            statusLabel.Text = $"已添加 {addedCount} 个新文件，总共 {fileListBox.Items.Count} 个MP3文件";
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            fileListBox.Items.Clear();
            statusLabel.Text = "列表已清空";
            progressBar.Value = 0;
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (fileListBox.Items.Count == 0)
            {
                MessageBox.Show("请先添加MP3文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ProcessFiles("copy");
        }

        private void RenameButton_Click(object sender, EventArgs e)
        {
            if (fileListBox.Items.Count == 0)
            {
                MessageBox.Show("请先添加MP3文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ProcessFiles("rename");
        }

        private void PlaylistButton_Click(object sender, EventArgs e)
        {
            if (fileListBox.Items.Count == 0)
            {
                MessageBox.Show("请先添加MP3文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            CreatePlaylist();
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
                MessageBox.Show("时间格式错误，请用mm:ss格式", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void ProcessFiles(string operation)
        {
            try
            {
                string outputPath = outputPathTextBox.Text;
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                progressBar.Value = 0;
                progressBar.Maximum = fileListBox.Items.Count;

                int count = 0;
                foreach (string file in fileListBox.Items)
                {
                    string fileName = Path.GetFileName(file);
                    string extension = Path.GetExtension(file);
                    string destFile;

                    if (operation == "copy")
                    {
                        destFile = Path.Combine(outputPath, $"copy_{DateTime.Now:yyyyMMdd}_{count + 1:D2}_{fileName}");
                    }
                    else // rename
                    {
                        destFile = Path.Combine(outputPath, $"track_{count + 1:D2}{extension}");
                    }

                    File.Copy(file, destFile, true);
                    count++;
                    progressBar.Value = count;
                    
                    statusLabel.Text = $"正在处理: {count}/{fileListBox.Items.Count}";
                    Application.DoEvents();
                }

                string operationName = operation == "copy" ? "复制" : "重命名";
                statusLabel.Text = $"{operationName}完成！共处理 {count} 个文件到 {outputPath}";
                MessageBox.Show($"{operationName}完成！\n共处理 {count} 个文件\n保存位置: {outputPath}", 
                    "操作完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "操作失败";
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreatePlaylist()
        {
            try
            {
                string outputPath = outputPathTextBox.Text;
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                string playlistFile = Path.Combine(outputPath, $"playlist_{DateTime.Now:yyyyMMdd_HHmmss}.m3u");
                
                using (var writer = new StreamWriter(playlistFile, false, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine("#EXTM3U");
                    writer.WriteLine($"# Created by MP3文件管理工具 on {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine();

                    int index = 1;
                    foreach (string file in fileListBox.Items)
                    {
                        string title = Path.GetFileNameWithoutExtension(file);
                        writer.WriteLine($"#EXTINF:-1,{index:D2}. {title}");
                        writer.WriteLine(file);
                        writer.WriteLine();
                        index++;
                    }
                }

                statusLabel.Text = $"播放列表已创建: {playlistFile}";
                MessageBox.Show($"播放列表创建成功！\n文件: {playlistFile}\n\n可以用音乐播放器打开此文件播放所有歌曲", 
                    "创建成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "创建播放列表失败";
                MessageBox.Show($"创建播放列表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CutAudioByTime(string inputFile, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                statusLabel.Text = "正在剪切音频...";
                progressBar.Value = 0;
                string fileName = Path.GetFileNameWithoutExtension(inputFile);
                string outputFile = Path.Combine(outputPathTextBox.Text, $@"{fileName}_cut_{startTime:mm\-ss}_to_{endTime:mm\-ss}.mp3");
                using (var reader = new Mp3FileReader(inputFile))
                {
                    var startBytes = (long)(reader.Length * startTime.TotalSeconds / reader.TotalTime.TotalSeconds);
                    var endBytes = (long)(reader.Length * endTime.TotalSeconds / reader.TotalTime.TotalSeconds);
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
                        var endTime = i == segments - 1 ? totalDuration : TimeSpan.FromSeconds(segmentDuration.TotalSeconds * (i + 1));
                        string outputFile = Path.Combine(outputPathTextBox.Text, $"{fileName}_segment_{i + 1:D2}.mp3");
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
                string outputFile = Path.Combine(outputPathTextBox.Text, $"merged_{DateTime.Now:yyyyMMdd_HHmmss}.mp3");
                using (var firstFile = new Mp3FileReader(inputFiles[0]))
                {
                    using (var writer = new LameMP3FileWriter(outputFile, firstFile.WaveFormat, LAMEPreset.STANDARD))
                    {
                        firstFile.CopyTo(writer);
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

        private void BrowseOutputButton_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "选择输出文件夹";
                folderDialog.SelectedPath = outputPathTextBox.Text;
                
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    outputPathTextBox.Text = folderDialog.SelectedPath;
                    statusLabel.Text = $"输出路径已设置为: {folderDialog.SelectedPath}";
                }
            }
        }
    }
}
