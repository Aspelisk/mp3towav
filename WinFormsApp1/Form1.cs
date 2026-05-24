using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;

namespace Mp3ToWavConverter
{
    public class Form1 : Form
    {
        // Поля с явной инициализацией (чтобы убрать CS8618)
        private TextBox txtMp3Path = null!;
        private Button btnBrowse = null!;
        private Button btnConvert = null!;
        private Label lblInfo = null!;
        private SaveFileDialog saveFileDialog = null!;

        public Form1()
        {
            InitializeComponentManual();
        }

        private void InitializeComponentManual()
        {
            // Настройка формы
            Text = "MP3 → WAV Конвертер";
            Size = new Size(600, 180);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Метка
            lblInfo = new Label
            {
                Text = "Выберите MP3 файл:",
                Location = new Point(20, 25),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            // Поле пути
            txtMp3Path = new TextBox
            {
                Location = new Point(140, 22),
                Size = new Size(320, 27),
                ReadOnly = true,
                BackColor = Color.WhiteSmoke
            };

            // Кнопка "Обзор"
            btnBrowse = new Button
            {
                Text = "Обзор...",
                Location = new Point(470, 20),
                Size = new Size(90, 30)
            };
            btnBrowse.Click += btnBrowse_Click;   // упрощённая подписка (без new EventHandler)

            // Кнопка "Конвертировать"
            btnConvert = new Button
            {
                Text = "Конвертировать в WAV",
                Location = new Point(200, 80),
                Size = new Size(200, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnConvert.Click += btnConvert_Click;

            // Диалог сохранения
            saveFileDialog = new SaveFileDialog
            {
                Filter = "WAV файлы|*.wav",
                DefaultExt = "wav",
                Title = "Сохранить WAV файл как..."
            };

            // Добавляем элементы
            Controls.Add(lblInfo);
            Controls.Add(txtMp3Path);
            Controls.Add(btnBrowse);
            Controls.Add(btnConvert);
        }

        // Обработчик кнопки "Обзор" (sender теперь object? для совместимости с nullable)
        private void btnBrowse_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new()   // упрощённый using
            {
                Filter = "MP3 файлы|*.mp3",
                Title = "Выберите MP3 файл"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtMp3Path.Text = openFileDialog.FileName;
            }
        }

        // Обработчик кнопки "Конвертировать"
        private void btnConvert_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMp3Path.Text))
            {
                MessageBox.Show("Сначала выберите MP3 файл!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(txtMp3Path.Text))
            {
                MessageBox.Show("Файл не найден!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(txtMp3Path.Text) + ".wav";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string outputWavPath = saveFileDialog.FileName;

            try
            {
                btnConvert.Enabled = false;
                btnConvert.Text = "Конвертирую...";
                UseWaitCursor = true;

                using var mp3Reader = new Mp3FileReader(txtMp3Path.Text);
                WaveFileWriter.CreateWaveFile(outputWavPath, mp3Reader);

                MessageBox.Show($"Конвертация завершена!\n\nWAV сохранён:\n{outputWavPath}",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при конвертации:\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConvert.Enabled = true;
                btnConvert.Text = "Конвертировать в WAV";
                UseWaitCursor = false;
            }
        }
    }
}